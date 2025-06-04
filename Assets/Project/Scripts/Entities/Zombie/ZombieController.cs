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
        [SerializeField] private float _chaseRange = 10f;
        [SerializeField] private float _attackCooldown = 1.5f;
        [SerializeField] private int _attackDamage = 10;

        private bool _isAttacking;
        private bool _canAttack = true;
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
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // Check if player is within chase range
            if (distanceToPlayer <= _chaseRange)
            {
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
            }
            else
            {
                // Stop chasing if player is too far
                StopChasing();
            }
        }

        private void ChasePlayer()
        {
            _navMeshAgent.SetDestination(_playerTransform.position);
            _animator.SetBool(_IsMovingHashh, true);
        }

        private void StopChasing()
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetBool(_IsMovingHashh, false);
        }

        private IEnumerator PerformAttack()
        {
            _isAttacking = true;
            _canAttack = false;
            _navMeshAgent.isStopped = true;
            _animator.SetBool(_IsMovingHashh, false);
            _animator.SetTrigger(Attack);

            // Wait for attack animation to complete
            yield return new WaitForSeconds(0.5f);

            // Deal damage to player if still in range
            if (Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange)
            {
                // You'll need to implement a way to damage the player
                // For example: _playerTransform.GetComponent<PlayerHealth>()?.TakeDamage(_attackDamage);
            }

            _navMeshAgent.isStopped = false;
            _isAttacking = false;

            // Start attack cooldown
            yield return new WaitForSeconds(_attackCooldown);
            _canAttack = true;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            // Draw chase range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _chaseRange);
        }
    }
}