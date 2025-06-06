using UnityEngine;
using ZuyZuy.PT.Constants;
using ZuyZuy.PT.Manager;
using ZuyZuy.PT.SOs;

namespace ZuyZuy.PT.Entities.Zombie
{
    [RequireComponent(typeof(ZombieController))]
    public class Zombie : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ZombieSO _zombieSO;

        public int ZombieId { get; private set; }
        public ZombieSO ZombieData => _zombieSO;
        private bool _isPooled = false;

        private void OnValidate()
        {
#if UNITY_EDITOR
            ZombieId = _zombieSO.id;

            // Only proceed if we have a valid ZombieId
            if (ZombieId > 0)
            {
                // Try to load the ZombieSO if it's not set
                if (_zombieSO == null)
                {
                    _zombieSO = Resources.Load<ZombieSO>(Path.ZOMBIE_SO + ZombieId);
                }

                // Update the GameObject name
                string newName = "Zombie_" + ZombieId;
                if (gameObject.name != newName)
                {
                    gameObject.name = newName;

                    // Handle prefab renaming
                    string assetPath = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        string directory = System.IO.Path.GetDirectoryName(assetPath);
                        string newPath = System.IO.Path.Combine(directory, newName + ".prefab");
                        if (assetPath != newPath)
                        {
                            UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                        }
                    }
                }
            }
#endif
        }

        // public void Initialize(int zombieId)
        // {
        //     ZombieId = zombieId;
        //     _isPooled = true;
        // }

        private void OnDisable()
        {
            if (_isPooled)
            {
                // Return to pool instead of destroying
                Manager.ZombiePool.Instance.ReturnZombie(ZombieId, gameObject);
            }
        }

        private void OnDestroy()
        {
            _isPooled = false;
        }
    }
}