using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;
        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

        public UnitMovement GetUnitMovement() { return unitMovement; }

        #region Server

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
        }

        #endregion
        
        #region Client

        public override void OnStartClient()
        {
            if (!isClientOnly || !hasAuthority) { return; } // `!isClientOnly` Prevents the Host from running to avoid duplicated lists
            
            AuthorityOnUnitSpawned?.Invoke(this);
        }
        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) { return; } // `!isClientOnly` Prevents the Host from running to avoid duplicated lists
            
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) { return; }
            
            onSelected?.Invoke();
        }
        
        [Client]
        public void Deselect()
        {
            if (!hasAuthority) { return; }
            
            onDeselected?.Invoke();
        }
        #endregion
        
    }
}
