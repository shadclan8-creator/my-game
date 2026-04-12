using UnityEngine;
using TimesBaddestCat.Tests.Helpers;

namespace TimesBaddestCat.Foundation
{
    /// <summary>
    /// Physics System - Foundation layer system for collision detection, projectile physics,
    /// and environmental collision.
    ///
    /// Implements ADR-0002: Physics Collision Strategy
    /// </summary>
    public class PhysicsSystem : MonoBehaviour
    {
        #region Constants

        private const float MAX_RAYCAST_DISTANCE = 20f;
        private const float WALL_RUN_ATTACH_SPEED = 2f;
        private const float WALL_RUN_DETACH_SPEED = 1f;
        private const float GROUND_DETECTION_RADIUS = 1f;
        private const float MAX_PROJECTILE_POOL_SIZE = 50;

        #endregion

        #region LayerMasks

        [Header("Layer Masks")]
        private static class Layers
        {
            public static readonly int Traversable = 1 << 0;
            public static readonly int Ground = 1 << 1;
            public static readonly int Enemy = 1 << 2;
            public static readonly int Target = 1 << 3;
            public static readonly int Projectile = 1 << 4;
            public static readonly int Environment = 1 << 5;
            public static readonly int Cover = 1 << 6;
        }

        #endregion

        #region Serialized Data

        [Header("Physics Configuration")]
        [SerializeField]
        private LayerMask traversableLayer = Layers.Traversable;
        [SerializeField]
        private LayerMask groundLayer = Layers.Ground;
        [SerializeField]
        private LayerMask enemyLayer = Layers.Enemy | Layers.Target;
        [SerializeField]
        private LayerMask projectileCollisionLayers = Layers.Enemy | Layers.Target | Layers.Environment;

        #endregion

        #region Projectile Pooling

        private GameObject[] projectilePool;
        private int poolIndex = 0;

        [Header("Projectile Pooling")]
        private void InitializeProjectilePool(int size)
        {
            projectilePool = new GameObject[MAX_PROJECTILE_POOL_SIZE];
            for (int i = 0; i < size; i++)
            {
                projectilePool[i] = new GameObject("Projectile", typeof(CapsuleCollider));
                projectilePool[i].layer = Layers.Projectile;
                projectilePool[i].SetActive(false);
            }
            poolIndex = 0;
        }

        public GameObject GetProjectile()
        {
            for (int i = 0; i < projectilePool.Length; i++)
            {
                if (!projectilePool[i].activeInHierarchy)
                {
                    projectilePool[i].SetActive(true);
                    return projectilePool[i];
                }
            }
            }
            Debug.LogWarning("Projectile pool exhausted!");
            return null;
        }

        public void ReturnProjectile(GameObject projectile)
        {
            if (projectile != null)
            {
                projectile.SetActive(false);
            }
        }

        #endregion

        #region Surface Detection

        [Header("Surface Detection")]
        public bool CanWallRun(Vector3 direction, float maxDistance = MAX_RAYCAST_DISTANCE)
        {
            return Physics.Raycast(origin: direction, direction: direction, maxDistance: maxDistance,
                layerMask: traversableLayer);
        }

        public bool CanWallRun(Vector3 origin, Vector3 direction)
        {
            return CanWallRun(origin, direction, MAX_RAYCAST_DISTANCE);
        }

        public bool CanClimb(Vector3 position, float downwardCheckDistance = GROUND_DETECTION_RADIUS)
        {
            Vector3 raycastOrigin = position + Vector3.up * downwardCheckDistance;
            return Physics.Raycast(raycastOrigin, Vector3.down, downwardCheckDistance,
                layerMask: traversableLayer);
        }

        public bool IsGrounded(Vector3 position, float checkDistance = GROUND_DETECTION_RADIUS)
        {
            return Physics.SphereCast(position, checkDistance, Vector3.down, checkDistance,
                    layerMask: groundLayer);
        }

        public Collider GetNearbyCover(Vector3 position, float radius = 2f)
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius, layerMask: Layers.Cover);
            return colliders.Length > 0 ? colliders[0] : null;
        }

        #endregion

        #region Parkour-Specific Physics

        [Header("Parkour Physics")]
        public float CalculateWallRunGravityModifier()
        {
            // From physics-system.md: SlideModifier: 0.6 to 0.8
            return 0.7f; // Middle of range for sliding
        }

        public float CalculateClimbGravityModifier()
        {
            // From physics-system.md: ClimbModifier: 0.7 to 0.9
            return 0.8f; // Middle of range for climbing
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            InitializeProjectilePool(10); // Start with 10, expand as needed
        }

        #endregion

        #region Debug Visualization

        #if UNITY_EDITOR
        [Header("Debug Visualization")]
        private void OnDrawGizmos()
        {
            // Draw traversable surface detection
            Gizmos.color = Color.cyan * 0.5f;

            if (Camera.main != null)
            {
                Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward,
                    MAX_RAYCAST_DISTANCE, Color.cyan * 0.5f);
            }
        }
        }
        #endif
    }

    #region Helper Classes

    /// <summary>
    /// Data container for collision events.
    /// </summary>
    public struct CollisionData
    {
        public Vector3 point;
        public Vector3 normal;
        public Collider collider;
    }

    #endregion
}
