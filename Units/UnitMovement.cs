﻿using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Targeter targeter;
        [SerializeField] private float chaseTargetRange = 10f;
        
        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            if (targeter.GetHasTarget())
            {
                var target = targeter.GetTarget();
                if ((target.transform.position - transform.position).sqrMagnitude > chaseTargetRange * chaseTargetRange) // To avoid using Vector3.Distance, which uses sqrRoot which is very expensive
                {
                    agent.SetDestination(target.transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }
                return;
            }
            
            if (!agent.hasPath) { return; }
            if (agent.remainingDistance > agent.stoppingDistance) { return; }
            
            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position) => ServerMove(position);

        [Server]
        public void ServerMove(Vector3 position)
        {
            targeter.ClearTarget();
            
            if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas)) { return; }
        
            agent.SetDestination(hit.position);
        }

        [Server]
        private void ServerHandleGameOver() => agent.ResetPath();

        #endregion

    }
}
