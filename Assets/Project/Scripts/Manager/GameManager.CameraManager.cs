using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using TriInspector;
namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private float _damageShakeIntensity = 1f;
        [SerializeField] private float _damageShakeDuration = 0.3f;
        [SerializeField] private float _deathShakeIntensity = 2f;
        [SerializeField] private float _deathShakeDuration = 0.5f;
        [SerializeField] private float _shootShakeIntensity = 0.5f;
        [SerializeField] private float _shootShakeDuration = 0.1f;


        private CinemachineBasicMultiChannelPerlin _noise;
        private Color _originalColor;
        private float _currentShakeIntensity;

        private void InitializeCamera()
        {
            if (_playerCamera != null)
            {
                _noise = _playerCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
            if (_mainCamera != null)
            {
                _originalColor = _mainCamera.backgroundColor;
            }
        }

        [Button("Test Damage Effect")]
        public void OnPlayerDamaged()
        {
            StartCoroutine(DamageEffect());
        }

        [Button("Test Death Effect")]
        public void OnPlayerDeathClick()
        {
            StartCoroutine(DeathEffect());
        }

        [Button("Test Shoot Effect")]
        public void OnPlayerShoot()
        {
            StartCoroutine(ShootEffect());
        }

        private IEnumerator DamageEffect()
        {
            // Camera shake
            if (_noise != null)
            {
                _noise.AmplitudeGain = _damageShakeIntensity;
            }

            // Red flash effect
            if (_mainCamera != null)
            {
                // Flash red
                _mainCamera.backgroundColor = Color.red;
                yield return new WaitForSeconds(0.1f);
                _mainCamera.backgroundColor = _originalColor;
                yield return new WaitForSeconds(0.1f);
                _mainCamera.backgroundColor = Color.red;
                yield return new WaitForSeconds(0.1f);
                _mainCamera.backgroundColor = _originalColor;
            }

            // Reset camera shake
            if (_noise != null)
            {
                _noise.AmplitudeGain = 0f;
            }
        }

        private IEnumerator DeathEffect()
        {
            if (_noise != null)
            {
                _noise.AmplitudeGain = _deathShakeIntensity;
            }

            if (_mainCamera != null)
            {
                // More intense red flash for death
                for (int i = 0; i < 3; i++)
                {
                    _mainCamera.backgroundColor = Color.red;
                    yield return new WaitForSeconds(1f);
                    _mainCamera.backgroundColor = _originalColor;
                    yield return new WaitForSeconds(0.5f);
                }
            }

            if (_noise != null)
            {
                _noise.AmplitudeGain = 0f;
            }
        }

        private IEnumerator ShootEffect()
        {
            if (_noise != null)
            {
                _noise.AmplitudeGain = _shootShakeIntensity;
            }

            yield return new WaitForSeconds(_shootShakeDuration);

            if (_noise != null)
            {
                _noise.AmplitudeGain = 0f;
            }
        }
    }
}