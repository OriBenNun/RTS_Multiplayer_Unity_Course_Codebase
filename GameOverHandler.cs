using System;
using System.Collections.Generic;
using Buildings;
using Mirror;

public class GameOverHandler : NetworkBehaviour
{

    public static event Action ServerOnGameOver;
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
        
        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winnerId)
    {
        ClientOnGameOver?.Invoke(winnerId);
    }

    #endregion
}
