using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Mirror;
using Units;
using UnityEngine;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private float buildingRangeLimit = 5f;
        [SerializeField] private Building[] buildings = Array.Empty<Building>();
        
        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int _resources = 250;

        public event Action<int> ClientOnResourcesUpdated;

        private Color _teamColor;
        private List<Unit> _myUnits = new List<Unit>();
        private List<Building> _myBuildings = new List<Building>();

        public Color GetTeamColor() => _teamColor;
        public List<Unit> GetPlayerUnits() => _myUnits;
        public List<Building> GetPlayerBuildings() => _myBuildings;
        public int GetResources() => _resources;

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 pointToPlace)
        {
            // Check for collision with blocking layers
            if (Physics.CheckBox(
                    pointToPlace + buildingCollider.center,
                    buildingCollider.size / 2,
                    Quaternion.identity,
                    buildingBlockLayer))
            {
                return false;
            }

            // Check for in within range of other player's buildings
            return _myBuildings.Any(building => (pointToPlace - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit);
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }
        
        [Server]
        public void SetResources(int newResources) => _resources = newResources;
        
        [Server]
        public void SetTeamColor(Color newColor) => _teamColor = newColor;

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
        {
            var buildingToPlace = buildings.FirstOrDefault(building => building.GetId() == buildingId);

            if (buildingToPlace == null) { return; }

            var price = buildingToPlace.GetPrice();
            if (_resources < price) { return; }

            var buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
            
            if (!CanPlaceBuilding(buildingCollider, point)) { return; }

            var buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
            
            NetworkServer.Spawn(buildingInstance, connectionToClient);
            
            SetResources(_resources - price);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            
            _myUnits.Add(unit);
        }
        
        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            
            _myUnits.Remove(unit);
        }
        
        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            
            _myBuildings.Add(building);
        }
        
        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            
            _myBuildings.Remove(building);
        }
        
        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) { return; }
            
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            
            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) { return; }
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            
            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }
        
        private void AuthorityHandleUnitSpawned(Unit unit) => _myUnits.Add(unit);

        private void AuthorityHandleUnitDespawned(Unit unit) => _myUnits.Remove(unit);
        
        private void AuthorityHandleBuildingSpawned(Building building) => _myBuildings.Add(building);

        private void AuthorityHandleBuildingDespawned(Building building) => _myBuildings.Remove(building);

        private void ClientHandleResourcesUpdated(int oldResources, int newResources) => ClientOnResourcesUpdated?.Invoke(newResources);

        #endregion

    }
}
