using Server;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (GameServer.instance != null)
            {
                Destroy(this);
            }
        }
    }
}

