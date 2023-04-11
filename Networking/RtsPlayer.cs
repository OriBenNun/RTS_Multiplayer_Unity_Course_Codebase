using System.Collections.Generic;
using Buildings;
using Mirror;
using Units;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        private List<Unit> _myUnits = new List<Unit>();
        private List<Building> _myBuildings = new List<Building>();

        public List<Unit> GetPlayerUnits() => _myUnits;
        public List<Building> GetPlayerBuildings() => _myBuildings;

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

        #endregion
    }
}
