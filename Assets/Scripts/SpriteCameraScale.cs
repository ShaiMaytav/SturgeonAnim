using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCameraScale : MonoBehaviour
{
    [SerializeField] private MapCameraController mapController;
    [SerializeField] private float scaleMultiplier;

    private float _scaleSpeed;
    private Vector3 _originScale;
    private Vector3 _scaleTarget;
    private void Awake()
    {
        mapController.OnZoomChanged.AddListener(ChangeScaleTarget);
        _originScale = transform.localScale;
    }

    private void Start()
    {
        transform.localScale = _scaleTarget;
    }
    private void Update()
    {
        AdjustScale();
    }

    private void ChangeScaleTarget(float zoomTarget, float scaleSpeed)
    {
        _scaleTarget = _originScale * zoomTarget * scaleMultiplier;
        _scaleSpeed = scaleSpeed;
    }

    private void AdjustScale()
    {
        if (transform.localScale != _scaleTarget)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _scaleTarget, _scaleSpeed * Time.deltaTime);
        }
    }
}
