using UnityEngine;
using ZuyZuy.Workspace.MobileController;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private PlayerAttack _playerAttack;

        [SerializeField] private UniversalButton _inputMove;

#if UNITY_EDITOR
        [SerializeField] private bool _forceUseMobileInput = false;
#endif

        private Vector3 moveDirection;

        void Start()
        {
            _playerMovement.OnMove += _playerAnimation.SetMotion;
            _playerAttack.OnHaveZombieInRange += _playerAnimation.SetGunShoot;
        }

        private void Update()
        {
            HandleInput();
            _playerMovement.HandleMovement(moveDirection);
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
            if (_inputMove.isFingerDown){
 Vector3 input = _inputMove.direction;
                    moveDirection = new Vector3(input.x, 0, input.y).normalized;
            }
                else
 moveDirection = Vector3.zero;
#endif

            Debug.Log(moveDirection);
        }
    }
}