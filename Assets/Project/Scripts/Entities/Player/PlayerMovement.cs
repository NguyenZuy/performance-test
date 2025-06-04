using System;
using UnityEngine;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -15.0f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private LayerMask groundLayers;

        public Action<Vector3> OnMove;

        private float verticalVelocity;
        private bool isGrounded;
        private float terminalVelocity = 53.0f;

        private void Update()
        {
            GroundedCheck();
            ApplyGravity();
        }

        public void HandleMovement(Vector3 moveDirection)
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
                movement.y = verticalVelocity * Time.deltaTime;
                characterController.Move(movement);

                // Normalize the movement direction for consistent animation values
                Vector3 normalizedMovement = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
                OnMove?.Invoke(normalizedMovement);
            }
            else
            {
                // Apply only vertical movement when not moving horizontally
                characterController.Move(new Vector3(0, verticalVelocity * Time.deltaTime, 0));
                OnMove?.Invoke(Vector3.zero);
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                QueryTriggerInteraction.Ignore);

            // Reset vertical velocity when grounded
            if (isGrounded && verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }
        }

        private void ApplyGravity()
        {
            if (!isGrounded)
            {
                // Apply gravity over time if under terminal velocity
                if (verticalVelocity < terminalVelocity)
                {
                    verticalVelocity += gravity * Time.deltaTime;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (isGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                groundedRadius);
        }
    }
}
