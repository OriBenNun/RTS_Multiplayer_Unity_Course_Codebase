using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {

        [FormerlySerializedAs("unitSpawnerPrefab")]
        [Header("Additional Custom Settings")]
        [SerializeField] private GameObject playerBasePrefab; 
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        private bool _isGameInProgress;

        private const string GameSceneName = "Scene_Map_01";
        
        public List<RtsPlayer> Players { get; } = new List<RtsPlayer>();

        #region Server

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (!_isGameInProgress) { return; }
            
            conn.Disconnect();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            var player = conn.identity.GetComponent<RtsPlayer>();

            Players.Remove(player);
            
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();

            _isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2) { return; }

            _isGameInProgress = true;
            
            ServerChangeScene(GameSceneName);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RtsPlayer>();
            
            Players.Add(player);

            player.SetDisplayName($"Player {Players.Count}");

            player.SetResources(player.GetStartingResources());

            player.SetTeamColor(new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            ));
            
            player.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name != GameSceneName) return;
            
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (var player in Players)
            {
                var playerBaseInstance = Instantiate(playerBasePrefab, GetStartPosition().position, Quaternion.identity);
                
                NetworkServer.Spawn(playerBaseInstance, player.connectionToClient);
            }
        }

        #endregion

        #region Client

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            
            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            
            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient() => Players.Clear();

        #endregion
    }
}
