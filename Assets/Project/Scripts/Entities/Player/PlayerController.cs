using UnityEngine;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;

        private Vector3 moveDirection;

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