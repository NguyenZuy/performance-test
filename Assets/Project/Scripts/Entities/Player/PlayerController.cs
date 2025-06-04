using UnityEngine;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private PlayerAttack _playerAttack;
        private Vector3 moveDirection;

        void Start()
        {
            _playerMovement.OnMove += _playerAnimation.SetMotion;
        }

        private void Update()
        {
            HandleInput();
            _playerMovement.HandleMovement(moveDirection);
        }

        private void HandleInput()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Debug.Log($"Horizontal: {horizontalInput}, Vertical: {verticalInput}");

            moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        }
    }
}