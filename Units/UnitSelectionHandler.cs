using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask = new LayerMask();
        
        private Camera _mainCamera;

        private List<Unit> _selectedUnits = new List<Unit>();
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // DeselectAllUnits();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                DeselectAllUnits();
                ClearSelectionArea();
            }
        }

        private void ClearSelectionArea()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit>(out var unit)) { return; }

            if (!unit.hasAuthority) { return; }
            
            _selectedUnits.Add(unit);

            foreach (var selectedUnit in _selectedUnits)
            {
                selectedUnit.Select();
            }
        }

        private void DeselectAllUnits()
        {
            foreach (var selectedUnit in _selectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            _selectedUnits.Clear();
        }
    }
}
