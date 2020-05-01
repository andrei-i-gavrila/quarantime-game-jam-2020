using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class TopCamera : MonoBehaviour
{
    public float minPanSpeed = 10f; // Starting panning speed
    public float maxPanSpeed = 100f; // Max panning speed
    public float panTimeConstant = 3f; // Time to reach max panning speed

    // Mouse right-down rotation
    public float rotateSpeed = 10; // mouse down rotation speed about x and y axes
    
    public float minZoomDistance = 10f;
    public float maxZoomDistance = 50f;
    public float zoomSpeed = 2;
    
    [Range(0, 1)] public float scrollEdge = 0.15f;
    
    private float _panT = 0;
    private float _panSpeed = 10;

    private Camera _camera;
    private float _currentZoom = 0f;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        var panTranslation = Vector3.zero;
        if (Input.mousePosition.x < Screen.width * scrollEdge) panTranslation += Vector3.left;
        if (Input.mousePosition.x > Screen.width * (1 - scrollEdge)) panTranslation += Vector3.right;

        
        if (Input.mousePosition.y < Screen.height * scrollEdge) panTranslation += Vector3.back;
        if (Input.mousePosition.y > Screen.height * (1 - scrollEdge)) panTranslation += Vector3.forward;
        

        if (panTranslation != Vector3.zero)
        {
            transform.position += panTranslation * (_panSpeed * Time.deltaTime);
            
            _panT += Time.deltaTime / panTimeConstant;
            _panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, _panT * _panT);
        }
        else
        {
            _panT = 0;
            _panSpeed = minPanSpeed;
        }

        if (Input.GetMouseButton(3))
        {
            
        }

        var zoom = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Math.Abs(zoom) > Mathf.Epsilon)
        {
            var zoomTranslation = Vector3.forward * (zoom * zoomSpeed);
            var hit = GetRaycastHit();
            if (hit.HasValue)
            {

                if (zoomTranslation.z + hit.Value.distance > maxZoomDistance && zoomTranslation.z < 0f)
                {
                    zoomTranslation.z = 0f;
                }
                if (zoomTranslation.z + hit.Value.distance < minZoomDistance && zoomTranslation.z > 0f)
                {
                    zoomTranslation.z = 0f;
                }
                transform.Translate(zoomTranslation, Space.Self);
                _camera.fieldOfView += zoomTranslation.z;
            }
        }

    }

    private RaycastHit? GetRaycastHit()
    {
        var ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out var hit))
            return hit;
        
        return null;

    }
}