using System;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cameras
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform playerCameraTransform;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float screenBorderThickness = 10f;
        [SerializeField] private Vector2 screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 screenZLimits = Vector2.zero;
        
        private Controls _controls;

        private Vector2 _prevInputs;
        public override void OnStartAuthority()
        {
            playerCameraTransform.gameObject.SetActive(true);

            _controls = new Controls();

            _controls.Player.MoveCamera.performed += SetPreviousInput;
            _controls.Player.MoveCamera.canceled += SetPreviousInput;
            
            _controls.Enable();
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused) { return; }

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            var pos = playerCameraTransform.position;

            if (_prevInputs == Vector2.zero)
            {
                var cursorMovement = Vector3.zero;
                var cursorPos = Mouse.current.position.ReadValue();

                if (cursorPos.y >= Screen.height - screenBorderThickness)
                {
                    cursorMovement.z += 1;
                }
                else if (cursorPos.y <= screenBorderThickness)
                {
                    cursorMovement.z -= 1;
                }
                
                if (cursorPos.x >= Screen.width - screenBorderThickness)
                {
                    cursorMovement.x += 1;
                }
                else if (cursorPos.x <= screenBorderThickness)
                {
                    cursorMovement.x -= 1;
                }

                pos += cursorMovement.normalized * (speed * Time.deltaTime);
            }
            else
            {
                pos += new Vector3(_prevInputs.x, 0f, _prevInputs.y) * (speed * Time.deltaTime);
            }

            pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
            pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

            playerCameraTransform.position = pos;
        }

        private void SetPreviousInput(InputAction.CallbackContext ctx) => _prevInputs = ctx.ReadValue<Vector2>();
    }
}
