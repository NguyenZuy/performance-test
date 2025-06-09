using UnityEngine;
using ZuyZuy.PT.Constants;
using System;
using ZuyZuy.PT.Entities.Player;
using TriInspector;
using System.Collections;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        [Title("Player Manager")]
        [SerializeField] private int _maxPlayerHP = 100;
        [ReadOnly][SerializeField] private int _curPlayerHP;
        [SerializeField] private bool _isPlayerDead = false;
        [SerializeField] private AudioClip _playerHurtSFX;
        [SerializeField] private AudioClip _playerDeathSFX;

        public Action<int> OnPlayerHPChanged;
        public Action OnPlayerDeath;

        public int MaxPlayerHP => _maxPlayerHP;
        public int CurPlayerHP => _curPlayerHP;


        public void InitializePlayerHP()
        {
            _curPlayerHP = _maxPlayerHP;
            _isPlayerDead = false;
            OnPlayerHPChanged?.Invoke(_curPlayerHP);
        }

        public void TakeDamage(int damage)
        {
            if (_isPlayerDead) return;

            _curPlayerHP = Mathf.Max(0, _curPlayerHP - damage);
            OnPlayerHPChanged?.Invoke(_curPlayerHP);

            // Play hurt sound effect
            if (_playerHurtSFX != null)
            {
                AudioSource.PlayClipAtPoint(_playerHurtSFX, Camera.main.transform.position);
            }

            if (_curPlayerHP <= 0)
            {
                HandlePlayerDeath();
            }
        }

        private void HandlePlayerDeath()
        {
            if (_isPlayerDead) return;
            _isPlayerDead = true;

            // Play death sound effect
            if (_playerDeathSFX != null)
            {
                AudioSource.PlayClipAtPoint(_playerDeathSFX, Camera.main.transform.position);
            }

            // Stop all game processes
            StopAllGameProcesses();

            // Trigger death effects
            OnPlayerDeath?.Invoke();
            // StartCoroutine(DeathEffect());

            PlayerController.Instance.PlayerAnimation.DeactiveAllRigs();

            // Show death popup
            StartCoroutine(DelayShowDeathPopup());
        }

        IEnumerator DelayShowDeathPopup()
        {
            yield return new WaitForSeconds(4.333f);
            ShowPopup(UIPopupName.DeathPopup);
        }

        private void StopAllGameProcesses()
        {
            // Stop all zombie spawning and wave progression
            _waveCancellationSource?.Cancel();
            _waveCancellationSource?.Dispose();
            _waveCancellationSource = null;
            _isLevelActive = false;

            // Stop all active zombies
            if (ZombiePool.Instance != null)
            {
                ZombiePool.Instance.ClearPool();
            }

            // Stop player movement and attacks
            if (PlayerController.Instance != null)
            {
                var playerMovement = PlayerController.Instance.GetComponent<PlayerMovement>();
                var playerAttack = PlayerController.Instance.PlayerAttack;

                if (playerMovement != null)
                {
                    playerMovement.enabled = false;
                }
                if (playerAttack != null)
                {
                    playerAttack.enabled = false;
                }
            }
        }

        public void RestartGame()
        {
            // Reset player state
            InitializePlayerHP();

            // Re-enable player components
            if (PlayerController.Instance != null)
            {
                var playerMovement = PlayerController.Instance.GetComponent<PlayerMovement>();
                var playerAttack = PlayerController.Instance.PlayerAttack;
                var playerAnimation = PlayerController.Instance.PlayerAnimation;

                playerAnimation.ActiveAllRigs();
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                }
                if (playerAttack != null)
                {
                    playerAttack.enabled = true;
                }

                // Respawn player at the spawn point
                PlayerController.Instance.Respawn();
            }

            // Clear all zombies
            if (ZombiePool.Instance != null)
            {
                ZombiePool.Instance.ClearPool();
            }

            // Restart the current level
            if (_currentLevelSO != null)
            {
                LaunchLevel(_currentLevelSO.LevelIndex);
            }
        }
    }
}
