using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCameraScale : MonoBehaviour
{
    [SerializeField] private MapCameraController mapController;
    [SerializeField] private float scaleMultiplier;

    private Vector3 _originScale;
    private Vector3 _scaleTarget;
    private void Awake()
    {
        mapController.OnZoomChanged.AddListener(ChangeScaleTarget);
        _originScale = transform.localScale;
    }

    private void Start()
    {
        ChangeScaleTarget();
        transform.localScale = _scaleTarget;
    }
    private void Update()
    {
        AdjustScale();
    }

    private void ChangeScaleTarget()
    {
        _scaleTarget = _originScale * mapController._zoomTarget * scaleMultiplier;
    }

    private void AdjustScale()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _scaleTarget, mapController.zoomSmoothSpeed * Time.deltaTime);
    }
}
