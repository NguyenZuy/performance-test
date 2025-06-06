using ZuyZuy.PT.Constants;
using ZuyZuy.PT.SOs;

namespace ZuyZuy.PT.Entities
{
    [System.Serializable]
    public class Wave
    {
        public int zombieId;
        public int zombieCount; // Number of zombies to spawn in this wave
        public float spawnRate; // Time interval between zombie spawns in seconds
    }

    [System.Serializable]
    public class SpawnWave
    {
        public float timeToSpawn; // Time to spawn the wave calculate from start of the level
        public Wave[] waves; // Waves to spawn in this wave
    }
}
