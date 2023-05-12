using System;
using System.Collections.Generic;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUi;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
        [SerializeField] private string emptySeatDisplayName = "Waiting For Player...";

        private void Start()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerUpdated;
            RtsPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }

        private void OnDestroy()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerUpdated;
            RtsPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }
        
        private void ClientHandleInfoUpdated()
        {
            var players = ((RtsNetworkManager)NetworkManager.singleton).Players;

            for (var i = 0; i < players.Count; i++)
            {
                playerNameTexts[i].text = players[i].GetDisplayName();
            }

            for (var i = players.Count; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = emptySeatDisplayName;
            }

            startGameButton.interactable = players.Count >= 2;
        }
        private void HandleClientConnected() => lobbyUi.SetActive(true);
        
        private void AuthorityHandlePartyOwnerUpdated(bool state) => startGameButton.gameObject.SetActive(state);

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
