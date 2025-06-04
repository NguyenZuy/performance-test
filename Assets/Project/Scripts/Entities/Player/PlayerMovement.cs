using System;
using UnityEngine;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float gravity = -15.0f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private AnimationCurve turningPowerCurve;
        [SerializeField] private bool lerpStopping = true;

        public Action<Vector3> OnMove;

        private float verticalVelocity;
        private bool isGrounded;
        private float terminalVelocity = 53.0f;
        private Vector3 cachedInput;
        private float cachedRotationValue;

        private void Update()
        {
            GroundedCheck();
            ApplyGravity();
        }

        public void HandleMovement(Vector3 moveDirection)
        {
            // Cache the input for smooth stopping
            if (moveDirection.magnitude >= 0.1f)
            {
                cachedInput = moveDirection;
            }
            else if (lerpStopping)
            {
                cachedInput.x = Mathf.Lerp(cachedInput.x, 0f, rotationSpeed * Time.deltaTime);
                cachedInput.z = Mathf.Lerp(cachedInput.z, 0f, moveSpeed * Time.deltaTime);
            }
            else
            {
                cachedInput = Vector3.zero;
            }

            // Calculate rotation based on horizontal input
            if (cachedInput.x != 0f)
            {
                if (cachedInput.x < 0f)
                {
                    cachedRotationValue = -1f * turningPowerCurve.Evaluate(Mathf.Abs(cachedInput.x));
                }
                else
                {
                    cachedRotationValue = turningPowerCurve.Evaluate(Mathf.Abs(cachedInput.x));
                }
                transform.Rotate(transform.up, cachedRotationValue * rotationSpeed * Time.deltaTime);
            }

            // Apply movement
            if (cachedInput.magnitude >= 0.1f)
            {
                Vector3 movement = transform.forward * cachedInput.z * moveSpeed * Time.deltaTime;
                movement.y = verticalVelocity * Time.deltaTime;
                characterController.Move(movement);

                // Normalize the movement direction for consistent animation values
                Vector3 normalizedMovement = new Vector3(cachedInput.x, 0, cachedInput.z).normalized;
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
