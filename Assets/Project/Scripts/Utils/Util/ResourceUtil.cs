using UnityEngine;
using ZuyZuy.PT.Constants;
using ZuyZuy.PT.SOs;

namespace ZuyZuy.PT.Utils
{
    public static class ResourceUtil
    {
        public static T LoadResource<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public static LevelSO LoadLevelSO(int levelIndex)
        {
            return LoadResource<LevelSO>(Path.LEVEL_SO + levelIndex);
        }

        public static GameObject LoadZombiePrefab(int zombieId)
        {
            return LoadResource<GameObject>(Path.ZOMBIE_PREFAB + zombieId);
        }

        public static GameObject LoadMapPrefab(int mapIndex)
        {
            return LoadResource<GameObject>(Path.MAP_PREFAB + mapIndex);
        }
    }
}
