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
        [SerializeField] private LayerMask _zombieLayer;

        private BaseGun _currentGun;
        private bool _isInAttackState;
        private ZombieController _nearestZombie;

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
                    if (zombie != null)
                    {
                        float distance = Vector3.Distance(transform.position, zombie.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestZombie = zombie;
                        }
                    }
                }

                _nearestZombie = closestZombie;
                _isInAttackState = true;
                OnHaveZombieInRange?.Invoke(true);
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
