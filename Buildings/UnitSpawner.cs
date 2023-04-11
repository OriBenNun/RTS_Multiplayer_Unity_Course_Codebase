using Combat;
using Mirror;
using Networking;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;
        [SerializeField] private TMP_Text remainingUnitsText;
        [SerializeField] private Image unitProgressImage;
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 7f;
        [SerializeField] private float unitSpawnDuration = 5f;

        [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
        private int _queuedUnits;
        [SyncVar]
        private float _unitTimer;

        private float _progressImageVelocity;

        private void Update()
        {
            if (isServer)
            {
                ProduceUnits();
            }

            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server

        public override void OnStartServer() => health.ServerOnDie += ServerHandleDie;

        public override void OnStopServer() => health.ServerOnDie -= ServerHandleDie;
        
        [Server]
        public void ProduceUnits()
        {
            if (_queuedUnits == 0) { return; }

            _unitTimer += Time.deltaTime;

            if (_unitTimer < unitSpawnDuration) { return; }

            var unitSpawnPos = unitSpawnPoint.position;
            var unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPos, unitSpawnPoint.rotation);
            
            NetworkServer.Spawn(unitInstance, connectionToClient);

            var spawnOffset = Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = unitSpawnPos.y;

            unitInstance.GetComponent<UnitMovement>().ServerMove(spawnOffset + unitSpawnPos);

            _queuedUnits--;
            _unitTimer = 0;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (_queuedUnits == maxUnitQueue) { return; }

            var rtsPlayer = connectionToClient.identity.GetComponent<RtsPlayer>();

            var resources = rtsPlayer.GetResources();
            var cost = unitPrefab.GetResourceCost();

            if ( resources < cost) { return; }

            _queuedUnits++;
            
            rtsPlayer.SetResources(resources - cost);
        }

        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }
            
            if (!hasAuthority) { return; }

            CmdSpawnUnit();
        }
        
        [Client]
        private void UpdateTimerDisplay()
        {
            var newProgress = _unitTimer / unitSpawnDuration;

            if (newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress,
                    ref _progressImageVelocity, 0.1f);
            }
        }

        private void ClientHandleQueuedUnitsUpdated(int oldAmount, int newAmount) => remainingUnitsText.text = newAmount.ToString();

        #endregion
    }
}
