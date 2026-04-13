using UnityEngine;
using TimesBaddestCat.Tests.Helpers;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Foundation
{
    /// <summary>
    /// Physics System - Foundation layer system for collision detection, projectile physics,
    /// and environmental collision.
    ///
    /// Implements ADR-0002: Physics Collision Strategy
    /// </summary>
    public class PhysicsSystem : MonoBehaviour, IPhysicsProvider
    {
        #region Constants

        private const float MAX_RAYCAST_DISTANCE = 20f;
        private const float WALL_RUN_ATTACH_SPEED = 2f;
        private const float WALL_RUN_DETACH_SPEED = 1f;
        private const float GROUND_DETECTION_RADIUS = 1f;
        private const float MAX_PROJECTILE_POOL_SIZE = 50;
        private const int TRAVERSABLE_LAYER = 6;  // Layer index for traversable surfaces
        private const int GROUND_LAYER = 8;       // Layer index for ground

        #endregion

        #region Serialized Data

        [Header("Physics Configuration")]
        [SerializeField]
        private LayerMask traversableLayer = 1 << TRAVERSABLE_LAYER;
        [SerializeField]
        private LayerMask groundLayer = 1 << GROUND_LAYER;
        [SerializeField]
        private LayerMask enemyLayer = 1 << 10;
        [SerializeField]
        private LayerMask projectileCollisionLayers = -1; // All layers

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
                GameObject proj = new GameObject($"Projectile_{i}", typeof(CapsuleCollider), typeof(Rigidbody));
                proj.layer = LayerMask.NameToLayer("Projectile");
                proj.SetActive(false);
                projectilePool[i] = proj;

                // Setup collider
                CapsuleCollider col = proj.GetComponent<CapsuleCollider>();
                col.radius = 0.1f;
                col.height = 0.3f;
                col.isTrigger = true;

                // Setup rigidbody
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
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
            Debug.LogWarning("Projectile pool exhausted!");
            return null;
        }

        public void ReturnProjectile(GameObject projectile)
        {
            if (projectile != null)
            {
                projectile.SetActive(false);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        #endregion

        #region IPhysicsProvider Implementation

        public bool IsGrounded(Vector3 position)
        {
            return IsGrounded(position, GROUND_DETECTION_RADIUS);
        }

        public bool CanWallRun(Vector3 position, Vector3 direction)
        {
            return CanWallRun(position, direction, MAX_RAYCAST_DISTANCE);
        }

        public bool CanClimb(Vector3 position)
        {
            return CanClimb(position, GROUND_DETECTION_RADIUS);
        }

        public Vector3 GetWallRunAttachmentPoint(Vector3 position, Vector3 direction, float maxDistance = 10f)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, direction, out hit, maxDistance, traversableLayer))
            {
                // Return point slightly offset from wall
                return hit.point - direction * 0.1f;
            }
            return position + direction * 2f;
        }

        public Vector3 GetWallNormal(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.forward, out hit, 1f, traversableLayer))
            {
                return hit.normal;
            }
            return Vector3.back;
        }

        #endregion

        #region Surface Detection

        [Header("Surface Detection")]
        public bool CanWallRun(Vector3 direction, float maxDistance = MAX_RAYCAST_DISTANCE)
        {
            return Physics.Raycast(Vector3.zero, direction, maxDistance, traversableLayer);
        }

        public bool CanClimb(Vector3 position, float upwardCheckDistance = 2f)
        {
            Vector3 raycastOrigin = position + Vector3.up * 0.5f;
            RaycastHit hit;
            return Physics.Raycast(raycastOrigin, Vector3.up, out hit, upwardCheckDistance, traversableLayer);
        }

        public bool IsGrounded(Vector3 position, float checkDistance = GROUND_DETECTION_RADIUS)
        {
            return Physics.SphereCast(position, checkDistance * 0.5f, Vector3.down, out _, checkDistance, groundLayer);
        }

        public Collider GetNearbyCover(Vector3 position, float radius = 2f)
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius, 1 << 7);
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
                Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * MAX_RAYCAST_DISTANCE);
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
