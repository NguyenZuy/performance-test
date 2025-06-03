using UnityEngine;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;

        public void HandleMovement(Vector3 moveDirection)
        {
            if (moveDirection.magnitude >= 0.1f)
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

    }
}
