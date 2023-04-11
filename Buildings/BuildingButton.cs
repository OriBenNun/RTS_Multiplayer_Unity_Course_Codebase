using Mirror;
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
        
        private bool _isPlayerNull;
        private bool _isBuildingPreviewInstanceNull = true;
        
        private void Start()
        {
            _isPlayerNull = _player == null;
            _mainCamera = Camera.main;

            iconImage.sprite = building.GetIcon();
            priceText.text = $"${building.GetPrice()}";
        }

        private void Update()
        {
            // TODO remove after adding the lobby, it's just a temporary solution, later it can be moved to Start
            if (_isPlayerNull)
            {
                _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
                _isPlayerNull = false;
            }
            
            if (_isBuildingPreviewInstanceNull) {return;}

            UpdateBuildingPreview();

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

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
        }
    }
}
