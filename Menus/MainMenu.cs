using Mirror;
using Steamworks;
using UnityEngine;

namespace Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPanel;
        [SerializeField] private bool useSteam = false;

        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> LobbyEntered;

        private const string PchKey = "HostAddress";

        private void Start()
        {
            if (!useSteam) { return; }

            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
            landingPanel.SetActive(false);

            if (useSteam)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
                return;
            }
            
            NetworkManager.singleton.StartHost();
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                landingPanel.SetActive(true);
                return;
            }
            
            NetworkManager.singleton.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), PchKey,
                SteamUser.GetSteamID().ToString());
        }
        
        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }
        
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active) { return; }

            var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), PchKey);

            NetworkManager.singleton.networkAddress = hostAddress;
            
            NetworkManager.singleton.StartClient();
            
            landingPanel.SetActive(false);
        }

        public void QuitGame() => Application.Quit();
    }
}
