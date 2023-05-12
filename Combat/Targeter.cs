using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable _target;
        private bool _hasTarget = false;

        public bool GetHasTarget() => _hasTarget;

        public Targetable GetTarget() => _target;

        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out var target)) { return; }

            _hasTarget = true;
            _target = target;
        }

        [Server]
        public void ClearTarget()
        {
            _hasTarget = false;
            _target = null;
        }

        [Server]
        private void ServerHandleGameOver() => ClearTarget();

        #endregion

    }
}
