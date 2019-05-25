using Client;
using Resource;
using Scriptable;
using UnityEngine;

namespace VehicleFunctions
{
    public class VehicleController : MonoBehaviour
    {
        private float yaw;
        private float pitch;
        private float roll;
        public float speedRoll;
        public float speedYaw;
        public float speedPitch;
        public float currentSpeed;
        public float maxSpeed;
        public GameObject vehicleControlled;
        Transform _vTransform;
        private bool _tookOff;
        private float _acceleration;
        public bool vehicleControl;
        public bool vehicleInitialized;
        public ClientGameManager game;
        private FoiledWings[] _wings;

        private void Start()
        {
            game = GetComponent<ClientGameManager>();
        }

        // Start is called before the first frame update
        void InitShip(int vehicleDatabaseId)
        {
            Vehicle V = Loader.instance.vehicles[vehicleDatabaseId];
            maxSpeed = V.maximumSpeed;
            speedYaw = V.yawSpeed;
            speedPitch = V.pitchSpeed;
            speedRoll = V.rollSpeed;
            _acceleration = V.acceleration;
            currentSpeed = 0f;
            pitch = 0f;
            yaw = 0f;
            roll = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (!vehicleControl || !vehicleInitialized)
                return;

            GetMouseInput();
            GetKeyInput();
            UpdateRotation();
            Move();
        }

        private void GetMouseInput()
        {
            roll = Input.GetAxis("Roll") * speedRoll * Time.deltaTime;
            pitch = Input.GetAxis("Vertical") * speedPitch * Time.deltaTime;
            yaw = Input.GetAxis("Horizontal") * speedYaw * Time.deltaTime;
        }

        private void GetKeyInput()
        {
            if (Input.GetButton("TakeOff"))
            {
                FoilWings();
            }

            if (Input.GetButton("Accelerate"))
            {
                currentSpeed += Input.GetAxis("Accelerate") * _acceleration * Time.deltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

            float fire = Input.GetAxisRaw("Fire3");

        }

        private void FoilWings()
        {
            _wings = vehicleControlled.gameObject.GetComponentsInChildren<FoiledWings>();
            if (_wings.Length > 0)
            {
                foreach (FoiledWings wing in _wings)
                {
                    wing.FoilWings();
                }
            }
        }

        private void UpdateRotation()
        {
            _vTransform.Rotate(pitch, yaw, roll);
        }

        private void Move()
        {
            _vTransform.position += _vTransform.forward * currentSpeed * Time.deltaTime;
        }

        public void GiveControlToPlayer(GameObject vehicle, int vehicleDatabaseId)
        {
            if (!vehicleInitialized)
            {
                InitShip(vehicleDatabaseId);
                vehicleInitialized = true;
            }

            vehicleControlled = vehicle;
            vehicleControl = true;
            _vTransform = vehicleControlled.transform;
            game.ShowVehicleUI();
        }
    }
}