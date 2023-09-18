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

    private Vector3 _clickPos;
    private float _minX, _minY, _maxX, _maxY;


    private void Awake()
    {
        _minX = transform.position.x - mapSprite.bounds.size.x / 2;
        _minY = transform.position.y - mapSprite.bounds.size.y / 2;
        _maxX = transform.position.x + mapSprite.bounds.size.x / 2;
        _maxY = transform.position.y + mapSprite.bounds.size.y / 2;
    }


    private void Update()
    {
        PanCamera();
        Zoom();
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

        mapCamera.transform.position = Mathf.Clamp(mapCamera.transform.position, );
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
