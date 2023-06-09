﻿using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Building building;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private LayerMask floorMask;

        private Camera _mainCamera;
        private RtsPlayer _player;
        private GameObject _buildingPreviewInstance;
        private Renderer _buildingRendererInstance;
        private BoxCollider _buildingCollider;
        
        private bool _isBuildingPreviewInstanceNull = true;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void Start()
        {
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            
            _mainCamera = Camera.main;

            iconImage.sprite = building.GetIcon();
            priceText.text = $"${building.GetPrice()}";

            _buildingCollider = building.GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (_isBuildingPreviewInstanceNull) {return;}

            UpdateBuildingPreview();

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            if (_player.GetResources() < building.GetPrice()) { return; }
            
            _buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
            _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();

            _isBuildingPreviewInstanceNull = false;
            
            _buildingPreviewInstance.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isBuildingPreviewInstanceNull) { return; }

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, floorMask))
            {
                _player.CmdTryPlaceBuilding(building.GetId(), hit.point);
            }

            _isBuildingPreviewInstanceNull = true;
            Destroy(_buildingPreviewInstance);
        }
        
        private void UpdateBuildingPreview()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, floorMask)) { return; }

            _buildingPreviewInstance.transform.position = hit.point;

            if (!_buildingPreviewInstance.activeSelf)
            {
                _buildingPreviewInstance.SetActive(true);
            }

            var color = _player.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.red;
            
            _buildingRendererInstance.material.SetColor(BaseColor, color);
            
            if (_buildingRendererInstance.materials.Length > 1)
            {
                _buildingRendererInstance.materials[1].SetColor(BaseColor, color);
            }
        }
    }
}
