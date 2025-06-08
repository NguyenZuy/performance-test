using UnityEngine;
using System.Collections;
using ZuyZuy.PT.Entities.Zombie;
using ZuyZuy.PT.Entities.Player;
using TriInspector;
using System;

namespace ZuyZuy.PT.Entities.Gun
{
    public class RifleGun : BaseGun
    {
        [Title("Rifle Settings")]
        [SerializeField] private float bulletLength = 200f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private TrailRenderer _bulletTracerPrefab;
        [SerializeField] private Transform _headGun;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _gunshotClip;
        [SerializeField] private GameObject _bulletHoleDecalPrefab;
        [SerializeField] private float _recoilKickback = 0.1f; // How much the gun kicks back when firing

        private PlayerAttack _playerAttack;
        private Vector3 _originalPosition;

        private void Awake()
        {
            _playerAttack = FindFirstObjectByType<PlayerAttack>();
            if (_headGun != null)
            {
                _originalPosition = _headGun.localPosition;
            }
        }

        private void PlayGunshotSound()
        {
            if (_audioSource && _gunshotClip)
                _audioSource.PlayOneShot(_gunshotClip);
        }

        protected override void AffectTarget()
        {
            PlayGunshotSound();

            // Get target direction
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

            // Create bullet trail
            TrailRenderer trail = Instantiate(_bulletTracerPrefab, _headGun.position, Quaternion.LookRotation(targetDirection));

            // Cast ray and collect all hits
            RaycastHit[] hits = Physics.RaycastAll(_headGun.position, targetDirection, bulletLength, enemyLayer);
            Vector3 endPoint = _headGun.position + targetDirection * bulletLength;

            // Sort hits by distance
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // Process all hits
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.TryGetComponent<ZombieController>(out var zombie))
                {
                    zombie.BeAttacked(m_Damage);

                    // Play hit effect
                    if (m_HitEffectPrefab != null)
                    {
                        GameObject hitEffect = Instantiate(m_HitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                        ParticleSystem hitParticles = hitEffect.GetComponent<ParticleSystem>();
                        if (hitParticles != null)
                        {
                            hitParticles.Play();
                            Destroy(hitEffect, hitParticles.main.duration);
                        }
                    }

                    // Spawn bullet hole decal
                    if (_bulletHoleDecalPrefab)
                    {
                        GameObject decal = Instantiate(_bulletHoleDecalPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                        Destroy(decal, 10f);
                    }
                }
            }

            // // If we hit anything, set the end point to the last hit
            // if (hits.Length > 0)
            // {
            //     endPoint = hits[hits.Length - 1].point;
            // }

            // Animate the bullet trail
            StartCoroutine(AnimateTrail(trail, endPoint));

            // Apply kickback effect
            StartCoroutine(ApplyKickback());
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

        private IEnumerator ApplyKickback()
        {
            if (_headGun != null)
            {
                // Move gun back
                _headGun.localPosition = _originalPosition + Vector3.back * _recoilKickback;

                // Return to original position
                float time = 0;
                float duration = 0.1f;
                while (time < duration)
                {
                    _headGun.localPosition = Vector3.Lerp(_headGun.localPosition, _originalPosition, time / duration);
                    time += Time.deltaTime;
                    yield return null;
                }
                _headGun.localPosition = _originalPosition;
            }
        }
    }
}
