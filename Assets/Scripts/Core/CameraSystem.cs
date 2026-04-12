using UnityEngine;
using TimesBaddestCat.Tests.Helpers;
using Cinemachine;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Camera System - Core layer system for following fast parkour movement.
    ///
    /// Implements ADR-0008: Camera System Implementation
    /// Uses Cinemachine 3.x with custom ImpulseListener for camera shake and FOV dynamics.
    /// </summary>
    public class CameraSystem : MonoBehaviour
    {
        #region Constants

        [Header("Camera Constants")]
        private const float DEFAULT_FOV = 75f;
        private const float ADS_FOV = 50f;
        private const float FOV_LERP_SPEED = 5f;
        private const float SHAKE_INTENSITY = 0.5f;
        private const float SHAKE_DURATION = 0.15f;

        #endregion

        #region Serialized Data

        [Header("Camera Configuration")]
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;
        [SerializeField]
        private CinemachineImpulseListener cameraImpulse;
        [SerializeField]
        private Transform playerTarget;
        [SerializeField]
        private Vector3 positionOffset;
        [SerializeField]
        private float currentFOV = DEFAULT_FOV;
        [SerializeField]
        private bool isADSMode = false;

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        private IMovementProvider movementSystem;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            // Find or create Cinemachine components
            if (virtualCamera == null)
            {
                CreateCinemachine();
            }
            else
            {
                virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
                cameraImpulse = FindObjectOfType<CinemachineImpulseListener>();
            }
        }

        protected virtual void Start()
        {
            CacheDependencies();

            // Set initial follow target
            if (playerTarget == null)
            {
                Debug.LogWarning("Player target not set! Camera will follow first player GameObject found.");
            }

            // Cinemachine setup
            virtualCamera.Follow = playerTarget;
            virtualCamera.LookAt = playerTarget;

            // Apply initial settings
            ApplyFOV(DEFAULT_FOV);
        }

        protected virtual void OnEnable()
        {
            EnableCinemachine();
        }

        protected virtual void OnDisable()
        {
            DisableCinemachine();
        }

        #endregion

        #region Cinemachine Management

        [Header("Cinemachine Management")]
        private void CreateCinemachine()
        {
            // Create Cinemachine Virtual Camera
            GameObject cameraObject = new GameObject("Main Virtual Camera");
            cameraObject.tag = "MainCamera";

            if (virtualCamera == null)
            {
                virtualCamera = cameraObject.AddComponent<CinemachineVirtualCamera>();
            }

            // Create Cinemachine Impulse Listener
            if (cameraImpulse == null)
            {
                GameObject impulseListener = new GameObject("Camera Impulse Listener");
                cameraImpulse = impulseListener.AddComponent<CinemachineImpulseListener>();

                cameraImpulse.AddReactionListener<CameraShake>();
                cameraImpulse.AddReactionListener<CameraFOV>();
            }
        }

        private void EnableCinemachine()
        {
            if (virtualCamera != null) virtualCamera.enabled = true;
            if (cameraImpulse != null) cameraImpulse.enabled = true;
        }

        private void DisableCinemachine()
        {
            if (virtualCamera != null) virtualCamera.enabled = false;
            if (cameraImpulse != null) cameraImpulse.enabled = false;
        }

        #endregion

        #region Camera Actions

        [Header("Camera Actions")]
        public void SetPlayerTarget(Transform target)
        {
            playerTarget = target;
            if (virtualCamera != null)
            {
                virtualCamera.Follow = target;
            }
        }

        public void AddCameraShake(float intensity)
        {
            if (cameraImpulse == null) return;

            // Apply shake with intensity from ADR-0008
            cameraImpulse.GenerateImpulse(SHAKE_INTENSITY * intensity, SHAKE_DURATION);
        }

        public void ApplyWeaponRecoil(float recoilAmount)
        {
            if (cameraImpulse == null) return;

            // Camera FOV kick on weapon fire
            float targetFOV = isADSMode ? ADS_FOV : DEFAULT_FOV;
            cameraImpulse.GenerateImpulse(Vector3.down, -recoilAmount, 0.05f);
        }

        public void EnableAimMode(bool enable)
        {
            isADSMode = enable;

            // Smooth FOV transition
            StartCoroutine(SmoothFOVTransition());
        }

        public void DisableAimMode()
        {
            isADSMode = false;
            StartCoroutine(SmoothFOVTransition());
        }

        public void SetPositionOffset(Vector3 offset)
        {
            positionOffset = offset;
        }

        #endregion

        #region FOV Management

        [Header("FOV Management")]
        private void ApplyFOV(float targetFOV)
        {
            currentFOV = targetFOV;
        }

        private IEnumerator SmoothFOVTransition()
        {
            float duration = 0.2f;
            float startFOV = currentFOV;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float currentFOV = Mathf.Lerp(startFOV, targetFOV, t);
                virtualCamera.m_Lens.FieldOfView = currentFOV;
                yield return null;
            }

            currentFOV = targetFOV;
        }

        #endregion

        #region Movement-Based FOV

        [Header("Speed-Based FOV")]
        private void Update()
        {
            // Check for player movement system
            if (movementSystem != null)
            {
                float speed = movementSystem.GetCurrentSpeed();

                // Expand FOV based on player speed
                if (speed > 0f)
                {
                    float speedMultiplier = Mathf.Clamp01(speed / 30f, 0.5f, 1.5f);
                    float targetFOV = DEFAULT_FOV + speedMultiplier * 10f;
                    ApplyFOV(targetFOV);
                }
                else
                {
                    ApplyFOV(DEFAULT_FOV);
                }

                // Update target if player exists
                if (playerTarget != null)
                {
                    virtualCamera.Follow = playerTarget;
                }
            }
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Visualization")]
        private void OnDrawGizmos()
        {
            // Draw camera position offset
            Gizmos.color = Color.magenta * 0.5f;
            if (positionOffset != Vector3.zero)
            {
                Gizmos.DrawWireSphere(transform.position + positionOffset, 0.5f);
            }

            // Draw aim direction
            if (playerTarget != null && movementSystem != null)
            {
                Vector3 aim = movementSystem.GetAimDirection();
                Gizmos.DrawLine(transform.position, transform.position + aim, Color.cyan * 0.5f, 0.5f);
            }
        }
        #endif
    }
}
