using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{

    public static event Action<string> ClientOnGameOver;
    
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

        var winnerId = _bases[0].connectionToClient.connectionId;
        
        RpcGameOver(winnerId.ToString());
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
