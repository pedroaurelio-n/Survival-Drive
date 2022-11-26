using UnityEngine;
using UnityEngine.InputSystem;

namespace PedroAurelio.SurvivalDrive
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Steering Settings")]
        [SerializeField] private float steeringSpeed;
        [SerializeField] private float steeringPosAccel;
        [SerializeField] private float steeringNegAccel;

        [Header("Move Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float maxPosSpeed;
        [SerializeField] private float maxNegSpeed;
        [SerializeField] private float posAccel;
        [SerializeField] private float negAccel;

        private float _steeringInput;
        private Vector2 _moveInput;

        private Rigidbody _rigidbody;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            Move();
            Rotate();
        }

        private void Move()
        {
            var moveInput = _moveInput.x + _moveInput.y;
            
            Vector2 targetSpeed;
            float acceleration;

            if (moveInput != 0f)
            {
                targetSpeed = moveInput > 0f ? moveSpeed * transform.up : moveSpeed * transform.up * moveInput;
                acceleration = posAccel;
            }
            else
            {
                targetSpeed = -(_rigidbody.velocity);
                acceleration = negAccel;
            }

            _rigidbody.AddForce(acceleration * targetSpeed, ForceMode.Acceleration);

            var maxSpeed = moveInput >= 0f ? maxPosSpeed : maxNegSpeed;
            var clampedVelocity = Vector2.ClampMagnitude(_rigidbody.velocity, maxSpeed);
            _rigidbody.velocity = clampedVelocity;
        }

        private void Rotate()
        {
            if (_steeringInput != 0f)
            {
                var steering = Mathf.MoveTowards(_rigidbody.angularVelocity.z, steeringSpeed * _steeringInput, steeringPosAccel);
                var newRotation = new Vector3(0f, 0f, steering);
                _rigidbody.angularVelocity = newRotation;
            }
            else
            {
                var steering = Mathf.MoveTowards(_rigidbody.angularVelocity.z, 0f, steeringNegAccel);
                var newRotation = new Vector3(0f, 0f, steering);
                _rigidbody.angularVelocity = newRotation;
            }
        }
        
        public void SetSteeringInput(InputAction.CallbackContext ctx)
        {
            _steeringInput = -(ctx.ReadValue<Vector2>().x);
        }

        public void SetAccelerateInput(InputAction.CallbackContext ctx)
        {
            _moveInput.x = ctx.ReadValue<float>();
        }

        public void SetBrakeInput(InputAction.CallbackContext ctx)
        {
            _moveInput.y = -(ctx.ReadValue<float>());
        }
    }
}
