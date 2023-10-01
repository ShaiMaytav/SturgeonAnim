using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspCamController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapSprite;
    [SerializeField] private Camera mapCamera;
    [SerializeField][Range(0, 1)] private float slideDrag;
    [SerializeField] private float slideForce;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float zoomStep;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSmoothSpeed;
    [SerializeField] private int memorySize;

    private Vector3Memory _mPosMemory;
    private Vector3 _camTarget;
    private Vector3 _clickPos;
    private float _zoomTarget;
    private float _minMapX, _minMapY, _maxMapX, _maxMapY;


    private void Awake()
    {
        //get end points of map
        _minMapX = mapSprite.transform.position.x - mapSprite.bounds.size.x / 2;
        _minMapY = mapSprite.transform.position.y - mapSprite.bounds.size.y / 2;
        _maxMapX = mapSprite.transform.position.x + mapSprite.bounds.size.x / 2;
        _maxMapY = mapSprite.transform.position.y + mapSprite.bounds.size.y / 2;
        _camTarget = mapCamera.transform.position;

        _mPosMemory = new Vector3Memory(memorySize);
        _zoomTarget = mapCamera.fieldOfView;
    }


    private void Update()
    {
        //PanCamera();
        MoveTarget();
        Smooth();
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
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 diff = _clickPos - mapCamera.ScreenToWorldPoint(Input.mousePosition);
            //_mPosMemory.Add(diff);
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
        mapCamera.fieldOfView = Mathf.Lerp(mapCamera.fieldOfView, _zoomTarget, zoomSmoothSpeed * Time.deltaTime);

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
        float camHeight = GetCamHeight();
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
        _camTarget.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        _camTarget.y = Mathf.Clamp(newCamPos.y, minY, maxY);

        mapCamera.transform.position = newCamPos;
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

    private float GetCamHeight()
    {
        float angle = Mathf.Tan(Mathf.Deg2Rad * (mapCamera.fieldOfView / 2));
        float adj = Mathf.Abs(mapCamera.transform.position.z - mapSprite.transform.position.z);
        float opp = angle * adj;
        return opp;
    }
}
