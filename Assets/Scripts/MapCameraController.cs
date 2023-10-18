using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-make the max zoom relative to the current min max
//-first clamp to current min max and then clamp to map min max
//-add icon scaling


public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer mapSprite;
    [SerializeField] private Camera mapCamera;

    [Header("Control")]
    [SerializeField] [Range(0, 1)] private float slideDrag;
    [SerializeField] private float slideForce;
    [SerializeField] private float zoomStep;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSmoothSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private int memorySize;

    [Header("Map Progression")]
    [SerializeField] private Vector3 startPos;
    [SerializeField] private float startMinX, startMinY, startMaxX, startMaxY;
    [Space(20)]
    [SerializeField] private float leftGrowthStep;
    [SerializeField] private float rightGrowthStep, bottomGrowthStep, topGrowthStep;

    //Control
    private Vector3Memory _mPosMemory;
    private Vector3 _camTarget;
    private Vector3 _clickPos;
    private float _zoomTarget;
    private float _minMapX, _minMapY, _maxMapX, _maxMapY;

    //progression
    private float currentMinX, currentMinY, currentMaxX, currentMaxY;


    private void Awake()
    {
        //get end points of map
        _minMapX = mapSprite.transform.position.x - mapSprite.bounds.size.x / 2;
        _minMapY = mapSprite.transform.position.y - mapSprite.bounds.size.y / 2;
        _maxMapX = mapSprite.transform.position.x + mapSprite.bounds.size.x / 2;
        _maxMapY = mapSprite.transform.position.y + mapSprite.bounds.size.y / 2;

        Vector3 originPos = mapSprite.transform.position + startPos;
        mapCamera.transform.position = originPos;
        currentMaxX = originPos.x + startMaxX;
        currentMaxY = originPos.y + startMaxY;
        currentMinX = originPos.x + startMinX;
        currentMinY = originPos.y + startMinY;

        _mPosMemory = new Vector3Memory(memorySize);
        _camTarget = mapCamera.transform.position;
        _zoomTarget = mapCamera.orthographicSize;

        AdjustMaxZoom();
    }


    private void Update()
    {
        MoveTarget();
        Smooth();
        Zoom();
        LimitCamMovement();
    }

    private void MoveTarget() //TBI
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickPos = mapCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 diff = _clickPos - mapCamera.ScreenToWorldPoint(Input.mousePosition);
            _camTarget += diff;
            _mPosMemory.Add(_camTarget);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopCoroutine(CameraSlide());
            StartCoroutine(CameraSlide());
        }

    }

    private void Smooth()
    {
        mapCamera.transform.position = Vector3.Lerp(mapCamera.transform.position, _camTarget, moveSpeed * Time.deltaTime);
        mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, _zoomTarget, zoomSmoothSpeed * Time.deltaTime);

    }

    private void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0)
        {
            ZoomOut();
        }
        else if (scroll > 0)
        {
            ZoomIn();
        }

        _zoomTarget = Mathf.Clamp(_zoomTarget, minZoom, maxZoom);

    }

    private void LimitCamMovement()
    {
        //get size of camera
        float camHeight = mapCamera.orthographicSize;
        float camWidth = camHeight * mapCamera.aspect;

        //get min/max available values for camera
        float minX = currentMinX + camWidth;
        float maxX = currentMaxX - camWidth;
        float minY = currentMinY + camHeight;
        float maxY = currentMaxY - camHeight;

        //clamping
        Vector3 newCamPos = mapCamera.transform.position;
        newCamPos.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        newCamPos.y = Mathf.Clamp(newCamPos.y, minY, maxY);
        _camTarget.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        _camTarget.y = Mathf.Clamp(newCamPos.y, minY, maxY);

        mapCamera.transform.position = newCamPos;
    }

    [ContextMenu("IncreaseArea")]
    public void IncreaseAreaSize()
    {
        if (currentMinX > _minMapX)
        {
            currentMinX -= leftGrowthStep;
            currentMinX = currentMinX < _minMapX ? _minMapX : currentMinX;
        }

        if (currentMaxX < _maxMapX)
        {
            currentMaxX += rightGrowthStep;
            currentMaxX = currentMaxX > _maxMapX ? _maxMapX : currentMaxX;
        }

        if (currentMinY > _minMapY)
        {
            currentMinY -= bottomGrowthStep;
            currentMinY = currentMinY < _minMapY ? _minMapY : currentMinY;
        }

        if (currentMaxY < _maxMapY)
        {
            currentMaxY += topGrowthStep;
            currentMaxY = currentMaxY > _maxMapY ? _maxMapY : currentMaxY;

        }

        AdjustMaxZoom();
    }

    public void IncreaseAreaSize(float topStep, float bottomStep, float leftStep, float rightStep)
    {
        if (currentMinX > _minMapX)
        {
            currentMinX -= leftStep;
            currentMinX = currentMinX < _minMapX ? _minMapX : currentMinX;
        }

        if (currentMaxX < _maxMapX)
        {
            currentMaxX += rightStep;
            currentMaxX = currentMaxX > _maxMapX ? _maxMapX : currentMaxX;
        }

        if (currentMinY > _minMapY)
        {
            currentMinY -= bottomStep;
            currentMinY = currentMinY < _minMapY ? _minMapY : currentMinY;
        }

        if (currentMaxY < _maxMapY)
        {
            currentMaxY += topStep;
            currentMaxY = currentMaxY > _maxMapY ? _maxMapY : currentMaxY;

        }

        AdjustMaxZoom();
    }

    [ContextMenu("AdjustMaxZoom")]
    private void AdjustMaxZoom()
    {
        float currentAreaHeight = (currentMaxY - currentMinY) / 2;
        float currentAreaWidth = (currentMaxX - currentMinX) / 2 / mapCamera.aspect;//divided by aspect so it will match the camera 
        maxZoom = currentAreaHeight < currentAreaWidth ? currentAreaHeight : currentAreaWidth;

        if (_zoomTarget > maxZoom)
        {
            _zoomTarget = maxZoom;
            mapCamera.orthographicSize = maxZoom;
        }
    }

    private void ZoomIn()
    {
        _zoomTarget -= zoomStep;
    }

    private void ZoomOut()
    {
        _zoomTarget += zoomStep;
    }

    private IEnumerator CameraSlide()
    {
        Vector3 slideStep = _mPosMemory.GetLatestDelta();
        float stepForce = slideForce;
        while (stepForce > 0)
        {
            _camTarget += slideStep * stepForce;
            stepForce -= slideDrag;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 originPos = mapSprite.transform.position + startPos;

        if (!Application.isPlaying)
        {
            currentMaxX = originPos.x + startMaxX;
            currentMaxY = originPos.y + startMaxY;
            currentMinX = originPos.x + startMinX;
            currentMinY = originPos.y + startMinY;
        }

        Vector3 topLeft = new Vector3(currentMinX, currentMaxY);
        Vector3 topRight = new Vector3(currentMaxX, currentMaxY);
        Vector3 bottomLeft = new Vector3(currentMinX, currentMinY);
        Vector3 bottomRight = new Vector3(currentMaxX, currentMinY);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(originPos, 0.1f);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }
}

public struct Vector3Memory
{
    public Vector3[] _vectors;

    public Vector3Memory(int memorySize)
    {
        memorySize = memorySize < 2 ? 2 : memorySize;
        _vectors = new Vector3[memorySize];
    }

    public Vector3 GetAverageVector()
    {
        Vector3 sumVec = Vector3.zero;
        foreach (var vec in _vectors)
        {
            sumVec += vec;
        }

        return sumVec / _vectors.Length;
    }

    public void Add(Vector3 vec)
    {
        for (int i = _vectors.Length - 1; i > 0; i--)
        {
            _vectors[i] = _vectors[i - 1];
        }
        _vectors[0] = vec;
    }

    public Vector3 GetLatestDelta()
    {
        Debug.Log(_vectors[0].ToString() + _vectors[1].ToString());
        return _vectors[0] - _vectors[1];
    }



}
