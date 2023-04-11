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

        private void Update()
        {
            // TODO remove after adding the lobby, it's just a temporary solution, later it can be moved to Start
            if (_player == null)
            {
                _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();

                if (_player != null)
                {
                    ClientHandleResourcesUpdated(_player.GetResources());
                    
                    _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                }
            }
        }

        private void OnDestroy() => _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;

        private void ClientHandleResourcesUpdated(int resources) => resourcesText.text = $"Money: ${resources}";
    }
}
