using UnityEngine;
using ZuyZuy.PT.Entities;
using TriInspector;
using ZuyZuy.PT.Constants;
using ZuyZuy.PT.Utils;

namespace ZuyZuy.PT.SOs
{
    [CreateAssetMenu(fileName = "Level_", menuName = "SO/Level/Level_")]
    public class LevelSO : ScriptableObject
    {
        [Title("Level Settings")]
        public int LevelIndex;
        public SpawnWave[] SpawnWaves;

        private void OnValidate()
        {
            string newName = "Level_" + LevelIndex;
            if (name != newName)
            {
                name = newName;
#if UNITY_EDITOR
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    string directory = System.IO.Path.GetDirectoryName(assetPath);
                    string newPath = System.IO.Path.Combine(directory, newName + ".asset");
                    if (assetPath != newPath)
                    {
                        UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                    }
                }
#endif
            }
        }

        [Button]
        public void RandomInitializeData(
            int levelIndex,
            int numberSpawnWaves,
            int maxZombieLevel,
            int maxZombieCount
        )
        {
            if (levelIndex <= 0 || numberSpawnWaves <= 0 || maxZombieLevel <= 0 || maxZombieCount <= 0)
            {
                Debug.LogError("Level index, number of spawn waves, max zombie level and max zombie count must be greater than 0");
                return;
            }

            LevelIndex = levelIndex;

            // Create spawn waves
            int waveCount = numberSpawnWaves;
            SpawnWaves = new SpawnWave[waveCount];

            // Generate random spawn times first
            float[] spawnTimes = new float[waveCount];
            for (int i = 0; i < waveCount; i++)
            {
                spawnTimes[i] = Random.Range(0f, 30f);
            }
            // Sort spawn times in ascending order
            System.Array.Sort(spawnTimes);

            for (int i = 0; i < waveCount; i++)
            {
                SpawnWave wave = new SpawnWave();
                wave.timeToSpawn = spawnTimes[i];

                // Create 1-3 sub-waves
                int subWaveCount = Random.Range(1, 4);
                wave.waves = new Wave[subWaveCount];

                for (int j = 0; j < subWaveCount; j++)
                {
                    Wave subWave = new Wave();

                    // Get random zombie type and level
                    int randomZombieLevel = Random.Range(1, maxZombieLevel);
                    ZombieType randomZombieType = (ZombieType)Random.Range(0, System.Enum.GetValues(typeof(ZombieType)).Length);

                    // Generate zombie ID using the same formula as in ZombieSO
                    subWave.zombieId = ConvertUtils.GetZombieId(randomZombieType, randomZombieLevel);

                    // Scale zombie count based on wave index, starting from 5-10 and increasing by 5-10 per wave
                    int baseZombieCount = Random.Range(5, 10);
                    int waveBonus = i * Random.Range(5, 10); // Increased base bonus per wave
                    int totalZombieCount = baseZombieCount + waveBonus;

                    // Ensure the total zombie count doesn't exceed maxZombieCount
                    subWave.zombieCount = Mathf.Min(totalZombieCount, maxZombieCount);

                    // Random spawn rate between 0.5 and 2 seconds
                    subWave.spawnRate = Random.Range(0.5f, 2f);

                    wave.waves[j] = subWave;
                }

                SpawnWaves[i] = wave;
            }
        }
    }
}
