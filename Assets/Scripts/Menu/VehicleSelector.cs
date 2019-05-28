using System.Collections.Generic;
using Resource;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class VehicleSelector : MonoBehaviour
    {
        public RectTransform content;
        // Start is called before the first frame update

        private GameObject button;

        public void LoadShipList()
        {
            button = Resources.Load("Button") as GameObject;
            foreach (KeyValuePair<int, Scriptable.Vehicle> v in Loader.instance.vehicles)
            {
                GameObject obj = Instantiate(button, content.gameObject.transform);
                obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                obj.GetComponentInChildren<Text>().text = v.Value.name;
            }
        }
    }
}
