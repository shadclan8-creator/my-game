using UnityEngine;
using TimesBaddestCat.Foundation;
using TimesBaddestCat.Core;
using TimesBaddestCat.Presentation;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// Player Controller - Connects all foundation systems for Time's Baddest Cat.
    /// Orchestrates Input, Movement, Combat, Camera, Combo, and Enemy AI.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Dependencies

        private IInputProvider inputSystem;
        private IMovementProvider movementSystem;
        private IPhysicsProvider physicsSystem;
        private ICombatProvider combatSystem;
        private ICameraProvider cameraSystem;
        private IComboProvider comboSystem;
        private IEnemyAIProvider enemySystem;

        #endregion

        #region Serialized Data

        [Header("Player Configuration")]
        [SerializeField]
        private float maxHealth = 100f;
        private float invulnerabilityTime = 0.5f;
        [SerializeField]
        private float invulnerabilityDuration = 2f;

        [Header("Movement Settings")]
        [SerializeField]
        private float walkSpeed = 10f;
        [SerializeField]
        private float runSpeed = 20f;

        [SerializeField]
        private float jumpForce = 15f;

        #endregion

        #region Player State

        [Header("Player State")]
        private float currentHealth;
        [SerializeField]
        private bool isInvulnerable;
        private bool isDead;

        #endregion

        #region Events

        [Header("Player Events")]
        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDeath;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            CacheDependencies();
            SetInitialHealth();
            SetInvulnerability(false);
        }

        protected virtual void Start()
        {
            // Find player spawn point
            // For MVP testing, spawn at origin
            transform.position = Vector3.zero;
        }

        protected virtual void Update()
        {
            if (isDead) return;

            HandleMovement();
            HandleCombat();
            UpdateInvulnerability();
        }

        protected virtual void OnDisable()
        {
            // Clean up
        }

        #endregion

        #region Initialization

        [Header("Initialization")]
        private void CacheDependencies()
        {
            inputSystem = FindObjectOfType<IInputProvider>();
            movementSystem = FindObjectOfType<IMovementProvider>();
            physicsSystem = FindObjectOfType<IPhysicsProvider>();
            combatSystem = FindObjectOfType<ICombatProvider>();
            cameraSystem = FindObjectOfType<ICameraProvider>();
            comboSystem = FindObjectOfType<IComboProvider>();
            enemySystem = FindObjectOfType<IEnemyAIProvider>();

            if (inputSystem == null || movementSystem == null || physicsSystem == null ||
                combatSystem == null || cameraSystem == null || comboSystem == null || enemySystem == null)
            {
                Debug.LogWarning("Player Controller missing dependencies!");
            }
        }

        private void SetInitialHealth()
        {
            currentHealth = maxHealth;
        }

        private void SetInvulnerability(bool invulnerable)
        {
            isInvulnerable = invulnerable;
        }

        #endregion

        #region Health Management

        [Header("Health Management")]
        public void TakeDamage(float damage, Vector3 hitPosition, bool isHeadshot)
        {
            if (isInvulnerable) return;

            float finalDamage = damage;
            if (isHeadshot) finalDamage *= 2f;

            currentHealth -= finalDamage;
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }

            // Spawn damage number
            if (combatSystem != null)
            {
                combatSystem.SpawnDamageNumber(hitPosition, (int)finalDamage);
            }
        }

        public void Die()
        {
            isDead = true;
            OnPlayerDeath?.Invoke();

            // Reset combo on death
            if (comboSystem != null)
            {
                comboSystem.ResetCombo();
            }

            // Debug log
            Debug.Log("Player died!");
        }

        public void Respawn()
        {
            isDead = false;
            SetInitialHealth();
            SetInvulnerability(false);
            transform.position = Vector3.zero;
        }

        private void UpdateInvulnerability()
        {
            // Check damage taken recently
            // In full implementation, this would track last hit time
        }

        #endregion

        #region Movement Control

        [Header("Movement Control")]
        private void HandleMovement()
        {
            if (isDead) return;

            // Get movement input
            Vector2 moveAxis = inputSystem.GetMovementAxis();

            // Get movement mode
            var movementMode = inputSystem.GetMovementMode();

            switch (movementMode)
            {
                case MovementMode.Normal:
                    HandleNormalMovement(moveAxis);
                    break;
                case MovementMode.Combat:
                    HandleCombatMovement(moveAxis);
                    break;
            }

            // Apply movement through Movement System
            if (movementSystem != null)
            {
                movementSystem.SetMovementMode(MovementMode.Normal);
            }
        }

        private void HandleNormalMovement(Vector2 moveAxis)
        {
            bool dashRequested = inputSystem.IsDashRequested();
            Vector3 forward = transform.forward;

            // Apply walk/run speed
            float speed = dashRequested ? runSpeed : walkSpeed;

            Vector3 movement = (forward * moveAxis.y) + (transform.right * moveAxis.x);
            transform.position += movement * speed * Time.deltaTime;

            // Update speed for combo system
            if (comboSystem != null)
            {
                comboSystem.OnMovementSustained();
            }
        }

        private void HandleCombatMovement(Vector2 moveAxis)
        {
            // In combat, movement is more restricted
            // Get aim direction
            Vector3 aimDirection = inputSystem.GetAimDirection();

            // Move toward aim position (for strafing)
            Vector3 movement = (transform.right * aimDirection.x) + (transform.forward * aimDirection.y);
            transform.position += movement * walkSpeed * Time.deltaTime;

            // Aim controls through camera
            if (cameraSystem != null)
            {
                cameraSystem.SetAimDirection(aimDirection);
            }
        }

        #endregion

        #region Combat Control

        [Header("Combat Control")]
        private void HandleCombat()
        {
            if (isDead) return;

            bool fireRequested = inputSystem.IsFireRequested();
            bool reloadRequested = inputSystem.IsReloadRequested();

            if (fireRequested)
            {
                if (combatSystem != null)
                {
                    combatSystem.FireWeapon();
                }
            }

            if (reloadRequested)
            {
                if (combatSystem != null)
                {
                    combatSystem.ReloadWeapon();
                }
            }
        }

        #endregion

        #region Camera Integration

        [Header("Camera Integration")]
        private void UpdateCamera()
        {
            // Follow player position
            if (cameraSystem != null)
            {
                cameraSystem.SetPlayerTarget(transform);
            }
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Health: {currentHealth:F1}/{maxHealth:F1}");
            GUILayout.Label($"Invulnerable: {(isInvulnerable ? "YES" : "NO")}");
            GUILayout.Label($"Movement Mode: {inputSystem.GetMovementMode()}");
            GUILayout.Label($"Combo: x{comboSystem.GetCurrentCombo()}");
        }
        #endif
    }
}
