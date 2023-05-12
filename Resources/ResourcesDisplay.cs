using System;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;

namespace Resources
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourcesText;

        private RtsPlayer _player;

        private void Start()
        {
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            
            ClientHandleResourcesUpdated(_player.GetResources());
                    
            _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }

        private void OnDestroy() => _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;

        private void ClientHandleResourcesUpdated(int resources) => resourcesText.text = $"Money: ${resources}";
    }
}
