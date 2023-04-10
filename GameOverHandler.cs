using System.Collections;
using System.Collections.Generic;
using Buildings;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{

    private List<PlayerBase> _bases = new List<PlayerBase>();
    
    #region Server

    public override void OnStartServer()
    {
        PlayerBase.ServerOnBaseSpawn += ServerHandleBaseSpawned;
        PlayerBase.ServerOnBaseDespawn += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        PlayerBase.ServerOnBaseSpawn -= ServerHandleBaseSpawned;
        PlayerBase.ServerOnBaseDespawn -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(PlayerBase playerBase)
    {
        _bases.Add(playerBase);
    }
    
    [Server]
    private void ServerHandleBaseDespawned(PlayerBase playerBase)
    {
        _bases.Remove(playerBase);

        if (_bases.Count != 1) { return; }
        
        Debug.Log("Game Over! Only 1 player left!");
    }

    #endregion

    #region Client

    

    #endregion
}
