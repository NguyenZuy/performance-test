using UnityEngine;

namespace ZuyZuy.PT.Entities
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private Transform[] _zombieSpawnPoints;

        public Transform[] GetZombieSpawnPoints()
        {
            return _zombieSpawnPoints;
        }
    }
}