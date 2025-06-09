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
                PlaySFX(_playerHurtSFX, 0.5f);
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
                PlaySFX(_playerDeathSFX, 0.5f);
            }

            // Trigger death effects
            OnPlayerDeath?.Invoke();

            StopAllGameProcesses();

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
            _isPlayerDead = false;

            // Re-enable player components
            if (PlayerController.Instance != null)
            {
                var playerMovement = PlayerController.Instance.GetComponent<PlayerMovement>();
                var playerAttack = PlayerController.Instance.PlayerAttack;
                var playerAnimation = PlayerController.Instance.PlayerAnimation;

                // Reset animation state
                playerAnimation.ActiveAllRigs();
                playerAnimation.ResetAnimationState();

                // Re-enable components
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

            // Clear all zombies and reset the pool
            if (ZombiePool.Instance != null)
                ZombiePool.Instance.ClearPool();

            // Reset wave system
            _waveCancellationSource?.Cancel();
            _waveCancellationSource?.Dispose();
            _waveCancellationSource = new System.Threading.CancellationTokenSource();
            _isLevelActive = true;

            // Reload the current level
            if (_currentLevelIndex >= 0)
            {
                LaunchLevel(_currentLevelIndex);
            }
        }
    }
}
