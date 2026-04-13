using System;
using System.Collections;
using UnityEngine;
using TimesBaddestCat.Tests.Helpers;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Movement System - Core layer system for parkour movement.
    ///
    /// Implements ADR-0003: Parkour Movement Implementation
    /// </summary>
    public class MovementSystem : MonoBehaviour, IMovementProvider
    {
        #region Constants

        [Header("Movement Constants")]
        private const float WALL_RUN_ATTACH_SPEED = 2f;
        private const float WALL_RUN_DETACH_SPEED = 1f;
        private const float WALL_RUN_ATTACH_DISTANCE = 0.1f;
        private const float WALL_RUN_MIN_DISTANCE = 0.5f;
        private const float DASH_FORCE = 800f;
        private const float DASH_DURATION = 0.15f;
        private const float CLIMB_SPEED = 3f;
        private const float CLIMB_SURFACE_DETACH_DISTANCE = 0.05f;
        private const float SLIDE_FRICTION_MULTIPLIER = 0.7f;
        private const float GRAVITY_MODIFIED = -30f;
        private const float MAX_PARKOUR_SPEED = 50f;
        private const float MAX_RAYCAST_DISTANCE = 10f;
    {
        #region Constants

        [Header("Movement Constants")]
        private const float WALL_RUN_ATTACH_SPEED = 2f;
        private const float WALL_RUN_DETACH_SPEED = 1f;
        private const float WALL_RUN_ATTACH_DISTANCE = 0.1f;
        private const float WALL_RUN_MIN_DISTANCE = 0.5f;
        private const float DASH_FORCE = 800f;
        private const float DASH_DURATION = 0.15f;
        private const float CLIMB_SPEED = 3f;
        private const float CLIMB_SURFACE_DETACH_DISTANCE = 0.05f;
        private const float SLIDE_FRICTION_MULTIPLIER = 0.7f;
        private const float GRAVITY_MODIFIED = -30f;
        private const float MAX_PARKOUR_SPEED = 50f;

        #endregion

        #region Serialized Data

        [Header("Movement State")]
        [SerializeField]
        private Rigidbody rb;
        [SerializeField]
        private CapsuleCollider capsuleCollider;

        [Header("Parkour State")]
        [SerializeField]
        private bool isWallRunning = false;
        [SerializeField]
        private bool isClimbing = false;
        [SerializeField]
        private bool isDashing = false;
        [SerializeField]
        private bool isSliding = false;
        [SerializeField]
        private bool isDashOnCooldown = false;
        [SerializeField]
        private Transform currentWallSurface;
        [SerializeField]
        private float currentSpeed = 0f;
        [SerializeField]
        private Vector2 moveInput = Vector2.zero;

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        private IInputProvider inputSystem;
        private IPhysicsProvider physicsSystem;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            InitializeRigidbody();
        }

        protected virtual void Start()
        {
            CacheDependencies();
        }

        protected virtual void OnEnable()
        {
            EnableMovement();
        }

        protected virtual void OnDisable()
        {
            DisableMovement();
        }

        #endregion

        #region Initialization

        [Header("Movement Initialization")]
        private void InitializeRigidbody()
        {
            rb = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            if (capsuleCollider == null)
            {
                capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            }

            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolationMode.Interpolate;
        }

        private void CacheDependencies()
        {
            inputSystem = FindObjectOfType<IInputProvider>();
            physicsSystem = FindObjectOfType<IPhysicsProvider>();

            if (inputSystem == null || physicsSystem == null)
            {
                Debug.LogWarning("Movement System missing dependencies!");
            }
        }

        #endregion

        #region Movement Control

        [Header("Movement Control")]
        private void EnableMovement()
        {
            // Enable input for movement
        }

        private void DisableMovement()
        {
            // Disable input for movement
        }

        #endregion

        #region Update Loop

        [Header("Update Loop")]
        private void Update()
        {
            ReadInput();
            HandleParkourActions();
            UpdateVelocity();
        }

        #endregion

        #region Input Reading

        [Header("Input Reading")]
        private void ReadInput()
        {
            // Get movement input from Input System
            Vector2 moveInput = inputSystem.GetMovementAxis();
            bool dashRequested = inputSystem.IsDashRequested();

            // Clear state if no input
            if (moveInput.magnitude < 0.01f && !isDashing && !isSliding && !isWallRunning)
            {
                StopParkour();
            }

            // Process dash input
            if (dashRequested && !isDashOnCooldown)
            {
                StartCoroutine(DashRoutine());
            }
        }

        #endregion

        #region Parkour Actions

        [Header("Parkour Actions")]
        private void HandleParkourActions()
        {
            // Wall running
            if (isWallRunning)
            {
                HandleWallRunning();
            }
            // Climbing
            else if (isClimbing)
            {
                HandleClimbing();
            }
            // Normal movement
            else
            {
                HandleNormalMovement(moveInput);
            }
        }

        private void StopParkour()
        {
            StopWallRunning();
            StopClimbing();
            StopDashing();
            isSliding = false;
        }

        #endregion

        #region Wall Running

        [Header("Wall Running")]
        private void TryStartWallRun()
        {
            Vector3 direction = transform.forward;

            // Check if wall is ahead and traversable
            if (physicsSystem.CanWallRun(transform.position, direction))
            {
                StartCoroutine(AttachToWallRoutine());
            }
        }

        private IEnumerator AttachToWallRoutine()
        {
            // Move to wall surface
            Vector3 wallPoint = physicsSystem.GetWallRunAttachmentPoint(transform.position, transform.forward, MAX_RAYCAST_DISTANCE);

            // Animate to wall (smooth transition)
            for (float t = 0f; t < WALL_RUN_ATTACH_SPEED * 0.5f; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, wallPoint, t / (WALL_RUN_ATTACH_SPEED * 0.5f));
                yield return null;
            }

            isWallRunning = true;
            if (physicsSystem != null)
            {
                currentWallSurface = physicsSystem.GetWallRunAttachmentPoint(wallPoint, -transform.forward);
            }
        }

        private void HandleWallRunning()
        {
            if (!isWallRunning) return;

            // Check if still on wall
            Vector3 toWall = currentWallSurface - transform.position;

            // Detach if at edge or input stops moving along wall
            if (toWall.magnitude < WALL_RUN_MIN_DISTANCE || toWall.magnitude > MAX_RAYCAST_DISTANCE)
            {
                StartCoroutine(DetachFromWallRoutine());
            }
        }

        private IEnumerator DetachFromWallRoutine()
        {
            Vector3 toWall = currentWallSurface - transform.position;

            // Smooth transition off wall
            for (float t = 0f; t < WALL_RUN_DETACH_SPEED * 0.5f; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, currentWallSurface + toWall, t / (WALL_RUN_DETACH_SPEED * 0.5f));
                yield return null;
            }

            isWallRunning = false;
        }

        #endregion

        #region Climbing

        [Header("Climbing")]
        private void TryStartClimb()
        {
            // Check if traversable surface above
            if (physicsSystem.CanClimb(transform.position))
            {
                isClimbing = true;
                ApplyClimbPhysics();
            }
        }

        private void HandleClimbing()
        {
            if (!isClimbing) return;

            Vector3 climbDirection = transform.up;
            Vector3 targetPosition = transform.position + climbDirection * CLIMB_SURFACE_DETACH_DISTANCE;

            // Smooth movement toward surface
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance > 0.01f)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, CLIMB_SPEED * Time.deltaTime);
            }
        }

        private void StopClimbing()
        {
            ApplyNormalGravity();
            isClimbing = false;
        }

        private void ApplyClimbPhysics()
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        #endregion

        #region Dashing

        [Header("Dashing")]
        private IEnumerator DashRoutine()
        {
            isDashing = true;
            isSliding = false;
            StopWallRunning();
            StopClimbing();

            // Apply dash force
            Vector2 moveInput = inputSystem.GetMovementAxis().normalized;
            Vector3 dashDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            rb.velocity = dashDirection * DASH_FORCE;

            // Apply slide friction during dash
            Vector3 groundNormal = physicsSystem.IsGrounded(transform.position) ? Vector3.up : Vector3.zero;
            rb.drag = SLIDE_FRICTION_MULTIPLIER;

            yield return new WaitForSeconds(DASH_DURATION);

            // End dash
            isDashing = false;
            rb.drag = 0f;
        }

        #endregion

        #region Normal Movement

        [Header("Normal Movement")]
        private void HandleNormalMovement(Vector2 moveInput)
        {
            // Get camera forward direction
            Vector3 forward = Camera.main != null ? Camera.main.transform.forward : transform.forward;
            Vector3 right = Camera.main != null ? Camera.main.transform.right : transform.right;

            // Calculate movement direction relative to camera
            Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);

            // Apply movement
            rb.velocity = moveDirection * MAX_PARKOUR_SPEED;
            currentSpeed = rb.velocity.magnitude;

            // Validate speed within limits
            GameAssert.VelocityWithinParkourLimits(rb.velocity, MAX_PARKOUR_SPEED);
        }

        #endregion

        #region Physics Modifiers

        [Header("Physics Modifiers")]
        private void ApplyNormalGravity()
        {
            rb.useGravity = true;
        }

        private void ApplyReducedFriction(float multiplier)
        {
            rb.drag = multiplier;
        }

        private void ApplyClimbPhysics()
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }

        #endregion

        #region Public Interface

        [Header("Public Interface")]
        public Vector3 GetVelocity()
        {
            return rb.velocity;
        }

        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        public bool IsParkourStateActive()
        {
            return isWallRunning || isClimbing || isDashing || isSliding;
        }

        public bool IsWallRunning() => isWallRunning;
        public bool IsClimbing() => isClimbing;
        public bool IsDashing() => isDashing;
        public bool IsSliding() => isSliding;

        // IMovementProvider additional methods
        void IMovementProvider.SetMovementMode(MovementMode mode)
        {
            // Could switch between movement modes here
        }

        Vector3 IMovementProvider.GetAimDirection()
        {
            if (inputSystem != null)
            {
                Vector2 aim = inputSystem.GetAimDirection();
                return new Vector3(aim.x, 0f, aim.y).normalized;
            }
            return Vector3.forward;
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Visualization")]
        private void OnDrawGizmos()
        {
            // Visualize velocity
            Gizmos.color = Color.yellow * 0.7f;
            Gizmos.DrawLine(transform.position, transform.position + rb.velocity * 0.1f, Gizmos.color);

            // Visualize wall attachment point
            if (isWallRunning && currentWallSurface != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(currentWallSurface, 0.1f, 8);
            }
        }
        #endif
    }
}
