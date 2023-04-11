using System;
using Combat;
using Mirror;
using Networking;
using UnityEngine;

namespace Buildings
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private int resourcesPerInterval = 5;
        [SerializeField] private float interval = 2;

        private float _timer;
        private RtsPlayer _player;

        public override void OnStartServer()
        {
            _timer = interval;

            _player = connectionToClient.identity.GetComponent<RtsPlayer>();

            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            _timer -= Time.fixedDeltaTime;

            if (!(_timer <= 0)) return;
            
            _player.SetResources(_player.GetResources() + resourcesPerInterval);
                
            _timer = interval;
        }

        private void ServerHandleDie() => NetworkServer.Destroy(gameObject);

        private void ServerHandleGameOver() => enabled = false;
    }
}
