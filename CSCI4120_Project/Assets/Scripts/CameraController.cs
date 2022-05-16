using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _height = 5f;
    [SerializeField]
    float _dist = 10f;
    [SerializeField]
    float _angle = 45f;
    [SerializeField]
    float _smoothedSpeed = 0.5f;

    [SerializeField]
    Vector3 _offset = new Vector3(0.0f, 6.0f, -5.0f);

    public GameObject _target;

    private Vector3 _refVel;

    private void Start()
    {
        transform.position = _target.transform.position + _offset;
        transform.LookAt(_target.transform);
    }

    private void LateUpdate()
    {
        if (_target == null || !_target.activeSelf)
            return;
        // HandleCamera();
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Wall");

        if (Physics.Raycast(_target.transform.position, _offset, out hit, _offset.magnitude, mask))
        {
            float dist = (hit.point - _target.transform.position).magnitude * 0.8f;
            transform.position = _target.transform.position + _offset.normalized * dist;
        }
        else
        {
            transform.position = _target.transform.position + _offset;
            transform.LookAt(_target.transform.transform);
        }
    }

    public void SetCameraView(Vector3 offset)
    {
        _offset = offset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        if (_target.transform)
        {
            Vector3 lookAtPos = _target.transform.position;
            lookAtPos.y += 2.0f;
            Gizmos.DrawLine(transform.position, lookAtPos);
            Gizmos.DrawSphere(lookAtPos, 0.25f);
        }
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
