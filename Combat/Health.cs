using System;
using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;


        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated;

        #region Server

        public override void OnStartServer()
        {
            _currentHealth = maxHealth;

            PlayerBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            PlayerBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (_currentHealth == 0) { return; }
            
            _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

            if (_currentHealth != 0) { return; }
            
            ServerOnDie?.Invoke();
        }

        [Server]
        private void ServerHandlePlayerDie(int connectionId)
        {
            if (connectionToClient.connectionId != connectionId) { return; }
            
            DealDamage(_currentHealth); // Kill the unit if this player is dead
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }
        
        #endregion

    }
}
