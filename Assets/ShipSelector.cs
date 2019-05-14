using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelector : MonoBehaviour
{
    public RectTransform content;
    // Start is called before the first frame update

    private GameObject button;

    public void LoadShipList()
    {
        button = Resources.Load("Button") as GameObject;
        foreach (KeyValuePair<int, Vehicle> v in Loader.instance.vehicles)
        {
            GameObject obj = Instantiate(button);
            obj.transform.parent = content.gameObject.transform;
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            obj.GetComponentInChildren<Text>().text = v.Value.name;
        }
    }
}
