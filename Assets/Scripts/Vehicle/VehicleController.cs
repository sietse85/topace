using System.Collections;
using Client;
using Resource;
using Scriptable;
using UI;
using UnityEngine;

namespace Vehicle
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
        private FoiledWings[] _wings;
        private TurretSlot[] turrets;
        private bool _canFoil;
        public bool isLandVehicle;

        // Start is called before the first frame update
        void InitVehicle(int vehicleDatabaseId, GameObject vehicle)
        {
            VehicleScriptable V = Loader.instance.vehicles[vehicleDatabaseId];
            vehicleControlled = vehicle;
            maxSpeed = V.maximumSpeed;
            speedYaw = V.yawSpeed;
            speedPitch = V.pitchSpeed;
            speedRoll = V.rollSpeed;
            _acceleration = V.acceleration;
            currentSpeed = 0f;
            pitch = 0f;
            yaw = 0f;
            roll = 0f;
            turrets = vehicleControlled.GetComponentsInChildren<TurretSlot>();
            isLandVehicle = Loader.instance.vehicles[vehicleDatabaseId].isLandVehicle;

            _canFoil = true;
            StartCoroutine(FoilWings());
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!vehicleControl || !vehicleInitialized)
                return;

            if (!isLandVehicle)
            {
                GetMouseInput();
                GetKeyInput();
                UpdateRotation();
                Move();
            }
        }

        private void GetMouseInput()
        {
            roll = Input.GetAxis("Roll") * speedRoll * Time.deltaTime;
            pitch = Input.GetAxis("Vertical") * speedPitch * Time.deltaTime;
            yaw = Input.GetAxis("Horizontal") * speedYaw * Time.deltaTime;
        }

        private void GetKeyInput()
        {
            if (Input.GetButton("Accelerate"))
            {
                currentSpeed += Input.GetAxis("Accelerate") * _acceleration * Time.deltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

            float fire = Input.GetAxisRaw("Fire3");

            if (fire > 0)
            {
                foreach (TurretSlot slot in turrets)
                {
                    slot.Fire(Vector3.zero);
                }
            }
        }

        private IEnumerator FoilWings()
        {
            while (true)
            {
                if (Input.GetButton("TakeOff"))
                {


                    if (_canFoil)
                    {
                        _canFoil = false;
                        _wings = vehicleControlled.gameObject.GetComponentsInChildren<FoiledWings>();
                        if (_wings.Length > 0)
                        {
                            foreach (FoiledWings wing in _wings)
                            {
                                wing.FoilWings();
                            }
                        }
                    }
                    

                }
                
                yield return new WaitForSeconds(1f);
                _canFoil = true;
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

        public void GiveControlToPlayer(GameObject vehicle, int vehicleDatabaseId, byte vehicleId)
        {
            if (!vehicleInitialized)
            {
                InitVehicle(vehicleDatabaseId, vehicle);
                vehicleInitialized = true;
            }

            vehicleControl = true;
            _vTransform = vehicleControlled.transform;
            UIManager.instance.ShowVehicleUI(vehicleDatabaseId);
            UIManager.instance.SetVehicleIdToMonitorInUI(vehicleId);

            Transform camslot = vehicle.gameObject.transform.Find("cockpit").transform.Find("cameraslot");
            GameObject mainCam = GameObject.Find("MainCam");
            mainCam.transform.SetParent(camslot);
            mainCam.gameObject.transform.position = camslot.position;
            mainCam.gameObject.transform.rotation = camslot.rotation;
        }
    }
}