using System;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUi;

        private void Start() => RtsNetworkManager.ClientOnConnected += HandleClientConnected;

        private void OnDestroy() => RtsNetworkManager.ClientOnConnected -= HandleClientConnected;

        private void HandleClientConnected() => lobbyUi.SetActive(true);

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                // The player is Host
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();

                SceneManager.LoadScene(0);
            }
        }
    }
}
