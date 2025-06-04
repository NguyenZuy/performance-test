using System;
using System.Collections;
using UnityEngine;
using ZuyZuy.PT.SOs;
using ZuyZuy.PT.Utils;
using ZuyZuy.PT.Entities;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        // Manager in charge of level management

        public Action OnLevelStart;
        public Action OnLevelEnd;
        public Action<int> OnWaveStart; // New event for wave start
        public Action<int> OnWaveEnd;   // New event for wave end

        private LevelSO _currentLevelSO;
        private int _currentWaveIndex = -1;
        private bool _isLevelActive = false;
        private Coroutine _waveCoroutine;

        void PrepareLevel(int levelIndex)
        {
            _currentLevelSO = ResourceUtil.LoadLevelSO(levelIndex);
            _currentWaveIndex = -1;
            _isLevelActive = false;
        }

        void StartLevel()
        {
            _isLevelActive = true;
            StartNextWave();
        }

        private void StartNextWave()
        {
            if (!_isLevelActive) return;

            _currentWaveIndex++;

            // Check if we've completed all waves
            if (_currentWaveIndex >= _currentLevelSO.spawnWaves.Length)
            {
                EndLevel();
                return;
            }

            // Start the wave coroutine
            if (_waveCoroutine != null)
            {
                StopCoroutine(_waveCoroutine);
            }
            _waveCoroutine = StartCoroutine(SpawnWaveCoroutine(_currentWaveIndex));
        }

        private IEnumerator SpawnWaveCoroutine(int waveIndex)
        {
            SpawnWave spawnWave = _currentLevelSO.spawnWaves[waveIndex];

            // Wait for the wave's start time
            yield return new WaitForSeconds(spawnWave.timeToSpawn);

            OnWaveStart?.Invoke(waveIndex);

            // Spawn each sub-wave
            foreach (Wave wave in spawnWave.waves)
            {
                for (int i = 0; i < wave.zombieCount; i++)
                {
                    // wave.zombieType.SpawnZombie();
                    yield return new WaitForSeconds(wave.spawnRate);
                }
            }

            OnWaveEnd?.Invoke(waveIndex);

            // Start the next wave
            StartNextWave();
        }

        private void EndLevel()
        {
            _isLevelActive = false;
            if (_waveCoroutine != null)
            {
                StopCoroutine(_waveCoroutine);
                _waveCoroutine = null;
            }
            OnLevelEnd?.Invoke();
        }

        public void LaunchLevel(int levelIndex)
        {
            PrepareLevel(levelIndex);
            StartLevel();
            OnLevelStart?.Invoke();
        }
    }
}
