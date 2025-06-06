using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace ZuyZuy.PT.Entities.Zombie
{
    public class ZombieController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _playerTransform;

        [Header("Settings")]
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackCooldown = 1.5f;
        [SerializeField] private int _attackDamage = 10;
        [SerializeField] private float _updatePathInterval = 0.5f;
        [SerializeField] private float _moveSpeed = 3.5f;

        private bool _isAttacking;
        private bool _canAttack = true;
        private bool _isMoving;
        private float _lastPathUpdateTime;
        private static readonly int _isMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int _attackHash = Animator.StringToHash("Attack");

        private void Start()
        {
            if (_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();

            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_playerTransform == null)
                _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (_navMeshAgent != null)
            {
                _navMeshAgent.stoppingDistance = _attackRange;
                _navMeshAgent.speed = _moveSpeed;
                _navMeshAgent.updateRotation = true;
                _navMeshAgent.updateUpAxis = false;
                Debug.Log($"NavMeshAgent initialized - Speed: {_moveSpeed}, Stopping Distance: {_attackRange}");
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
            Debug.Log($"Distance to player: {distanceToPlayer}, Attack Range: {_attackRange}");

            // If within attack range, attack
            if (distanceToPlayer <= _attackRange && _canAttack)
            {
                StartCoroutine(PerformAttack());
            }
            // Otherwise chase the player
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
                Debug.Log($"Setting new destination to player position: {_playerTransform.position}");
            }
        }

        private void UpdateMovementState()
        {
            if (_navMeshAgent == null) return;

            bool shouldBeMoving = !_navMeshAgent.pathPending &&
                                _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance &&
                                !_navMeshAgent.isStopped;

            if (shouldBeMoving != _isMoving)
            {
                _isMoving = shouldBeMoving;
                _animator.SetBool(_isMovingHash, _isMoving);
                Debug.Log($"Movement state changed to: {_isMoving}");
            }
        }

        private IEnumerator PerformAttack()
        {
            _isAttacking = true;
            _canAttack = false;
            _navMeshAgent.isStopped = true;
            _animator.SetBool(_isMovingHash, false);
            _animator.SetTrigger(_attackHash);
            Debug.Log("Starting attack");

            // Wait for attack animation to complete
            yield return new WaitForSeconds(0.5f);

            // Deal damage to player if still in range
            if (Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange)
            {
                // You'll need to implement a way to damage the player
                // For example: _playerTransform.GetComponent<PlayerHealth>()?.TakeDamage(_attackDamage);
                Debug.Log("Player in attack range, would deal damage here");
            }

            _navMeshAgent.isStopped = false;
            _isAttacking = false;
            Debug.Log("Attack finished");

            // Start attack cooldown
            yield return new WaitForSeconds(_attackCooldown);
            _canAttack = true;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}