using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using ZuyZuy.PT.SOs;
using ZuyZuy.PT.Manager;

namespace ZuyZuy.PT.Entities.Zombie
{
    [RequireComponent(typeof(Zombie))]
    public class ZombieController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _playerTransform;

        private Zombie _zombie;
        public bool IsDead { get; private set; }

        private float _attackRange;
        private float _moveSpeed;
        private float _attackCooldown;
        private float _updatePathInterval = 0.5f;

        private bool _isAttacking;
        private bool _canAttack = true;
        private bool _isMoving;
        private float _lastPathUpdateTime;
        private static readonly int _isMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _dieHash = Animator.StringToHash("Die");

        private void Start()
        {
            if (_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();

            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_playerTransform == null)
                _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            _zombie = GetComponent<Zombie>();
            if (_zombie != null && _zombie.ZombieData != null)
            {
                _moveSpeed = _zombie.ZombieData.Speed;
                _attackRange = _zombie.ZombieData.AttackRange;
                _attackCooldown = 1f / _zombie.ZombieData.AttackSpeed;
            }
            else
            {
                Debug.LogError("Zombie component or ZombieSO data is missing!");
            }

            if (_navMeshAgent != null)
            {
                _navMeshAgent.stoppingDistance = _attackRange;
                _navMeshAgent.speed = _moveSpeed;
                _navMeshAgent.updateRotation = true;
                _navMeshAgent.updateUpAxis = false;
            }
            else
            {
                Debug.LogError("NavMeshAgent component is missing!");
            }

            if (_playerTransform == null)
            {
                Debug.LogError("Player transform not found! Make sure the player has the 'Player' tag.");
            }
        }

        private void Update()
        {
            if (_playerTransform == null || _navMeshAgent == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // If within attack range, attack
            if (distanceToPlayer <= _attackRange && _canAttack)
            {
                StartCoroutine(PerformAttack());
            }
            // Otherwise chase the player only if not attacking
            else if (!_isAttacking)
            {
                ChasePlayer();
            }

            // Update movement state
            UpdateMovementState();
        }

        private void ChasePlayer()
        {
            // Update path periodically to avoid constant recalculations
            if (Time.time >= _lastPathUpdateTime + _updatePathInterval)
            {
                _lastPathUpdateTime = Time.time;
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_playerTransform.position);
            }
        }

        private void UpdateMovementState()
        {
            if (_navMeshAgent == null) return;

            bool shouldBeMoving = !_isAttacking &&
                                 !_navMeshAgent.pathPending &&
                                 _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance &&
                                 !_navMeshAgent.isStopped;

            if (shouldBeMoving != _isMoving)
            {
                _isMoving = shouldBeMoving;
                _animator.SetBool(_isMovingHash, _isMoving);
            }
        }

        private void FacePlayer()
        {
            if (_playerTransform == null) return;
            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            direction.y = 0; // Keep rotation only on the horizontal plane
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }

        private IEnumerator PerformAttack()
        {
            _isAttacking = true;
            _canAttack = false;
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero; // Ensure immediate stop
            _animator.SetBool(_isMovingHash, false);

            // Face the player before attacking
            FacePlayer();
            _animator.SetTrigger(_attackHash);

            // Wait for the attack animation to start
            yield return new WaitForSeconds(0.1f);

            // Get the current attack animation state
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float attackDuration = stateInfo.length;

            // Keep the zombie stopped and facing the player during the entire attack animation
            float elapsedTime = 0f;
            while (elapsedTime < attackDuration)
            {
                FacePlayer();
                _navMeshAgent.isStopped = true;
                _navMeshAgent.velocity = Vector3.zero;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure we're still stopped after the animation
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;

            // Deal damage to player if still in range
            if (Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange)
            {
                GameManager.Instance.TakeDamage((int)_zombie.ZombieData.AttackDamage);
            }

            _isAttacking = false;

            // Start attack cooldown
            yield return new WaitForSeconds(_attackCooldown);
            _canAttack = true;
        }

        public void BeAttacked(int damage)
        {
            if (_zombie != null)
            {
                _zombie.TakeDamage(damage);
            }
        }

        public void OnZombieDeath()
        {
            IsDead = true;
            StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            // Stop all movement and attacks
            _isAttacking = true;
            _canAttack = false;
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.velocity = Vector3.zero;
            }

            // Trigger death animation
            _animator.SetTrigger(_dieHash);

            // Wait for death animation to complete
            yield return new WaitForSeconds(4.6f);

            // Disable the game object
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}