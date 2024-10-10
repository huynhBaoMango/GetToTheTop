using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField]
    private float _maximumForce;
    [SerializeField]
    private float _maximumForceTime;
    private float _timeMouseButtonDown;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _timeMouseButtonDown = Time.time;
        }

        if (Input.GetMouseButtonUp(0)) // Ki?m tra khi nút chu?t ???c nh?
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo)) // G?i Raycast ?úng cách
            {
                Zoombie zoombie = hitInfo.collider.GetComponentInParent<Zoombie>();
                if (zoombie != null)
                {
                    float mouseButtonDownDuration = Time.time - _timeMouseButtonDown;
                    float forcePercentage = mouseButtonDownDuration / _maximumForceTime;
                    float forceMagnitude = Mathf.Lerp(1, _maximumForce, forcePercentage);

                    Vector3 forceDirection = (zoombie.transform.position - _camera.transform.position).normalized;

                    Vector3 force = forceMagnitude * forceDirection;

                    // S? d?ng hitInfo.point thay vì hitInfo
                    zoombie.TriggerRagdoll(force, hitInfo.point);
                }
            }
        }
    }
}
