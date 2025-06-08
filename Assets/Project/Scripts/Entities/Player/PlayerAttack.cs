using System;
using UnityEngine;
using ZuyZuy.PT.Entities.Gun;
using ZuyZuy.PT.Entities.Zombie;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _attackRange = 10f;
        [SerializeField] private float _switchTargetRange = 5f;
        [SerializeField] private LayerMask _zombieLayer;

        [SerializeField] private BaseGun _currentGun;
        private bool _isInAttackState;
        private ZombieController _nearestZombie;

        public bool IsInAttackState => _isInAttackState;
        public ZombieController NearestZombie => _nearestZombie;
        public Action<bool> OnHaveZombieInRange;

        private void Update()
        {
            CheckForZombiesInRange();

            if (_isInAttackState && _nearestZombie != null)
            {
                Attack();
            }
        }

        private void CheckForZombiesInRange()
        {
            Collider[] zombiesInRange = Physics.OverlapSphere(transform.position, _attackRange, _zombieLayer);

            if (zombiesInRange.Length > 0)
            {
                // Find the nearest zombie
                float closestDistance = float.MaxValue;
                ZombieController closestZombie = null;

                foreach (Collider zombieCollider in zombiesInRange)
                {
                    ZombieController zombie = zombieCollider.GetComponent<ZombieController>();
                    if (zombie != null && !zombie.IsDead)  // Only consider alive zombies
                    {
                        float distance = Vector3.Distance(transform.position, zombie.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestZombie = zombie;
                        }
                    }
                }

                // Only switch target if the new target is significantly closer
                if (_nearestZombie != null && !_nearestZombie.IsDead)  // Check if current target is still alive
                {
                    float distanceToCurrentTarget = Vector3.Distance(transform.position, _nearestZombie.transform.position);
                    if (distanceToCurrentTarget - closestDistance < 2f) // Only switch if new target is at least 2 units closer
                    {
                        _nearestZombie = closestZombie;
                    }
                }
                else
                {
                    _nearestZombie = closestZombie;
                }

                _isInAttackState = _nearestZombie != null;  // Only set attack state if we have a valid target
                OnHaveZombieInRange?.Invoke(_isInAttackState);
            }
            else
            {
                _nearestZombie = null;
                _isInAttackState = false;
                OnHaveZombieInRange?.Invoke(false);
            }
        }

        public void Attack()
        {
            if (_currentGun != null)
            {
                _currentGun.TryShoot();
            }
        }

        public void SetCurrentGun(BaseGun gun)
        {
            _currentGun = gun;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
