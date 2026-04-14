using UnityEngine;
using TimesBaddestCat.Foundation;
using TimesBaddestCat.Core;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// GameplayIntegrator - Connects all systems for full gameplay.
    /// Handles combat interactions, damage dealing, scoring, and game flow.
    /// </summary>
    public class GameplayIntegrator : MonoBehaviour
    {
        #region Singleton

        private static GameplayIntegrator _instance;
        public static GameplayIntegrator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameplayIntegrator>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameplayIntegrator");
                        _instance = go.AddComponent<GameplayIntegrator>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region References

        [Header("System References")]
        private PlayerController playerController;
        private MovementSystem movementSystem;
        private CombatSystem combatSystem;
        private CameraSystem cameraSystem;
        private ComboSystem comboSystem;
        private EnemyAIProvider enemyAIProvider;
        private GameManager gameManager;
        private Foundation.AudioManager audioManager;

        #endregion

        #region Serialized Fields

        [Header("Gameplay Settings")]
        [SerializeField] private int scorePerKill = 100;
        [SerializeField] private int scorePerHeadshot = 200;
        [SerializeField] private float comboScoreMultiplier = 1.5f;
        [SerializeField] private float maxComboTime = 30f;

        #endregion

        #region State

        private bool isInitialized = false;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        protected virtual void Start()
        {
            InitializeSystems();
            ConnectEvents();
        }

        protected virtual void OnDestroy()
        {
            DisconnectEvents();
        }

        #endregion

        #region Initialization

        private void InitializeSystems()
        {
            if (isInitialized) return;

            // Find all systems
            playerController = FindObjectOfType<PlayerController>();
            movementSystem = FindObjectOfType<MovementSystem>();
            combatSystem = FindObjectOfType<CombatSystem>();
            cameraSystem = FindObjectOfType<CameraSystem>();
            comboSystem = FindObjectOfType<ComboSystem>();
            enemyAIProvider = FindObjectOfType<EnemyAIProvider>();
            gameManager = GameManager.Instance;
            audioManager = Foundation.AudioManager.Instance;

            // Connect player systems
            ConnectPlayerSystems();

            isInitialized = true;
            Debug.Log("Gameplay integrator initialized!");
        }

        private void ConnectPlayerSystems()
        {
            if (playerController == null)
            {
                Debug.LogWarning("PlayerController not found!");
                return;
            }

            // Get input system from player
            var inputSystem = playerController.GetComponent<Foundation.InputSystem>();
            if (inputSystem != null)
            {
                // Subscribe to input events for system integration
            }

            // Connect movement system
            if (movementSystem != null && playerController != null)
            {
                // Set movement provider reference
            }

            // Connect combat system
            if (combatSystem != null && playerController != null)
            {
                // Set combat provider reference
            }

            // Connect camera to player
            if (cameraSystem != null && playerController != null)
            {
                cameraSystem.SetPlayerTarget(playerController.transform);
            }
        }

        #endregion

        #region Event Connection

        private void ConnectEvents()
        {
            if (comboSystem != null)
            {
                comboSystem.OnMilestone10x += OnComboMilestone10;
                comboSystem.OnMilestone25x += OnComboMilestone25;
                comboSystem.OnMilestone50x += OnComboMilestone50;
            }
        }

        private void DisconnectEvents()
        {
            if (comboSystem != null)
            {
                comboSystem.OnMilestone10x -= OnComboMilestone10;
                comboSystem.OnMilestone25x -= OnComboMilestone25;
                comboSystem.OnMilestone50x -= OnComboMilestone50;
            }
        }

        #endregion

        #region Combat Events

        public void OnEnemyKilled(EnemyAI enemy, Vector3 hitPosition, BodyPart bodyPart, bool isHeadshot)
        {
            // Calculate score
            int baseScore = isHeadshot ? scorePerHeadshot : scorePerKill;
            int combo = comboSystem?.GetCurrentCombo() ?? 0;
            float multiplier = comboSystem?.GetComboMultiplier() ?? 1f;

            int finalScore = Mathf.RoundToInt(baseScore * multiplier);

            // Add score to game manager
            gameManager?.AddScore(finalScore);

            // Notify combo system
            comboSystem?.OnKillScored(hitPosition);

            // Play audio
            audioManager?.PlayKill();

            // Debug log
            Debug.Log($"Enemy killed! Score: +{finalScore} (Combo: x{multiplier:F1}, Headshot: {isHeadshot})");
        }

        public void OnPlayerHit(float damage, Vector3 hitPosition, BodyPart bodyPart)
        {
            // Play hit audio
            audioManager?.PlayHit();

            // Camera shake on hit
            cameraSystem?.AddCameraShake(0.5f);

            // Notify HUD (would connect to HUDController)
            Debug.Log($"Player hit for {damage:F1} damage on {bodyPart}");
        }

        public void OnPlayerDamaged(float currentHealth, float maxHealth)
        {
            // Notify HUD of health change
            Debug.Log($"Player health: {currentHealth:F1}/{maxHealth:F1}");
        }

        #endregion

        #region Combo Events

        private void OnComboMilestone10()
        {
            Debug.Log("Combo milestone reached: 10x!");
            audioManager?.PlayDash(); // Reuse sound for milestone
        }

        private void OnComboMilestone25()
        {
            Debug.Log("Combo milestone reached: 25x!");
            audioManager?.PlayDash();
        }

        private void OnComboMilestone50()
        {
            Debug.Log("Combo milestone reached: 50x!");
            audioManager?.PlayDash();
        }

        #endregion

        #region Movement Events

        public void OnPlayerMoved(float speed)
        {
            // Notify combo system of sustained movement
            if (speed > 5f && comboSystem != null)
            {
                comboSystem.OnMovementSustained();
            }

            // Footstep audio
            if (speed > 1f)
            {
                // Could add footstep timing here
            }
        }

        public void OnPlayerJumped()
        {
            audioManager?.PlayJump();
        }

        public void OnPlayerLanded()
        {
            audioManager?.PlayLand();
        }

        public void OnPlayerDashed()
        {
            audioManager?.PlayDash();
        }

        #endregion

        #region Camera Events

        public void OnWeaponFired(WeaponType weaponType)
        {
            cameraSystem?.AddCameraShake(0.3f);
            audioManager?.PlayFire();
        }

        public void OnWeaponReloaded()
        {
            audioManager?.PlayReload();
        }

        #endregion

        #region Game Flow

        public void OnLevelComplete()
        {
            int totalScore = gameManager?.TotalScore ?? 0;

            // Calculate bonus based on time and combo
            float levelBonus = totalScore * 0.2f; // 20% bonus
            int bonusScore = Mathf.RoundToInt(levelBonus);

            gameManager?.AddScore(bonusScore);

            Debug.Log($"Level complete! Bonus: +{bonusScore}");

            // Would trigger level complete screen here
        }

        #endregion

        #region Raycasting

        public bool TryGetAimTarget(out IKillable target, out Vector3 hitPosition, out BodyPart bodyPart)
        {
            target = null;
            hitPosition = Vector3.zero;
            bodyPart = BodyPart.Body;

            if (cameraSystem == null) return false;

            // Raycast from camera center
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, ~LayerMask.GetMask("Player", "UI", "Ignore Raycast"));

            if (hits.Length > 0)
            {
                // Find closest hit that has IKillable
                float closestDistance = float.MaxValue;
                RaycastHit closestHit = default;

                foreach (var hit in hits)
                {
                    IKillable killable = hit.collider.GetComponent<IKillable>();
                    if (killable != null && hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestHit = hit;
                    }
                }

                if (closestHit.collider != null)
                {
                    hitPosition = closestHit.point;
                    target = closestHit.collider.GetComponent<IKillable>();
                    bodyPart = DetermineBodyPart(closestHit.point, target);
                    return true;
                }
            }

            return false;
        }

        private BodyPart DetermineBodyPart(Vector3 hitPosition, IKillable target)
        {
            if (target == null) return BodyPart.Body;

            // Get the target's bounds
            Collider targetCollider = (target as Component)?.GetComponent<Collider>();
            if (targetCollider == null) return BodyPart.Body;

            Vector3 targetPosition = targetCollider.bounds.center;
            float targetHeight = targetCollider.bounds.size.y;

            // Calculate relative hit height
            float relativeHeight = hitPosition.y - (targetPosition.y - targetHeight / 2);

            // Determine body part based on height
            if (relativeHeight > targetHeight * 0.7f)
            {
                return BodyPart.Head;
            }
            else if (relativeHeight < targetHeight * 0.3f)
            {
                return BodyPart.Limbs;
            }
            else
            {
                return BodyPart.Body;
            }
        }

        #endregion

        #region Public API

        public bool IsInitialized => isInitialized;

        #endregion

        #region Debug

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUILayout.Label($"Gameplay Integrator: {(isInitialized ? "Active" : "Inactive")}");
            GUILayout.Label($"Enemies: {enemyAIProvider?.GetEnemyCount() ?? 0}");
            GUILayout.Label($"Combo: {comboSystem?.GetCurrentCombo() ?? 0}");
            GUILayout.Label($"Multiplier: x{comboSystem?.GetComboMultiplier() ?? 1f:F1}");

            if (GUILayout.Button("Test Kill"))
            {
                var enemies = enemyAIProvider?.GetAllEnemies();
                if (enemies != null && enemies.Count > 0)
                {
                    OnEnemyKilled(enemies[0], enemies[0].transform.position, BodyPart.Body, false);
                }
            }
        }
#endif
    }
}
