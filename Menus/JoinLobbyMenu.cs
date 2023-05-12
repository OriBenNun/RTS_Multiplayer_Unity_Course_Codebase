using System;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPanel;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private Button joinButton;

        private void OnEnable()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby()
        {
            NetworkManager.singleton.networkAddress = addressInput.text;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            
            gameObject.SetActive(false);
            landingPanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
        }
    }
}
