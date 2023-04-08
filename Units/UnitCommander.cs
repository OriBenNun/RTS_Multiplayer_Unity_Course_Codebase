using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitCommander : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask;

        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) { return; }

            TryMove(hit.point);
        }

        private void TryMove(Vector3 hitPoint)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(hitPoint);
            }
        }
    }
}
