using UnityEngine;

namespace Network
{
    public class NetworkBootstrap : MonoBehaviour
    {
        private bool _multiplayer = false;

        private void OnEnable()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-dedicated")
                {
                    _multiplayer = true;
                }
            }

            if (_multiplayer)
            {
                GameObject obj = GameObject.Find("client");
                Destroy(obj);
            }
            else
            {
                GameObject obj = GameObject.Find("server");
                Destroy(obj);
            }
        }
    }
}
