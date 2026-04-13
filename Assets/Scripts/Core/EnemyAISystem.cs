using UnityEngine;
using UnityEngine.AI;
using TimesBaddestCat.Tests.Helpers;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Enemy AI System - Core layer system for enemy behaviors and pathfinding.
    ///
    /// Implements ADR-0009: Enemy AI Pathfinding
    /// Uses Unity NavMesh with AI State Machine pattern.
    /// </summary>
    public class EnemyAISystem : MonoBehaviour
    {
        #region Constants

        [Header("AI Constants")]
        private const float PATROL_SPEED = 3f;
        private const float ENGAGE_SPEED = 6f;
        private const float CHASE_SPEED = 8f;
        private const float DETECTION_RANGE = 15f;
        private const float ATTACK_RANGE = 5f;
        private const float ATTACK_COOLDOWN = 2f;

        #endregion

        #region Serialized Data

        [Header("Enemy Configuration")]
        [SerializeField]
        private NavMeshAgent navMeshAgent;

        [Header("AI State")]
        [SerializeField]
        private AIState currentState = AIState.Patrol;
        [SerializeField]
        private Transform playerTarget;

        #endregion

        #region State Machine

        [Header("State Machine")]
        private enum AIState
        {
            Patrol,
            Alerted,
            Engaging,
            Fleeing,
            Dead
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Start()
        {
            InitializeAgent();
        }

        #endregion

        #region Initialization

        [Header("AI Initialization")]
        private void InitializeAgent()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }

            FindPlayer();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        #endregion

        #region AI Behavior

        [Header("Patrol Behavior")]
        private void PatrolBehavior()
        {
            if (!navMeshAgent.enabled) return;

            // Set random patrol waypoints
            Vector3[] waypoints = GeneratePatrolWaypoints();
            navMeshAgent.SetDestination(waypoints[0]);
            currentState = AIState.Patrol;
        }

        [Header("Alerted Behavior")]
        private void AlertedBehavior()
        {
            if (!navMeshAgent.enabled) return;

            currentState = AIState.Alerted;
            navMeshAgent.speed = ENGAGE_SPEED;
        }

        [Header("Engaging Behavior")]
        private void EngagingBehavior()
        {
            if (!navMeshAgent.enabled) return;

            currentState = AIState.Engaging;
            navMeshAgent.speed = CHASE_SPEED;
            navMeshAgent.isStopped = false;

            // Face player
            FaceTarget();
        }

        [Header("Fleeing Behavior")]
        private void FleeingBehavior()
        {
            if (!navMeshAgent.enabled) return;

            currentState = AIState.Fleeing;
            navMeshAgent.speed = CHASE_SPEED * 1.5f;
            navMeshAgent.isStopped = false;
        }

        [Header("Dead Behavior")]
        private void DeadBehavior()
        {
            currentState = AIState.Dead;
            navMeshAgent.isStopped = true;
        }

        #endregion

        #region Helper Methods

        [Header("AI Helpers")]
        private void FaceTarget()
        {
            if (playerTarget != null)
            {
                Vector3 direction = (playerTarget.position - transform.position).normalized;
                transform.LookAt(playerTarget.position);
            }
        }

        private Vector3[] GeneratePatrolWaypoints()
        {
            // Generate random waypoints around current position
            Vector3[] waypoints = new Vector3[3];
            for (int i = 0; i < waypoints.Length; i++)
            {
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-10f, 10f),
                    0f,
                    UnityEngine.Random.Range(-5f, 5f)
                );
                waypoints[i] = transform.position + randomOffset;
            }
            return waypoints;
        }

        private void CheckPlayerDetection()
        {
            // Raycast to detect player
            if (playerTarget == null) return;

            Vector3 toPlayer = playerTarget.position - transform.position;
            float distance = toPlayer.magnitude;

            if (distance < DETECTION_RANGE)
            {
                // Player detected - switch to alerted/engaging
                switch (currentState)
                {
                    case AIState.Patrol:
                        AlertedBehavior();
                        break;
                    case AIState.Alerted:
                        EngagingBehavior();
                        break;
                }
            }
            else
            {
                // Player lost - continue patrol
                if (currentState != AIState.Patrol && currentState != AIState.Fleeing)
                {
                    PatrolBehavior();
                }
            }
        }

        public void TakeDamage(float damage)
        {
            // Current AI will die on damage (Combat System)
            currentState = AIState.Dead;
            navMeshAgent.isStopped = true;
        }

        #endregion

        #region Update Loop

        [Header("Update Loop")]
        private void Update()
        {
            // Only run if player target exists
            if (playerTarget == null) return;

            CheckPlayerDetection();

            // Execute current AI state behavior
            switch (currentState)
            {
                case AIState.Patrol:
                    PatrolBehavior();
                    break;
                case AIState.Alerted:
                    AlertedBehavior();
                    break;
                case AIState.Engaging:
                    EngagingBehavior();
                    break;
                case AIState.Fleeing:
                    FleeingBehavior();
                    break;
                case AIState.Dead:
                    DeadBehavior();
                    break;
            }
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Visualization")]
        private void OnDrawGizmos()
        {
            // Visualize detection range
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            if (playerTarget != null)
            {
                Gizmos.DrawWireSphere(playerTarget.position, DETECTION_RANGE);
            }

            // Visualize patrol waypoints
            Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
            Vector3[] waypoints = GeneratePatrolWaypoints();
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.3f);
            }
        }
        #endif
    }
}
