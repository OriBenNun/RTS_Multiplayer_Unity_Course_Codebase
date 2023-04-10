using System;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float destroyAfterSeconds = 5f;
        [SerializeField] private float launchForce = 10f;
        [SerializeField] private int damageToDeal = 20;

        private void Start()
        {
            rb.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
            
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<NetworkIdentity>(out var networkIdentity)) return;
            
            if (networkIdentity.connectionToClient == connectionToClient) { return; }

            if (other.TryGetComponent<Health>(out var targetHealth))
            {
                targetHealth.DealDamage(damageToDeal);
            }
            
            DestroySelf();
        }

        [Server]
        private void DestroySelf() => NetworkServer.Destroy(gameObject);

        [Server]
        private void ServerHandleGameOver() => DestroySelf();
    }
}
