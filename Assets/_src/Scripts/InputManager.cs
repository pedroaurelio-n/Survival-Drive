using UnityEngine;
using UnityEngine.InputSystem;

namespace PedroAurelio.SurvivalDrive
{
    public class InputManager : MonoBehaviour
    {
        private CarController _controller;

        private PlayerControls _controls;

        private void Awake()
        {
            _controller = GetComponent<CarController>();
        }

        private void OnEnable()
        {
            if (_controls == null)
                _controls = new PlayerControls();

            _controls.Gameplay.Move.performed += _controller.SetSteeringInput;

            _controls.Gameplay.Accelerate.performed += _controller.SetAccelerateInput;
            _controls.Gameplay.Accelerate.canceled += _controller.SetAccelerateInput;

            _controls.Gameplay.Brake.performed += _controller.SetBrakeInput;
            _controls.Gameplay.Brake.canceled += _controller.SetBrakeInput;

            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Gameplay.Move.performed -= _controller.SetSteeringInput;

            _controls.Gameplay.Accelerate.performed -= _controller.SetAccelerateInput;
            _controls.Gameplay.Accelerate.canceled -= _controller.SetAccelerateInput;

            _controls.Gameplay.Brake.performed -= _controller.SetBrakeInput;
            _controls.Gameplay.Brake.canceled -= _controller.SetBrakeInput;

            _controls.Disable();
        }
    }
}
