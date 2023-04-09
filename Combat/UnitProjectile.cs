﻿using System;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float destroyAfterSeconds = 5f;
        [SerializeField] private float launchForce = 10f;

        private void Start()
        {
            rb.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [Server]
        private void DestroySelf() => NetworkServer.Destroy(gameObject);
    }
}