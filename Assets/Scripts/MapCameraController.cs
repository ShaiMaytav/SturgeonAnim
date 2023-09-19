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

    private void MoveTarget()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickPos = mapCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 diff = _clickPos - mapCamera.ScreenToWorldPoint(Input.mousePosition);

            followTarget += diff;
        }
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
