using System;
using System.Collections;
using UnityEngine;
using ZuyZuy.PT.SOs;
using ZuyZuy.PT.Utils;
using ZuyZuy.PT.Entities;
using TriInspector;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        // Manager in charge of level management
        [Title("Level Manager")]
        [SerializeField] private Transform _mapParent;

        public Action OnLevelStart;
        public Action OnLevelEnd;
        public Action<int> OnWaveStart; // New event for wave start
        public Action<int> OnWaveEnd;   // New event for wave end

        private LevelSO _currentLevelSO;
        private int _currentWaveIndex = -1;
        private bool _isLevelActive = false;
        private Coroutine _waveCoroutine;
        private GameObject _currentMapInstance; // Reference to the current map instance

        void PrepareLevel(int levelIndex)
        {
            _currentLevelSO = ResourceUtil.LoadLevelSO(levelIndex);
            _currentWaveIndex = -1;
            _isLevelActive = false;

            // Load and instantiate the map
            LoadMap(_currentLevelSO.levelIndex);
        }

        private void LoadMap(int mapIndex)
        {
            // Clean up any existing map
            if (_currentMapInstance != null)
            {
                Destroy(_currentMapInstance);
                _currentMapInstance = null;
            }

            // Load and instantiate the new map
            GameObject mapPrefab = ResourceUtil.LoadMapPrefab(mapIndex);
            if (mapPrefab != null)
            {
                _currentMapInstance = Instantiate(mapPrefab, _mapParent);
                _currentMapInstance.transform.localScale = Vector3.one;
                _currentMapInstance.transform.localRotation = Quaternion.identity;
                _currentMapInstance.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError($"Failed to load map prefab for index: {mapIndex}");
            }
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

            // Clean up the map
            if (_currentMapInstance != null)
            {
                Destroy(_currentMapInstance);
                _currentMapInstance = null;
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
