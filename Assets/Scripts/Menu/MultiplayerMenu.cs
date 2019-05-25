using UnityEngine;

namespace Menu
{
    public class MultiplayerMenu : MonoBehaviour
    {
        // Start is called before the first frame update

        public void EnableMenu()
        {
            Canvas c = GetComponentInChildren<Canvas>();
            c.enabled = true;
        }

        public void DisableMenu()
        {
            Canvas c = GetComponentInChildren<Canvas>();
            c.enabled = false;
        }
    }
}
