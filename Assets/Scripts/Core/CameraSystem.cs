using System;
using System.Collections;
using UnityEngine;
using TimesBaddestCat.Tests.Helpers;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Camera System - Core layer system for following fast parkour movement.
    ///
    /// Implements ADR-0008: Camera System Implementation
    /// Uses Unity's camera system with FOV dynamics and smooth following.
    /// </summary>
    public class CameraSystem : MonoBehaviour, ICameraProvider
    {
        #region Constants

        [Header("Camera Constants")]
        private const float DEFAULT_FOV = 75f;
        private const float ADS_FOV = 50f;
        private const float FOV_LERP_SPEED = 5f;
        private const float SHAKE_INTENSITY = 0.5f;
        private const float SHAKE_DURATION = 0.15f;
        private const float FOLLOW_SMOOTH_SPEED = 10f;
        private const float LOOK_SMOOTH_SPEED = 5f;

        #endregion

        #region Serialized Data

        [Header("Camera Configuration")]
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Transform playerTarget;
        [SerializeField]
        private Vector3 positionOffset = new Vector3(0f, 2f, -5f);
        [SerializeField]
        private Vector2 lookSensitivity = new Vector2(2f, 2f);
        [SerializeField]
        private float maxLookUp = 80f;
        [SerializeField]
        private float maxLookDown = 80f;

        [Header("Camera State")]
        [SerializeField]
        private float currentFOV = DEFAULT_FOV;
        [SerializeField]
        private float targetFOV = DEFAULT_FOV;
        [SerializeField]
        private bool isADSMode = false;
        [SerializeField]
        private Vector2 currentRotation = Vector2.zero;
        [SerializeField]
        private float shakeIntensity = 0f;
        [SerializeField]
        private float shakeTimer = 0f;
        [SerializeField]
        private Vector3 shakeOffset = Vector3.zero;

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        private IMovementProvider movementSystem;
        private IInputProvider inputSystem;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            InitializeCamera();
        }

        protected virtual void Start()
        {
            CacheDependencies();
            ApplyFOV(DEFAULT_FOV);
        }

        protected virtual void OnEnable()
        {
            EnableCamera();
        }

        protected virtual void OnDisable()
        {
            DisableCamera();
        }

        protected virtual void LateUpdate()
        {
            UpdateCameraFollow();
            UpdateCameraLook();
            UpdateCameraShake();
            UpdateFOV();
        }

        #endregion

        #region Initialization

        [Header("Camera Initialization")]
        private void InitializeCamera()
        {
            // Find or create main camera
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    GameObject cameraObj = new GameObject("Main Camera");
                    cameraObj.tag = "MainCamera";
                    mainCamera = cameraObj.AddComponent<Camera>();
                    AudioListener listener = cameraObj.AddComponent<AudioListener>();
                }
            }

            // Set initial position
            transform.position = positionOffset;
        }

        private void CacheDependencies()
        {
            movementSystem = FindObjectOfType<IMovementProvider>();
            inputSystem = FindObjectOfType<IInputProvider>();

            if (movementSystem == null || inputSystem == null)
            {
                Debug.LogWarning("Camera System missing some dependencies!");
            }
        }

        #endregion

        #region Camera Management

        [Header("Camera Management")]
        private void EnableCamera()
        {
            if (mainCamera != null) mainCamera.enabled = true;
        }

        private void DisableCamera()
        {
            if (mainCamera != null) mainCamera.enabled = false;
        }

        #endregion

        #region Camera Actions

        [Header("Camera Actions")]
        public void SetPlayerTarget(Transform target)
        {
            playerTarget = target;
        }

        public void SetAimDirection(Vector2 direction)
        {
            // This would be used for aim assist or other systems
        }

        public void AddCameraShake(float intensity)
        {
            shakeIntensity = SHAKE_INTENSITY * intensity;
            shakeTimer = SHAKE_DURATION;
        }

        public void EnableAimMode(bool enable)
        {
            isADSMode = enable;
            targetFOV = isADSMode ? ADS_FOV : DEFAULT_FOV;
        }

        public void DisableAimMode()
        {
            isADSMode = false;
            targetFOV = DEFAULT_FOV;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            positionOffset = offset;
        }

        #endregion

        #region Camera Updates

        [Header("Camera Updates")]
        private void UpdateCameraFollow()
        {
            if (playerTarget == null) return;

            // Calculate target position
            Vector3 targetPosition = playerTarget.position + positionOffset + shakeOffset;

            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, targetPosition, FOLLOW_SMOOTH_SPEED * Time.deltaTime);
        }

        private void UpdateCameraLook()
        {
            if (inputSystem == null) return;

            // Get aim input
            Vector2 aimInput = inputSystem.GetAimDirection();

            // Apply rotation
            currentRotation.x += aimInput.x * lookSensitivity.x * Time.deltaTime;
            currentRotation.y -= aimInput.y * lookSensitivity.y * Time.deltaTime;

            // Clamp vertical rotation
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxLookDown, maxLookUp);

            // Apply rotation to camera
            transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);
        }

        private void UpdateCameraShake()
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;

                // Generate random shake offset
                shakeOffset = new Vector3(
                    UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                    UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                    UnityEngine.Random.Range(-shakeIntensity, shakeIntensity)
                ) * (shakeTimer / SHAKE_DURATION);

                if (shakeTimer <= 0f)
                {
                    shakeOffset = Vector3.zero;
                }
            }
        }

        #endregion

        #region FOV Management

        [Header("FOV Management")]
        private void ApplyFOV(float fov)
        {
            currentFOV = fov;
            if (mainCamera != null)
            {
                mainCamera.fieldOfView = fov;
            }
        }

        private void UpdateFOV()
        {
            // Lerp current FOV to target
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, FOV_LERP_SPEED * Time.deltaTime);

            // Apply to camera
            if (mainCamera != null)
            {
                mainCamera.fieldOfView = currentFOV;
            }

            // Speed-based FOV adjustment
            if (movementSystem != null && !isADSMode)
            {
                float speed = movementSystem.GetCurrentSpeed();
                if (speed > 0f)
                {
                    float speedMultiplier = Mathf.Clamp01(speed / 30f);
                    float speedFOV = DEFAULT_FOV + speedMultiplier * 10f;
                    targetFOV = Mathf.Max(targetFOV, speedFOV);
                }
            }
        }

        #endregion

        #region Public API

        public Camera GetCamera() => mainCamera;
        public float GetCurrentFOV() => currentFOV;
        public Vector3 GetForwardDirection() => transform.forward;
        public Vector3 GetRightDirection() => transform.right;

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Visualization")]
        private void OnDrawGizmos()
        {
            // Draw camera position offset
            Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
            if (positionOffset != Vector3.zero)
            {
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }

            // Draw aim direction
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
        }
        #endif
    }
}
