using System;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private GameObject buildingPreview;
        [SerializeField] private Sprite icon;
        [SerializeField] private int price = 100;
        [SerializeField] private int id = -1;
        
        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingDespawned;

        public static event Action<Building> AuthorityOnBuildingSpawned;
        public static event Action<Building> AuthorityOnBuildingDespawned;

        public GameObject GetBuildingPreview() => buildingPreview;
        public Sprite GetIcon() => icon;
        public int GetId() => id;
        public int GetPrice() => price;

        #region Server

        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartAuthority() => AuthorityOnBuildingSpawned?.Invoke(this);

        public override void OnStopClient()
        {
            if (!hasAuthority) { return; } // `!isClientOnly` Prevents the Host from running to avoid duplicated lists
            
            AuthorityOnBuildingDespawned?.Invoke(this);
        }

        #endregion
    }
}
