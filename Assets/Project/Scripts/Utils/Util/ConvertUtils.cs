using ZuyZuy.PT.Constants;

namespace ZuyZuy.PT.Utils
{
    public static class ConvertUtils
    {
        public static int GetZombieId(ZombieType zombieType, int zombieLevel)
        {
            return ((int)zombieType << 16) | (zombieLevel & 0xFFFF);
        }

        public static ZombieType GetZombieType(int zombieId)
        {
            return (ZombieType)(zombieId >> 16);
        }

        public static int GetZombieLevel(int zombieId)
        {
            return zombieId & 0xFFFF;
        }
    }
}