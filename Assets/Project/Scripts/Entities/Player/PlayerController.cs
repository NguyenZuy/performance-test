using UnityEngine;
using ZuyZuy.Workspace;
using ZuyZuy.Workspace.MobileController;
using ZuyZuy.PT.Manager;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerController : BaseSingleton<PlayerController>
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private PlayerAttack _playerAttack;

        [SerializeField] private UniversalButton _inputMove;

        private Transform _spawnPoint;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

#if UNITY_EDITOR
        [SerializeField] private bool _forceUseMobileInput = false;
#endif

        public PlayerAttack PlayerAttack => _playerAttack;
        public PlayerAnimation PlayerAnimation => _playerAnimation;

        private Vector3 moveDirection;

        void Start()
        {
            _playerMovement.OnMove += _playerAnimation.SetMotion;
            _playerAttack.OnHaveZombieInRange += _playerAnimation.SetGunShoot;

            // Store initial position and rotation
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        public void SetSpawnPoint(Transform spawnPoint)
        {
            if (spawnPoint == null)
            {
                Debug.LogWarning("Attempting to set null spawn point!");
                return;
            }
            _spawnPoint = spawnPoint;
        }

        public void Respawn()
        {
            Debug.Log($"[PlayerController] Before Respawn - Current Position: {transform.position}, Current Rotation: {transform.rotation}");
            Debug.Log($"[PlayerController] Spawn Point: {(_spawnPoint != null ? _spawnPoint.position.ToString() : "null")}");
            Debug.Log($"[PlayerController] Initial Position: {_initialPosition}, Initial Rotation: {_initialRotation}");

            // Reset position and rotation
            if (_spawnPoint != null)
            {
                transform.position = _spawnPoint.position;
                transform.rotation = _spawnPoint.rotation;
                Debug.Log($"[PlayerController] Respawned at Spawn Point - New Position: {transform.position}, New Rotation: {transform.rotation}");
            }
            else
            {
                // Fallback to initial position if no spawn point is set
                transform.position = _initialPosition;
                transform.rotation = _initialRotation;
                Debug.Log($"[PlayerController] Respawned at Initial Position - New Position: {transform.position}, New Rotation: {transform.rotation}");
            }

            // Reset movement direction and character controller state
            moveDirection = Vector3.zero;
            if (_playerMovement != null)
            {
                _playerMovement.ResetState();
            }
        }

        private void Update()
        {
            // Don't handle movement if player is dead
            if (GameManager.Instance != null && GameManager.Instance.CurPlayerHP <= 0)
            {
                moveDirection = Vector3.zero;
                return;
            }

            HandleInput();
            _playerMovement.HandleMovement(moveDirection);
            _playerMovement.HandleRotation(moveDirection);
        }

        public void SwitchGun(int index)
        {
            if (_playerAttack != null)
            {
                var newGun = _playerAttack.SwitchGun(index);
                if (newGun != null && _playerAnimation != null)
                {
                    _playerAnimation.ChangeGun(newGun);
                }
            }
        }

        private void HandleInput()
        {
#if UNITY_EDITOR
            if (!_forceUseMobileInput)
            {
                // PC input for editor testing
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                float verticalInput = Input.GetAxisRaw("Vertical");
                moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            }
            else
            {
                // Mobile input
                if (_inputMove.isFingerDown)
                {
                    Vector3 input = _inputMove.direction;
                    moveDirection = new Vector3(input.x, 0, input.y).normalized;
                }
                else
                    moveDirection = Vector3.zero;
            }
#else
            if (_inputMove.isFingerDown)
            {
                Vector3 input = _inputMove.direction;
                moveDirection = new Vector3(input.x, 0, input.y).normalized;
            }
            else
                moveDirection = Vector3.zero;
#endif
        }
    }
}