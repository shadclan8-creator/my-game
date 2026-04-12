using UnityEngine;
using UnityEngine.InputSystem;

namespace TimesBaddestCat.Foundation
{
    /// <summary>
    /// Input System - Foundation layer system for keyboard/mouse and gamepad input.
    ///
    /// Implements ADR-0001: Input System Architecture
    /// </summary>
    public class InputSystem : MonoBehaviour
    {
        #region Constants

        private const float DASH_COOLDOWN = 0.2f;
        private const float WEAPON_SWITCH_COOLDOWN = 0.3f;

        #endregion

        #region Input Actions

        [Header("Input Actions")]
        private InputAction moveAction;
        private InputAction dashAction;
        private InputAction fireAction;
        private InputAction reloadAction;
        private InputAction aimAction;

        [Header("Input Vectors")]
        private Vector2 movementInput;
        private Vector2 aimInput;

        #endregion

        #region State

        [Header("Input State")]
        private bool isDashRequested = false;
        private bool isDashOnCooldown = false;
        private float dashCooldownTimer = 0f;
        private bool isReloading = false;

        [Header("Device Type")]
        private DeviceType currentDevice = DeviceType.KeyboardMouse;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            InitializeInputActions();
        }

        protected virtual void OnEnable()
        {
            EnableInputActions();
        }

        protected virtual void OnDisable()
        {
            DisableInputActions();
        }

        #endregion

        #region Initialization

        [Header("Input Initialization")]
        private void InitializeInputActions()
        {
            // Movement
            moveAction = new InputAction("Move", InputActionType.Value);
            dashAction = new InputAction("Dash", InputActionType.Button);

            // Combat
            fireAction = new InputAction("Fire", InputActionType.Button);
            reloadAction = new InputAction("Reload", InputActionType.Button);

            // Camera/Menu
            aimAction = new InputAction("Aim", InputActionType.Value);
        }

        private void EnableInputActions()
        {
            moveAction.Enable();
            dashAction.Enable();
            fireAction.Enable();
            reloadAction.Enable();
            aimAction.Enable();
        }

        private void DisableInputActions()
        {
            moveAction.Disable();
            dashAction.Disable();
            fireAction.Disable();
            reloadAction.Disable();
            aimAction.Disable();
        }

        #endregion

        #region Input Reading

        [Header("Input Reading")]
        public Vector2 GetMovementAxis()
        {
            // Normalized movement input
            movementInput = moveAction.ReadValue<Vector2>();

            // Ensure magnitude <= 1.0 for frame budget
            if (movementInput.magnitude > 1.0001f)
            {
                movementInput = movementInput.normalized;
            }

            return movementInput;
        }

        public bool IsDashRequested()
        {
            bool inputDown = dashAction.WasPressedThisFrame();

            // Update cooldown state
            if (isDashOnCooldown)
            {
                inputDown = false;
            }

            return inputDown;
        }

        public bool IsFireRequested()
        {
            return fireAction.WasPressedThisFrame();
        }

        public bool IsReloadRequested()
        {
            return reloadAction.WasPressedThisFrame();
        }

        public Vector2 GetAimDirection()
        {
            // Normalized aim input (for 3D camera)
            aimInput = aimAction.ReadValue<Vector2>();

            if (aimInput.magnitude > 1.0001f)
            {
                aimInput = aimInput.normalized;
            }

            return aimInput;
        }

        #endregion

        #region State Management

        [Header("State Management")]
        public void SetMovementMode(MovementMode mode)
        {
            // Movement mode for different gameplay states
            // Implementation depends on Movement System needs
        }

        public void TogglePause()
        {
            // Pause menu navigation
            // Implementation depends on UI System needs
        }

        public void SwitchDevice()
        {
            // Automatic device switching
            currentDevice = currentDevice == DeviceType.KeyboardMouse ? DeviceType.Gamepad : DeviceType.KeyboardMouse;
            Debug.Log($"Device switched to: {currentDevice}");
        }

        #endregion

        #region Update Loop

        [Header("Update Loop")]
        private void Update()
        {
            UpdateDashCooldown();
        }

        private void UpdateDashCooldown()
        {
            if (isDashOnCooldown)
            {
                dashCooldownTimer -= Time.deltaTime;

                if (dashCooldownTimer <= 0f)
                {
                    isDashOnCooldown = false;
                }
            }
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Device: {currentDevice}");
            GUILayout.Label($"Movement: {GetMovementAxis()}");
            GUILayout.Label($"Dash: {IsDashRequested()} (Cooldown: {dashCooldownTimer:F2})");
            GUILayout.Label($"Fire: {IsFireRequested()}");
            GUILayout.Label($"Aim: {GetAimDirection()}");
        }
        #endif
    }

    #region Enums

    [Header("Device Type")]
    public enum DeviceType
    {
        KeyboardMouse,
        Gamepad
    }

    [Header("Movement Mode")]
    public enum MovementMode
    {
        Normal,
        Combat,
        Cinematic
    }

    #endregion

    #region Events

    /// <summary>
    /// Event for device connection/disconnection.
    /// </summary>
    public class DeviceEvent : GameEventBase
    {
        public DeviceType device;
    }

    #endregion
}
