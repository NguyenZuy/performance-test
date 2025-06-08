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

        [SerializeField] private BaseGun[] _guns;
        [SerializeField] private BaseGun _currentGun;

        private bool _isInAttackState;
        private ZombieController _nearestZombie;

        public bool IsInAttackState => _isInAttackState;
        public ZombieController NearestZombie => _nearestZombie;
        public Action<bool> OnHaveZombieInRange;

        private void Start()
        {
            // Initialize with the first gun if available
            if (_guns != null && _guns.Length > 0)
            {
                SetCurrentGun(_guns[0]);
            }
        }

        public BaseGun SwitchGun(int index)
        {
            if (_guns == null || index < 0 || index >= _guns.Length)
                return null;

            // Deactivate current gun if it exists
            if (_currentGun != null)
            {
                _currentGun.gameObject.SetActive(false);
            }

            // Set and activate new gun
            SetCurrentGun(_guns[index]);
            if (_currentGun != null)
            {
                _currentGun.gameObject.SetActive(true);
            }

            return _currentGun;
        }

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

                // Only switch target if current target is dead or outside switch range
                if (_nearestZombie != null)
                {
                    float distanceToCurrentTarget = Vector3.Distance(transform.position, _nearestZombie.transform.position);
                    if (_nearestZombie.IsDead || distanceToCurrentTarget > _switchTargetRange)
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
