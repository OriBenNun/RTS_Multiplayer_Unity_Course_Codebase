using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;

        public UnitMovement GetUnitMovement() { return unitMovement; }
        
        #region Client
        
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
