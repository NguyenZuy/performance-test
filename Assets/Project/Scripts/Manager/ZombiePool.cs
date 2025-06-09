using System.Collections.Generic;
using UnityEngine;
using ZuyZuy.Workspace;
using ZuyZuy.PT.Utils;
using ZuyZuy.PT.Entities.Zombie;

namespace ZuyZuy.PT.Manager
{
    public class ZombiePool : BaseSingleton<ZombiePool>
    {
        private Dictionary<int, Queue<GameObject>> _poolDictionary = new Dictionary<int, Queue<GameObject>>();
        private Dictionary<int, GameObject> _prefabDictionary = new Dictionary<int, GameObject>();
        private Transform _poolParent;
        private int _activeZombieCount = 0;

        private void Start()
        {
            _poolParent = new GameObject("PooledZombies").transform;
            _poolParent.SetParent(transform);
        }

        public int GetActiveZombieCount()
        {
            return _activeZombieCount;
        }

        public void InitializePool(int zombieId, int initialSize)
        {
            if (!_poolDictionary.ContainsKey(zombieId))
            {
                _poolDictionary[zombieId] = new Queue<GameObject>();

                // Load and cache the prefab
                GameObject prefab = ResourceUtil.LoadZombiePrefab(zombieId);
                if (prefab == null)
                {
                    Debug.LogError($"Failed to load zombie prefab for ID: {zombieId}");
                    return;
                }
                _prefabDictionary[zombieId] = prefab;

                // Create initial pool
                for (int i = 0; i < initialSize; i++)
                {
                    CreateNewZombie(zombieId);
                }
            }
        }

        private void CreateNewZombie(int zombieId)
        {
            GameObject zombie = Instantiate(_prefabDictionary[zombieId], _poolParent);
            zombie.SetActive(false);
            _poolDictionary[zombieId].Enqueue(zombie);
        }

        public GameObject GetZombie(int zombieId, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(zombieId))
            {
                InitializePool(zombieId, 5); // Initialize with 5 zombies if not already initialized
            }

            Queue<GameObject> pool = _poolDictionary[zombieId];

            // If pool is empty, create a new zombie
            if (pool.Count == 0)
            {
                CreateNewZombie(zombieId);
            }

            GameObject zombie = pool.Dequeue();
            zombie.transform.position = position;
            zombie.transform.rotation = rotation;
            zombie.SetActive(true);
            _activeZombieCount++;

            return zombie;
        }

        public void ReturnZombie(int zombieId, GameObject zombie)
        {
            if (!_poolDictionary.ContainsKey(zombieId))
            {
                Debug.LogWarning($"Attempting to return zombie with ID {zombieId} to non-existent pool");
                return;
            }

            zombie.SetActive(false);
            zombie.transform.SetParent(_poolParent);
            _poolDictionary[zombieId].Enqueue(zombie);
            _activeZombieCount--;
        }

        public void ClearPool()
        {
            // First, find and destroy all active zombies in the scene
            ZombieController[] activeZombies = FindObjectsOfType<ZombieController>();
            foreach (var zombie in activeZombies)
            {
                if (zombie != null)
                {
                    Destroy(zombie.gameObject);
                }
            }

            // Then clear the pool
            foreach (var pool in _poolDictionary.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject zombie = pool.Dequeue();
                    if (zombie != null)
                    {
                        Destroy(zombie);
                    }
                }
            }
            _poolDictionary.Clear();
            _prefabDictionary.Clear();
            _activeZombieCount = 0;
        }
    }
}