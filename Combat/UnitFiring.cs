using Mirror;
using UnityEngine;

namespace Combat
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private float fireRate = 3f;
        [SerializeField] private float rotationSpeed = 20f;

        private float _lastFireTime;

        [ServerCallback]
        private void Update()
        {
            if (!targeter.GetHasTarget()) { return; }

            var target = targeter.GetTarget();
            
            if (!CanFireAtTarget(target)) { return; }

            var targetRotation =
                Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / fireRate) + _lastFireTime)
            {
                var spawnPosition = projectileSpawnPoint.position;
                var projectileRotation =
                    Quaternion.LookRotation(target.GetAimAtPoint().position -
                                            spawnPosition);

                var projectileInstance =
                    Instantiate(projectilePrefab, spawnPosition, projectileRotation);
                
                NetworkServer.Spawn(projectileInstance, connectionToClient);

                _lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget(Component target)
        {
            Debug.Log((target.transform.position - transform.position).sqrMagnitude <=
                      fireRange * fireRange);
            
            return (target.transform.position - transform.position).sqrMagnitude <=
                   fireRange * fireRange; // To avoid using Vector3.Distance, which uses sqrRoot which is very expensive

        }
    }
}
