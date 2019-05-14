using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoiledWings : MonoBehaviour {
    public float idleAngle;
    public float unfoilAngle;
    public bool unfoiled;
    Vector3 v = new Vector3();
    // Start is called before the first frame update

    public void Start () {
        transform.Rotate (0f, 0f, 0f);
    }

    public void Update () {
        FoilWings ();
    }

    public void FoilWings () {
        if (unfoiled) {
            v.z = unfoilAngle;
            transform.localEulerAngles = v;
        } else {
            v.z = idleAngle;
            transform.localEulerAngles = v;
        }
    }
}