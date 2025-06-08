using UnityEngine;
using System;
using ZuyZuy.PT.Entities.Zombie;
using System.Collections;
using ZuyZuy.PT.Entities.Player;
using TriInspector;

namespace ZuyZuy.PT.Entities.Gun
{
    public class ShotGun : BaseGun
    {
        [Title("Shotgun Settings")]
        [SerializeField] private float spreadRadius = 5f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private int numberOfPellets = 8; // Number of pellets in each shot
        [SerializeField] private float pelletLength = 100f; // Length of each pellet's raycast
        [SerializeField] private TrailRenderer _bulletTracerPrefab;
        [SerializeField] private Transform _headGun; // bullet trail will be spawned from this transform
        [SerializeField] private AudioSource _audioSource; // Gunshot sound
        [SerializeField] private AudioClip _gunshotClip; // Gunshot sound clip
        [SerializeField] private GameObject _bulletHoleDecalPrefab; // Bullet hole decal prefab

        private Vector3[] lastPelletDirections; // Store last fired pellet directions
        private PlayerAttack _playerAttack; // Reference to PlayerAttack component

        private void Awake()
        {
            _playerAttack = FindFirstObjectByType<PlayerAttack>();
        }

        private void PlayGunshotSound()
        {
            if (_audioSource && _gunshotClip)
                _audioSource.PlayOneShot(_gunshotClip);
        }

        protected override void AffectTarget()
        {
            lastPelletDirections = new Vector3[numberOfPellets];
            PlayGunshotSound();

            // Get target direction (ignoring Y-axis)
            Vector3 targetDirection = Vector3.forward;
            if (_playerAttack != null && _playerAttack.NearestZombie != null)
            {
                Vector3 targetPosition = _playerAttack.NearestZombie.transform.position;
                Vector3 currentPosition = transform.position;

                // Create direction vector ignoring Y-axis
                targetDirection = new Vector3(
                    targetPosition.x - currentPosition.x,
                    0,
                    targetPosition.z - currentPosition.z
                ).normalized;
            }

            for (int i = 0; i < numberOfPellets; i++)
            {
                // Apply spread around the target direction
                Vector3 spreadDirection = Quaternion.Euler(
                    UnityEngine.Random.Range(-spreadRadius, spreadRadius),
                    UnityEngine.Random.Range(-spreadRadius, spreadRadius),
                    0) * targetDirection;
                spreadDirection.Normalize();
                lastPelletDirections[i] = spreadDirection;

                // Create trail renderer for this pellet
                TrailRenderer trail = Instantiate(_bulletTracerPrefab, _headGun.position, Quaternion.LookRotation(spreadDirection));

                // Cast ray
                RaycastHit hit;
                Vector3 endPoint;
                if (Physics.Raycast(_headGun.position, spreadDirection, out hit, pelletLength, enemyLayer))
                {
                    // Check if the hit object has a ZombieController component
                    if (hit.collider.TryGetComponent<ZombieController>(out var zombie))
                    {
                        // Apply damage to the enemy
                        zombie.BeAttacked(m_Damage);

                        // Play hit effect at the impact point
                        if (m_HitEffect != null)
                        {
                            m_HitEffect.transform.position = hit.point;
                            m_HitEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
                            m_HitEffect.Play();
                        }
                    }
                    endPoint = hit.point;

                    // Spawn bullet hole decal
                    if (_bulletHoleDecalPrefab)
                    {
                        GameObject decal = Instantiate(_bulletHoleDecalPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                        Destroy(decal, 10f);
                    }
                }
                else
                {
                    // If no hit, set trail end position to max distance
                    endPoint = _headGun.position + spreadDirection * pelletLength;
                }

                // Animate the trail
                StartCoroutine(AnimateTrail(trail, endPoint));
            }
        }

        private IEnumerator AnimateTrail(TrailRenderer trail, Vector3 endPoint)
        {
            float time = 0;
            Vector3 startPoint = trail.transform.position;
            float duration = 0.05f; // Bullet travel time
            while (time < duration)
            {
                trail.transform.position = Vector3.Lerp(startPoint, endPoint, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            trail.transform.position = endPoint;
            Destroy(trail.gameObject, trail.time);
        }
    }
}
