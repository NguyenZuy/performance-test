using UnityEngine;
using UnityEngine.AI;

public class TestZom : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _target;

    [Header("Movement Settings")]
    [SerializeField] private float _updatePathInterval = 0.5f;
    [SerializeField] private float _stoppingDistance = 1.5f;

    private float _lastPathUpdateTime;
    private bool _isMoving;

    private void Start()
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        if (_navMeshAgent != null)
        {
            _navMeshAgent.stoppingDistance = _stoppingDistance;
        }
        else
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }

    private void Update()
    {
        if (_navMeshAgent == null || _target == null) return;

        // Update path periodically to avoid constant recalculations
        if (Time.time >= _lastPathUpdateTime + _updatePathInterval)
        {
            _lastPathUpdateTime = Time.time;
            _navMeshAgent.SetDestination(_target.position);
        }

        // Check if we've reached the destination
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (_isMoving)
            {
                _isMoving = false;
                OnReachedDestination();
            }
        }
        else if (!_isMoving)
        {
            _isMoving = true;
        }
    }

    private void OnReachedDestination()
    {
        // You can add custom behavior here when the zombie reaches its target
        Debug.Log("Reached destination!");
    }
}
