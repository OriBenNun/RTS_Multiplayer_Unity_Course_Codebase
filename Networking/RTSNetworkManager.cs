using Mirror;
using UnityEngine;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {

        [Header("Additional Custom Settings")]
        [SerializeField] private GameObject unitSpawnerPrefab;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var playerTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab, playerTransform.position, playerTransform.rotation);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}
