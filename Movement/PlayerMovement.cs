﻿using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private Camera _mainCamera;

    #region Server

    [Command]
    private void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas)) { return; }
        
        agent.SetDestination(hit.position);
    }
    
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        _mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (!Input.GetMouseButtonDown(1)) { return; }
        
        // So the player clicked

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) { return; }
        
        // So found a desired hit
        
        CmdMove(hit.point);
    }

    #endregion
    
    
}
