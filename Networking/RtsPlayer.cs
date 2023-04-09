using System.Collections.Generic;
using Mirror;
using Units;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        private List<Unit> _myUnits = new List<Unit>();

        public List<Unit> GetPlayerUnits() => _myUnits;

        #region Server
        
        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
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
        
        #endregion

        #region Client

        public override void OnStartClient()
        {
            if (!isClientOnly) { return; }
            
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly) { return; }
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }
        
        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            if (!hasAuthority) { return; }
            
            _myUnits.Add(unit);
        }
        
        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            if (!hasAuthority) { return; }
            
            _myUnits.Remove(unit);
        }

        #endregion
    }
}
