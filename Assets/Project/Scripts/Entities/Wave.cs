using ZuyZuy.PT.Constants;

namespace ZuyZuy.PT.Entities
{
    [System.Serializable]
    public class Wave
    {
        public ZombieType zombieType; // The type of zombie to spawn in this wave
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
