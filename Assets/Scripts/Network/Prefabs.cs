using System.Collections.Generic;

namespace Network
{
    public class Prefabs
    {
        private static Dictionary<string, int> prefabs = new Dictionary<string, int>{
            {"shuttle", 1}
        };

        public static int GetFromString(string prefab)
        {
            return prefabs[prefab];
        }

        public static string GetFromInt(int prefab)
        {
            foreach (KeyValuePair<string, int> p in prefabs)
            {
                if (p.Value == prefab)
                {
                    return p.Key;
                }
            }

            return "";
        }
    }
}