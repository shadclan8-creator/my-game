using UnityEngine;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Base class for enemy AI behavior.
    /// Implements the IKillable interface for damage handling.
    /// </summary>
    public class EnemyAI : MonoBehaviour, IKillable
    {
        #region Serialized Fields

        [Header("Enemy Configuration")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float detectionRange = 20f;
        [SerializeField] protected float attackRange = 5f;
        [SerializeField] protected float attackDamage = 20f;
        [SerializeField] protected float attackCooldown = 1.5f;

        [Header("AI State")]
        [SerializeField] protected EnemyState currentState = EnemyState.Patrol;
        [SerializeField] protected float currentHealth;

        #endregion

        #region Protected Fields

        protected Transform playerTarget;
        protected Vector3 patrolDestination;
        protected float attackTimer = 0f;
        protected bool isDead = false;

        #endregion

        #region Public Events

        public event Action<EnemyAI> OnEnemyDeath;
        public event Action<float> OnHealthChanged;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            SelectNewPatrolDestination();
        }

        protected virtual void Start()
        {
            FindPlayer();
        }

        protected virtual void Update()
        {
            if (isDead) return;

            UpdateAttackTimer();
            UpdateAIState();
        }

        #endregion

        #region AI State Management

        protected virtual void UpdateAIState()
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    UpdatePatrol();
                    break;
                case EnemyState.Chase:
                    UpdateChase();
                    break;
                case EnemyState.Attack:
                    UpdateAttack();
                    break;
            }
        }

        protected virtual void UpdatePatrol()
        {
            // Move toward patrol destination
            if (Vector3.Distance(transform.position, patrolDestination) < 1f)
            {
                SelectNewPatrolDestination();
            }
            else
            {
                MoveToward(patrolDestination);
            }

            // Check for player
            if (playerTarget != null && Vector3.Distance(transform.position, playerTarget.position) < detectionRange)
            {
                EnterState(EnemyState.Chase);
            }
        }

        protected virtual void UpdateChase()
        {
            if (playerTarget == null)
            {
                EnterState(EnemyState.Patrol);
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer < attackRange)
            {
                EnterState(EnemyState.Attack);
            }
            else if (distanceToPlayer > detectionRange * 1.5f)
            {
                EnterState(EnemyState.Patrol);
            }
            else
            {
                MoveToward(playerTarget.position);
            }
        }

        protected virtual void UpdateAttack()
        {
            if (playerTarget == null)
            {
                EnterState(EnemyState.Patrol);
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer > attackRange * 1.2f)
            {
                EnterState(EnemyState.Chase);
                return;
            }

            // Face player
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
            }

            // Attack if cooldown is ready
            if (attackTimer <= 0f)
            {
                PerformAttack();
            }
        }

        #endregion

        #region Actions

        protected virtual void MoveToward(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 3f * Time.deltaTime);
                transform.position += transform.forward * 3f * Time.deltaTime;
            }
        }

        protected virtual void PerformAttack()
        {
            attackTimer = attackCooldown;

            // Deal damage to player if in range
            if (playerTarget != null)
            {
                PlayerController player = playerTarget.GetComponent<PlayerController>();
                if (player != null)
                {
                    // Placeholder: In real implementation, use proper damage system
                    Debug.Log($"Enemy {name} deals {attackDamage} damage to player!");
                }
            }

            // Visual feedback
            Debug.Log($"Enemy {name} attacks!");
        }

        protected virtual void SelectNewPatrolDestination()
        {
            patrolDestination = transform.position + Random.insideUnitSphere * 10f;
            patrolDestination.y = transform.position.y;
        }

        protected virtual void EnterState(EnemyState newState)
        {
            currentState = newState;
        }

        protected virtual void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        protected virtual void UpdateAttackTimer()
        {
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
        }

        #endregion

        #region IKillable Implementation

        public virtual void TakeDamage(float damage, Vector3 hitPosition, BodyPart bodyPart)
        {
            if (isDead) return;

            currentHealth -= damage;
            OnHealthChanged?.Invoke(currentHealth);

            // Visual feedback
            Debug.Log($"Enemy {name} took {damage:F1} damage! Health: {currentHealth:F1}/{maxHealth:F1}");

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public virtual BodyPart GetBodyPart()
        {
            // Simplified - return random body part for hit detection
            return (BodyPart)Random.Range(0, 3);
        }

        public bool enabled => !isDead && gameObject.activeInHierarchy;

        #endregion

        #region Death Handling

        protected virtual void Die()
        {
            isDead = true;
            OnEnemyDeath?.Invoke(this);

            // Disable collider and ragdoll would go here
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Destroy after delay
            Destroy(gameObject, 3f);

            Debug.Log($"Enemy {name} defeated!");
        }

        #endregion

        #region Public API

        public void SetPlayerTarget(Transform target)
        {
            playerTarget = target;
        }

        public EnemyState GetCurrentState() => currentState;
        public float GetHealthPercent() => currentHealth / maxHealth;
        public bool IsDead() => isDead;

        #endregion
    }

    /// <summary>
    /// Enemy AI states.
    /// </summary>
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Alerted,
        Dead
    }
}
