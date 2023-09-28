using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapSprite;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float zoomStep;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int memorySize;

    private Vector3Memory mPosMemory;
    private Vector3 followTarget;
    private Vector3 _clickPos;
    private float _minMapX, _minMapY, _maxMapX, _maxMapY;


    private void Awake()
    {
        //get end points of map
        _minMapX = mapSprite.transform.position.x - mapSprite.bounds.size.x / 2;
        _minMapY = mapSprite.transform.position.y - mapSprite.bounds.size.y / 2;
        _maxMapX = mapSprite.transform.position.x + mapSprite.bounds.size.x / 2;
        _maxMapY = mapSprite.transform.position.y + mapSprite.bounds.size.y / 2;
        followTarget = mapCamera.transform.position;

        mPosMemory = new Vector3Memory(memorySize);
    }


    private void Update()
    {
        //PanCamera();
        MoveTarget();
        CamFolllowTarget();
        Zoom();
        LimitCamMovement();
    }

    private void PanCamera()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickPos = mapCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 diff = _clickPos - mapCamera.ScreenToWorldPoint(Input.mousePosition);

            mapCamera.transform.position += diff;
        }
    }

    private void MoveTarget() //TBI
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickPos = mapCamera.ScreenToWorldPoint(Input.mousePosition);
            //mPosMemory.FillStack(_clickPos - mapCamera.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            mPosMemory.Add(_clickPos - mapCamera.ScreenToWorldPoint(Input.mousePosition));
            Debug.Log(mPosMemory._vectors[0].magnitude);
        }
        else
        {
            mPosMemory.Add(Vector3.zero);
            Debug.Log(mPosMemory._vectors[mPosMemory._vectors.Length - 1].magnitude);
        }

        Debug.Log(mPosMemory._vectors[mPosMemory._vectors.Length - 1].magnitude);
        followTarget += mPosMemory.GetAverageVector();
        //Debug.Log(mPosMemory.GetAverageVector().magnitude);
    }

    private void CamFolllowTarget()
    {
        mapCamera.transform.position = Vector3.Lerp(mapCamera.transform.position, followTarget, moveSpeed);
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

        mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize, minZoom, maxZoom);

    }

    private void LimitCamMovement()
    {
        //get size of camera
        float camHeight = mapCamera.orthographicSize;
        float camWidth = camHeight * mapCamera.aspect;

        //get min/max available values for camera
        float minX = _minMapX + camWidth;
        float maxX = _maxMapX - camWidth;
        float minY = _minMapY + camHeight;
        float maxY = _maxMapY - camHeight;

        //clamping
        Vector3 newCamPos = mapCamera.transform.position;
        newCamPos.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        newCamPos.y = Mathf.Clamp(newCamPos.y, minY, maxY);
        followTarget.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        followTarget.y = Mathf.Clamp(newCamPos.y, minY, maxY);

        mapCamera.transform.position = newCamPos;
    }

    private void ZoomIn()
    {
        mapCamera.orthographicSize -= zoomStep;
    }

    private void ZoomOut()
    {
        mapCamera.orthographicSize += zoomStep;
    }
}

public struct Vector3Memory
{
    public Vector3[] _vectors;

    public Vector3Memory(int memorySize)
    {
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
        for (int i = _vectors.Length - 1; i < 0; i--)
        {
            _vectors[i] = _vectors[i - 1];
        }
        _vectors[0] = vec;
    }

    public void FillStack(Vector3 _vec)
    {
        for (int i = 0; i < _vectors.Length; i++)
        {
            _vectors[i] = _vec;
        }
    }

}
