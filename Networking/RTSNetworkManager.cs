using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {

        [Header("Additional Custom Settings")]
        [SerializeField] private GameObject unitSpawnerPrefab; 
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RtsPlayer>();
            
            player.SetTeamColor(new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
                ));
            
            var playerTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab, playerTransform.position, playerTransform.rotation);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (!SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) return;
            
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
