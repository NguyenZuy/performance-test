using TriInspector;
using UnityEngine;
using ZuyZuy.PT.Constants;
using ZuyZuy.PT.Utils;

namespace ZuyZuy.PT.SOs
{
    [CreateAssetMenu(fileName = "Zombie_", menuName = "SO/Zombie/Zombie_")]
    public class ZombieSO : ScriptableObject
    {
        [Title("Zombie Settings")]
        [ReadOnly] public int id;
        public int ZombieLevel;
        public ZombieType ZombieType;
        public float Health;
        public float Speed;
        public float AttackRange;
        public float AttackDamage;
        public float AttackSpeed;

        private void OnValidate()
        {
            // Uniques ID for each zombie
            id = ConvertUtils.GetZombieId(ZombieType, ZombieLevel);

            string newName = $"Zombie_{ZombieType}_{ZombieLevel}";
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
    }
}