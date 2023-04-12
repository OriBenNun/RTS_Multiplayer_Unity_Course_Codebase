using System;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUi;
        [SerializeField] private Button startGameButton;

        private void Start()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerUpdated;
        }

        private void OnDestroy()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerUpdated;
        }

        private void HandleClientConnected() => lobbyUi.SetActive(true);
        
        private void AuthorityHandlePartyOwnerUpdated(bool state) => startGameButton.interactable = state;

        public void StartGame() => NetworkClient.connection.identity.GetComponent<RtsPlayer>().CmdStartGame();

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
