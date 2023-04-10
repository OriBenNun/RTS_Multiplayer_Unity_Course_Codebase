using System;
using Combat;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class PlayerBase : NetworkBehaviour
    {
        [SerializeField] private Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<PlayerBase> ServerOnBaseSpawn; 
        public static event Action<PlayerBase> ServerOnBaseDespawn; 

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            
            ServerOnBaseSpawn?.Invoke(this);
        }
        public override void OnStopServer()
        {
            ServerOnBaseDespawn?.Invoke(this);
            
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        

        #endregion
    }
}
