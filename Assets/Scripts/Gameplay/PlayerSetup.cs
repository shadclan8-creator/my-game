using UnityEngine;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// PlayerSetup - Automatically configures the player GameObject with all required components.
    /// Add this to a GameObject and run SetupPlayer() to create a complete player.
    /// </summary>
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Player Configuration")]
        [SerializeField] private bool setupOnAwake = true;

        [Header("Components (Auto-populated)")]
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Foundation.InputSystem inputSystem;
        [SerializeField] private Core.MovementSystem movementSystem;
        [SerializeField] private Core.CombatSystem combatSystem;
        [SerializeField] private Core.CameraSystem cameraSystem;
        [SerializeField] private Core.ComboSystem comboSystem;
        [SerializeField] private Core.EnemyAIProvider enemyAIProvider;
        [SerializeField] private Foundation.PhysicsSystem physicsSystem;

        protected virtual void Awake()
        {
            if (setupOnAwake)
            {
                SetupPlayer();
            }
        }

        [ContextMenu("Setup Player")]
        public void SetupPlayer()
        {
            // Tag as Player
            gameObject.tag = "Player";

            // Add required components
            EnsureComponent<Rigidbody>(ref playerRigidbody);
            EnsureComponent<CapsuleCollider>(ref playerCollider);
            EnsureComponent<PlayerController>(ref playerController);
            EnsureComponent<Foundation.InputSystem>(ref inputSystem);
            EnsureComponent<Core.MovementSystem>(ref movementSystem);
            EnsureComponent<Core.CombatSystem>(ref combatSystem);
            EnsureComponent<Core.CameraSystem>(ref cameraSystem);
            EnsureComponent<Core.ComboSystem>(ref comboSystem);

            // Find or create singleton providers
            enemyAIProvider = FindObjectOfType<Core.EnemyAIProvider>();
            if (enemyAIProvider == null)
            {
                GameObject providerObj = new GameObject("EnemyAIProvider");
                enemyAIProvider = providerObj.AddComponent<Core.EnemyAIProvider>();
            }

            physicsSystem = FindObjectOfType<Foundation.PhysicsSystem>();
            if (physicsSystem == null)
            {
                GameObject physicsObj = new GameObject("PhysicsSystem");
                physicsSystem = physicsObj.AddComponent<Foundation.PhysicsSystem>();
            }

            // Configure Rigidbody
            if (playerRigidbody != null)
            {
                playerRigidbody.mass = 70f;
                playerRigidbody.drag = 0f;
                playerRigidbody.angularDrag = 0f;
                playerRigidbody.useGravity = true;
                playerRigidbody.isKinematic = false;
                playerRigidbody.interpolation = RigidbodyInterpolationMode.Interpolate;
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            // Configure Capsule Collider
            if (playerCollider != null)
            {
                playerCollider.height = 2f;
                playerCollider.radius = 0.5f;
                playerCollider.center = new Vector3(0f, 1f, 0f);
            }

            // Set camera follow target
            if (cameraSystem != null)
            {
                cameraSystem.SetPlayerTarget(transform);
            }

            Debug.Log("Player setup complete!");
        }

        private void EnsureComponent<T>(ref T component) where T : Component
        {
            if (component == null)
            {
                component = GetComponent<T>();
                if (component == null)
                {
                    component = gameObject.AddComponent<T>();
                }
            }
        }
    }
}
