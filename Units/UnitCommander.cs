﻿using Combat;
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
            
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) { return; }

            if (hit.collider.TryGetComponent<Targetable>(out var target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }
                
                TryTarget(target);
                return;
            }
            
            TryMove(hit.point);
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void TryMove(Vector3 hitPoint)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(hitPoint);
            }
        }
        
        private void TryTarget(Targetable target)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitTargeter().CmdSetTarget(target.gameObject);
            }
        }

        private void ClientHandleGameOver(string winnerId)
        {
            enabled = false;
        }
    }
}
