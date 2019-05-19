using UnityEngine;

public class ShipController : MonoBehaviour {

    private float yaw;
    private float pitch;
    private float roll;

    [SerializeField]
    private float currentSpeed;
    private float maxSpeed;

    public GameObject shipControlled;
    Transform t;
    ShipAttributes a;
    bool tookOff = false;
    float accel = 0f;
    private int shipId = 0;
    public bool shipControl = false;
    private bool shipInitialized = false;

    private void Start()
    {
    }

    // Start is called before the first frame update
    void InitShip ()
    {
        maxSpeed = shipControlled.GetComponent<ShipAttributes> ().maxSpeed;
        accel = shipControlled.GetComponent<ShipAttributes> ().accel;
        t = shipControlled.transform;
        a = shipControlled.GetComponent<ShipAttributes>();
        currentSpeed = 0f;
        pitch = 0f;
        yaw = 0f;
        roll = 0f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!shipControl)
        {
            return;
        }
        else
        {
            if (!shipInitialized)
            {
                InitShip();
                shipInitialized = true;
            }
        }
        
        GetMouseInput();
        GetKeyInput();
        UpdateRotation();
        Move();
    }

    private void GetMouseInput () {
        roll = Input.GetAxis ("Roll") * a.speedRoll * Time.deltaTime;
        pitch = Input.GetAxis ("Vertical") * a.speedYaw * Time.deltaTime;
        yaw = Input.GetAxis ("Horizontal") * a.speedPitch * Time.deltaTime;
    }

    private void GetKeyInput () {
        if (Input.GetButton ("TakeOff")) {
            TakeOff ();
        }

        if (Input.GetButton ("Accelerate")) {
            currentSpeed += Input.GetAxis ("Accelerate") * accel * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp (currentSpeed, 0f, maxSpeed);

        float fire = Input.GetAxisRaw ("Fire3");
        if (fire != 0)
            Debug.Log ("Fire!");

    }

    private void TakeOff () {
        if (!tookOff) {
            tookOff = true;
            FoiledWings[] wings = shipControlled.gameObject.GetComponentsInChildren<FoiledWings> ();
            if (wings.Length > 0) {
                foreach (FoiledWings wing in wings) {
                    wing.unfoiled = true;
                }
            }
        }
    }

    private void UpdateRotation () {
        yaw = Mathf.Clamp (yaw, -0.5f, 0.5f);
        pitch = Mathf.Clamp (pitch, -0.5f, 0.5f);
        roll = Mathf.Clamp (roll, -0.5f, 0.5f);
        t.Rotate (pitch, yaw, roll);
    }

    private void Move () {
        t.position += t.forward * Time.deltaTime * currentSpeed;
    }
}