using System;
using UnityEngine;

namespace Cameras
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform _mainCameraTransform;

        private void Start()
        {
            if (Camera.main != null) _mainCameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            var camRotation = _mainCameraTransform.rotation;
            transform.LookAt(transform.position + camRotation * Vector3.forward, camRotation * Vector3.up);
        }
    }
}
