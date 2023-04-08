using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private RectTransform unitSelectionArea;

        private Camera _mainCamera;
        private RtsPlayer _player;

        private Vector2 _startPosition;
        private bool _isPlayerNull;

        public List<Unit> SelectedUnits { get; } = new List<Unit>();
        
        private void Start()
        {
            _isPlayerNull = _player == null;
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            // TODO remove after adding the lobby, it's just a temporary solution, later it can be moved to Start
            if (_isPlayerNull)
            {
                _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
                _isPlayerNull = false;
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
            {
                DeselectUnits();
            }

            unitSelectionArea.gameObject.SetActive(true);

            _startPosition = Mouse.current.position.ReadValue();
            
            UpdateSelectionArea();
        }

        private void UpdateSelectionArea()
        {
            var mousePos = Mouse.current.position.ReadValue();

            var areaWidth = mousePos.x - _startPosition.x;
            var areaHeight = mousePos.y - _startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

        private void ClearSelectionArea()
        {
            var isDeselecting = Keyboard.current.leftCtrlKey.isPressed;
            
            unitSelectionArea.gameObject.SetActive(false);

            // Clicked but didn't drag. Equals to single unit selection
            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) { return; }

                if (!hit.collider.TryGetComponent<Unit>(out var unit)) { return; }

                if (!unit.hasAuthority) { return; }

                if (isDeselecting)
                {
                    DeselectUnits(unit);
                    return;
                }
            
                SelectedUnits.Add(unit);

                foreach (var selectedUnit in SelectedUnits)
                {
                    selectedUnit.Select();
                }

                return;
            }

            var min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            var max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

            var playerUnits = _player.GetPlayerUnits();
            
            foreach (var unit in playerUnits)
            {
                if (SelectedUnits.Contains(unit) && !isDeselecting) { continue; }

                var unitScreenPos = _mainCamera.WorldToScreenPoint(unit.transform.position);

                var inSelectionBox = unitScreenPos.x > min.x &&
                                     unitScreenPos.x < max.x &&
                                     unitScreenPos.y > min.y &&
                                     unitScreenPos.y < max.y;

                switch (inSelectionBox)
                {
                    case true when isDeselecting:
                        SelectedUnits.Remove(unit);
                        unit.Deselect();
                        break;
                    case true:
                        SelectedUnits.Add(unit);
                        unit.Select();
                        break;
                }
            }

        }

        private void DeselectUnits()
        {
            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            
            SelectedUnits.Clear();
        }
        
        private void DeselectUnits(Unit unit)
        {
            unit.Deselect();
            SelectedUnits.Remove(unit);
        }
        
        // private void DeselectUnits(List<Unit> units)
        // {
        //     foreach (var selectedUnit in units)
        //     {
        //         selectedUnit.Deselect();
        //         SelectedUnits.Remove(selectedUnit);
        //     }
        // }
    }
}
