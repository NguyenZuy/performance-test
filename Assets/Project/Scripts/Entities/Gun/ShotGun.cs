using UnityEngine;
using System;
using ZuyZuy.PT.Entities.Zombie;

namespace ZuyZuy.PT.Entities.Gun
{
    public class ShotGun : BaseGun
    {
        [SerializeField] private float spreadRadius = 5f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private int numberOfPellets = 8; // Number of pellets in each shot
        [SerializeField] private float pelletLength = 100f; // Length of each pellet's raycast

        private Vector3[] lastPelletDirections; // Store last fired pellet directions

        protected override void AffectTarget()
        {
            lastPelletDirections = new Vector3[numberOfPellets];

            for (int i = 0; i < numberOfPellets; i++)
            {
                // Calculate random spread within the spread radius
                Vector3 randomSpread = new Vector3(
                    UnityEngine.Random.Range(-spreadRadius, spreadRadius),
                    UnityEngine.Random.Range(-spreadRadius, spreadRadius),
                    0
                );

                // Calculate the direction with spread
                Vector3 spreadDirection = transform.forward + randomSpread.normalized * (spreadRadius / 10f);
                spreadDirection.Normalize();

                lastPelletDirections[i] = spreadDirection;

                // Cast ray
                RaycastHit hit;
                if (Physics.Raycast(transform.position, spreadDirection, out hit, pelletLength, enemyLayer))
                {
                    // Check if the hit object has a ZombieController component
                    if (hit.collider.TryGetComponent<ZombieController>(out var zombie))
                    {
                        // Apply damage to the enemy
                        zombie.BeAttacked(m_Damage);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Draw the spread cone
            Gizmos.color = Color.yellow;
            float coneLength = 5f;
            Vector3 coneBase = transform.position + transform.forward * coneLength;

            // Draw the spread radius circle at the end of the cone
            int segments = 20;
            float angleStep = 360f / segments;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float nextAngle = (i + 1) * angleStep * Mathf.Deg2Rad;

                // Rotate the points based on the gun's rotation
                Vector3 point1 = coneBase + transform.rotation * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spreadRadius;
                Vector3 point2 = coneBase + transform.rotation * new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle), 0) * spreadRadius;

                Gizmos.DrawLine(coneBase, point1);
                Gizmos.DrawLine(point1, point2);
            }

            // Draw the last fired pellet trajectories
            if (lastPelletDirections != null)
            {
                Gizmos.color = Color.red;
                foreach (Vector3 direction in lastPelletDirections)
                {
                    // Rotate the direction based on the gun's rotation
                    Vector3 rotatedDirection = transform.rotation * direction;
                    Gizmos.DrawRay(transform.position, rotatedDirection * coneLength);
                }
            }
        }
    }
}
