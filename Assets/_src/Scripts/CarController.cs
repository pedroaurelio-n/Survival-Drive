using UnityEngine;
using UnityEngine.InputSystem;

namespace PedroAurelio.SurvivalDrive
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private Transform carMesh;
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float backwardsSpeed;
        [SerializeField] private float steeringSpeed;
        [SerializeField] private float steerVelocityWeight;
        [SerializeField] private float groundDrag;
        [SerializeField] private float airDrag;

        [Header("Grounded Settings")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector3 raycastOffset;
        [SerializeField] private float raycastDistance;

        private float _steeringInput;
        private Vector2 _carInput;

        private float _turnSpeed;
        private float _moveInput;

        private bool _isGrounded;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            CheckGround();
            Accelerate();
            Turn();
        }

        private void CheckGround()
        {
            _isGrounded = false;
            _rigidbody.drag = airDrag;

            if (Physics.Raycast(transform.position + raycastOffset, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                _isGrounded = true;
                _rigidbody.drag = groundDrag;

                // var rotationDirection = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                // var newRotation = Quaternion.RotateTowards(transform.rotation, rotationDirection, rotationSpeed);
                // transform.rotation = newRotation;
            }
        }

        private void Accelerate()
        {
            if (!_isGrounded)
                return;
            
            _moveInput = _carInput.x + _carInput.y;
            Vector3 targetSpeed;
            
            if (_moveInput > 0f)
                targetSpeed = _moveInput * forwardSpeed * transform.forward;
            else if (_moveInput < 0f)
                targetSpeed = _moveInput * backwardsSpeed * transform.forward;
            else
                targetSpeed = -(_rigidbody.velocity);
                
            _rigidbody.AddForce(targetSpeed, ForceMode.Acceleration);
        }

        private void Turn()
        {
            var targetTurn = steeringSpeed * _steeringInput;
            targetTurn *= _rigidbody.velocity.sqrMagnitude * steerVelocityWeight;
            targetTurn = Mathf.Clamp(targetTurn, -(steeringSpeed * Mathf.Abs(_steeringInput)), steeringSpeed * Mathf.Abs(_steeringInput));

            if (_moveInput >= 0f)
                transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, targetTurn, 0f));
            else
                transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, -targetTurn, 0f));
        }
        
        public void SetSteeringInput(InputAction.CallbackContext ctx)
        {
            _steeringInput = ctx.ReadValue<Vector2>().x;
        }

        public void SetAccelerateInput(InputAction.CallbackContext ctx)
        {
            _carInput.x = ctx.ReadValue<float>();
        }

        public void SetBrakeInput(InputAction.CallbackContext ctx)
        {
            _carInput.y = -(ctx.ReadValue<float>());
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + raycastOffset, Vector3.down * raycastDistance);
        }

        // [SerializeField] private float maxSteeringAngle = 30f;
        // [SerializeField] private float motorForce = 50f;
        // [SerializeField] private bool FWD = true;
        // [SerializeField] private bool RWD = false;

        // [SerializeField] private WheelCollider FrontLeftWheel;
        // [SerializeField] private WheelCollider FrontRightWheel;
        // [SerializeField] private WheelCollider BackLeftWheel;
        // [SerializeField] private WheelCollider BackRightWheel;

        // [SerializeField] private Transform FrontLeftTransform;
        // [SerializeField] private Transform FrontRightTransform;
        // [SerializeField] private Transform BackLeftTransform;
        // [SerializeField] private Transform BackRightTransform;

        // private float _steeringInput;
        // private float _steeringAngle;
        // private Vector2 _moveInput;

        // private void FixedUpdate()
        // {
        //     Steer();
        //     Accelerate();
        //     UpdateWheelPoses();
        // }

        // private void Steer()
        // {
        //     _steeringAngle = maxSteeringAngle * _steeringInput;

        //     FrontLeftWheel.steerAngle = _steeringAngle;
        //     FrontRightWheel.steerAngle = _steeringAngle;
        // }

        // private void Accelerate()
        // {
        //     var moveInput = _moveInput.x + _moveInput.y;

        //     if (FWD)
        //     {
        //         FrontLeftWheel.motorTorque = moveInput * motorForce;
        //         FrontRightWheel.motorTorque = moveInput * motorForce;
        //     }

        //     if (RWD)
        //     {
        //         BackLeftWheel.motorTorque = moveInput * motorForce;
        //         BackRightWheel.motorTorque = moveInput * motorForce;
        //     }
        // }

        // private void UpdateWheelPoses()
        // {
        //     UpdateWheelPose(FrontLeftWheel, FrontLeftTransform);
        //     UpdateWheelPose(FrontRightWheel, FrontRightTransform);
        //     UpdateWheelPose(BackLeftWheel, BackLeftTransform);
        //     UpdateWheelPose(BackRightWheel, BackRightTransform);
        // }

        // private void UpdateWheelPose(WheelCollider wheelCollider, Transform transform)
        // {
        //     var position = transform.position;
        //     var rotation = transform.rotation;

        //     wheelCollider.GetWorldPose(out position, out rotation);

        //     transform.position = position;
        //     transform.rotation = rotation;
        // }
    }
}
