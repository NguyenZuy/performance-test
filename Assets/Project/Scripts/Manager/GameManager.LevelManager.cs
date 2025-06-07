using System;
using System.Collections;
using UnityEngine;
using ZuyZuy.PT.SOs;
using ZuyZuy.PT.Utils;
using ZuyZuy.PT.Entities;
using ZuyZuy.PT.Constants;
using TriInspector;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        // Manager in charge of level management
        [Title("Level Manager")]
        [SerializeField] private Transform _mapParent;
        [SerializeField] private int _maxZombieCount = 20; // max zombie appear at the same time

        public Action OnLevelStart;
        public Action OnLevelEnd;
        public Action<int> OnWaveStart;
        public Action<int> OnWaveEnd;

        private LevelSO _currentLevelSO;
        private int _currentWaveIndex = -1;
        private bool _isLevelActive = false;
        private CancellationTokenSource _waveCancellationSource;
        private GameObject _currentMapInstance;
        private Map _currentMap;

        async UniTask PrepareLevel(int levelIndex)
        {
            _currentLevelSO = ResourceUtil.LoadLevelSO(levelIndex);
            _currentWaveIndex = -1;
            _isLevelActive = false;

            // Load and instantiate the map
            await LoadMap(_currentLevelSO.LevelIndex);

            InitializeZombiePools();
        }

        private void InitializeZombiePools()
        {
            // Initialize pools for each zombie type that will be used in this level
            foreach (SpawnWave spawnWave in _currentLevelSO.SpawnWaves)
            {
                foreach (Wave wave in spawnWave.waves)
                {
                    // Initialize pool with initial size based on the wave's zombie count
                    ZombiePool.Instance.InitializePool(wave.zombieId, wave.zombieCount);
                }
            }
        }

        private async UniTask LoadMap(int mapIndex)
        {
            // Clean up any existing map
            if (_currentMapInstance != null)
            {
                Destroy(_currentMapInstance);
                _currentMapInstance = null;
                _currentMap = null;
            }

            // Load and instantiate the new map
            GameObject mapPrefab = ResourceUtil.LoadMapPrefab(mapIndex);
            if (mapPrefab != null)
            {
                _currentMapInstance = Instantiate(mapPrefab, _mapParent);
                _currentMapInstance.transform.localScale = Vector3.one;
                _currentMapInstance.transform.localRotation = Quaternion.identity;
                _currentMapInstance.transform.localPosition = Vector3.zero;

                // Get the Map component
                _currentMap = _currentMapInstance.GetComponent<Map>();
                if (_currentMap == null)
                {
                    Debug.LogError("Map component not found on map prefab!");
                }
            }
            else
            {
                Debug.LogError($"Failed to load map prefab for index: {mapIndex}");
            }

            // Wait for next frame to ensure map is fully loaded
            await UniTask.Yield();
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
            if (_currentWaveIndex >= _currentLevelSO.SpawnWaves.Length)
            {
                EndLevel();
                return;
            }

            // Cancel previous wave if it exists
            _waveCancellationSource?.Cancel();
            _waveCancellationSource = new CancellationTokenSource();

            // Start the wave coroutine
            SpawnWaveAsync(_currentWaveIndex, _waveCancellationSource.Token).Forget();
        }

        private async UniTask SpawnWaveAsync(int waveIndex, CancellationToken cancellationToken)
        {
            SpawnWave spawnWave = _currentLevelSO.SpawnWaves[waveIndex];

            // Wait for the wave's start time
            await UniTask.Delay(TimeSpan.FromSeconds(spawnWave.timeToSpawn), cancellationToken: cancellationToken);

            OnWaveStart?.Invoke(waveIndex);

            // Spawn each sub-wave
            foreach (Wave wave in spawnWave.waves)
            {
                for (int i = 0; i < wave.zombieCount; i++)
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    SpawnZombie(wave.zombieId);
                    await UniTask.Delay(TimeSpan.FromSeconds(wave.spawnRate), cancellationToken: cancellationToken);
                }
            }

            OnWaveEnd?.Invoke(waveIndex);

            // Start the next wave
            StartNextWave();
        }

        private void SpawnZombie(int zombieId)
        {
            if (_currentMap == null)
            {
                Debug.LogError("Current map is not set!");
                return;
            }

            // Check if we've reached the maximum zombie count
            if (ZombiePool.Instance.GetActiveZombieCount() >= _maxZombieCount)
            {
                return; // Don't spawn more zombies if we've reached the limit
            }

            var spawnPoints = _currentMap.GetZombieSpawnPoints();
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("Zombie spawn points are not set in the current map!");
                return;
            }

            // Randomly select a spawn point from the array
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            // Calculate random offset (adjust these values based on your game's scale)
            float randomRadius = UnityEngine.Random.Range(1f, 3f);
            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            Vector3 randomOffset = new Vector3(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomRadius,
                0f,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomRadius
            );

            // Apply the random offset to the spawn position
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            // Get zombie from pool instead of instantiating
            GameObject zombieInstance = ZombiePool.Instance.GetZombie(zombieId, spawnPosition, spawnPoint.rotation);
            zombieInstance.transform.SetParent(_mapParent);
        }

        private void EndLevel()
        {
            _isLevelActive = false;
            _waveCancellationSource?.Cancel();
            _waveCancellationSource?.Dispose();
            _waveCancellationSource = null;

            // Clean up the map
            if (_currentMapInstance != null)
            {
                Destroy(_currentMapInstance);
                _currentMapInstance = null;
            }

            // Clear the zombie pool
            ZombiePool.Instance.ClearPool();

            OnLevelEnd?.Invoke();
        }

        public async void LaunchLevel(int levelIndex)
        {
            await PrepareLevel(levelIndex);
            StartLevel();
            OnLevelStart?.Invoke();
        }

        [Button("Test Level")]
        public void TestLevel(int levelIndex)
        {
            LaunchLevel(levelIndex);
        }
    }
}
