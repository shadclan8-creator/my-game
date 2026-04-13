using System;
using System.Collections;
using UnityEngine;
using TimesBaddestCat.Tests.Helpers;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Combat System - Core layer system for weapons, damage, and projectiles.
    ///
    /// Implements ADR-0004: Combat System Architecture
    /// </summary>
    public class CombatSystem : MonoBehaviour, ICombatProvider
    {
        #region Constants

        [Header("Combat Constants")]
        private const float BASE_DAMAGE = 100f;
        private const float HEADSHOT_MULTIPLIER = 2f;
        private const float BODY_PART_MULTIPLIER_MIN = 1f;
        private const float BODY_PART_MULTIPLIER_MAX = 1.5f;
        private const float WEAPON_MULTIPLIER_MAX = 3f;
        private const float PROJECTILE_SPEED = 500f;
        private const float PROJECTILE_MAX_LIFETIME = 10f;
        private const int MAX_PROJECTILE_POOL_SIZE = 50;

        #endregion

        #region Serialized Data

        [Header("Weapon Configuration")]
        [SerializeField]
        private WeaponData currentWeapon;

        [Header("Ammo State")]
        [SerializeField]
        private int currentAmmo;
        [SerializeField]
        private int maxAmmo = 30;

        #endregion

        #region State

        [Header("Combat State")]
        [SerializeField]
        private bool isReloading = false;

        #endregion

        #region Projectile Management

        [Header("Projectiles")]
        private GameObject[] projectilePool;
        private int poolIndex = 0;

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        private IInputProvider inputSystem;
        private IPhysicsProvider physicsSystem;
        private IComboProvider comboSystem;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            InitializeProjectilePool(MAX_PROJECTILE_POOL_SIZE);
            EquipDefaultWeapon();
        }

        protected virtual void Start()
        {
            CacheDependencies();
        }

        #endregion

        #region Weapon Management

        [Header("Weapon Management")]
        private void EquipDefaultWeapon()
        {
            // Default to AR (Assault Rifle)
            EquipWeapon(WeaponType.AssaultRifle);
        }

        public void EquipWeapon(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.AssaultRifle:
                    currentAmmo = 30;
                    maxAmmo = 30;
                    break;
                case WeaponType.SMG:
                    currentAmmo = 40;
                    maxAmmo = 40;
                    break;
                case WeaponType.Shotgun:
                    currentAmmo = 8;
                    maxAmmo = 8;
                    break;
                case WeaponType.SniperRifle:
                    currentAmmo = 5;
                    maxAmmo = 5;
                    break;
                case WeaponType.LMG:
                    currentAmmo = 100;
                    maxAmmo = 100;
                    break;
            }
        }

        private void ReloadWeaponInternal()
        {
            // Check if can reload
            if (currentAmmo == maxAmmo) return;

            // Visual feedback for reload would go here
            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            isReloading = true;

            // Reload duration based on weapon type
            float reloadTime = GetReloadTimeForWeapon(currentWeapon.type);
            yield return new WaitForSeconds(reloadTime);

            currentAmmo = maxAmmo;
            isReloading = false;
        }

        private float GetReloadTimeForWeapon(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Shotgun:
                    return 0.7f;
                case WeaponType.SniperRifle:
                    return 1.5f;
                case WeaponType.LMG:
                    return 0.3f;
                default: // AR, SMG
                    return 0.5f;
            }
        }

        #endregion

        #region Combat Actions

        [Header("Combat Actions")]
        private void FireWeaponInternal()
        {
            // Check if can fire
            if (currentAmmo <= 0 || isReloading) return;

            // Fire weapon
            GameObject projectile = GetProjectile();
            if (projectile != null)
            {
                Vector3 direction = GetFireDirection();

                // Set projectile position and velocity
                projectile.transform.position = transform.position;
                projectile.transform.rotation = Quaternion.LookRotation(direction);

                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
                if (projectileRb != null)
                {
                    projectileRb.velocity = direction * PROJECTILE_SPEED;
                }

                // Decrement ammo
                currentAmmo--;

                // Notify combo system of kill event
                comboSystem?.OnKillScored(transform.position);

                // Auto-destroy after max lifetime
                StartCoroutine(DestroyProjectile(projectile, PROJECTILE_MAX_LIFETIME));
            }
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

        private IEnumerator DestroyProjectile(GameObject projectile, float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            ReturnProjectile(projectile);
        }

        public void OnEnemyKill(IKillable enemy, Vector3 hitPosition)
        {
            // Combo scoring on enemy kill
            comboSystem?.OnKillScored(hitPosition);

            // Visual feedback
            SpawnImpactEffect(hitPosition, enemy.GetBodyPart());
        }

        public void TakeDamage(IKillable enemy, float damage, Vector3 hitPosition, BodyPart bodyPart)
        {
            float multiplier = GetBodyPartMultiplier(bodyPart);
            float finalDamage = (BASE_DAMAGE * GetWeaponMultiplier()) * multiplier;

            enemy?.TakeDamage(finalDamage, hitPosition, bodyPart);

            // Visual feedback
            SpawnImpactEffect(hitPosition, bodyPart);
        }

        // ICombatProvider implementation - redirect to internal methods
        void ICombatProvider.FireWeapon() => FireWeaponInternal();
        void ICombatProvider.ReloadWeapon() => ReloadWeaponInternal();
        void ICombatProvider.EquipWeapon(WeaponType weaponType) => EquipWeapon(weaponType);
        int ICombatProvider.GetCurrentAmmo() => currentAmmo;
        int ICombatProvider.GetMaxAmmo() => maxAmmo;
        bool ICombatProvider.IsReloading() => isReloading;

        public bool CanDamage(IKillable target)
        {
            // Check if target is valid
            return target != null && target.enabled;
        }

        #endregion

        #region Helpers

        [Header("Damage Helpers")]
        private float GetBodyPartMultiplier(BodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case BodyPart.Head:
                    return HEADSHOT_MULTIPLIER;
                case BodyPart.Body:
                    return UnityEngine.Random.Range(BODY_PART_MULTIPLIER_MIN, BODY_PART_MULTIPLIER_MAX);
                case BodyPart.Limbs:
                    return UnityEngine.Random.Range(BODY_PART_MULTIPLIER_MIN, BODY_PART_MULTIPLIER_MAX);
                default:
                    return 1f;
            }
        }

        private float GetWeaponMultiplier()
        {
            // From ADR-0004: weapon multiplier varies by type
            switch (currentWeapon?.type)
            {
                case WeaponType.SMG:
                    return 0.8f;
                case WeaponType.Shotgun:
                    return 1.2f;
                case WeaponType.SniperRifle:
                    return 3f;
                case WeaponType.LMG:
                    return WEAPON_MULTIPLIER_MAX;
                default:
                    return 1f;
            }
        }

        private Vector3 GetFireDirection()
        {
            // Use aim direction from input system
            Vector3 aim = inputSystem.GetAimDirection();
            return new Vector3(aim.x, 0f, aim.y);
        }

        private void SpawnImpactEffect(Vector3 position, BodyPart bodyPart)
        {
            // VFX system would handle this
            // For now, log debug
            Debug.Log($"Impact at {position} on {bodyPart}");
        }

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        private void CacheDependencies()
        {
            inputSystem = FindObjectOfType<IInputProvider>();
            physicsSystem = FindObjectOfType<IPhysicsProvider>();
            comboSystem = FindObjectOfType<IComboProvider>();

            if (inputSystem == null || physicsSystem == null || comboSystem == null)
            {
                Debug.LogWarning("Combat System missing dependencies!");
            }
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Weapon: {currentWeapon?.type}");
            GUILayout.Label($"Ammo: {currentAmmo}/{maxAmmo}");
            GUILayout.Label($"Projectiles Active: {GetActiveProjectileCount()}");
        }
        #endif
    }

    #region Enums & Interfaces

    [Header("Weapon Types")]
    public enum WeaponType
    {
        AssaultRifle,
        SMG,
        Shotgun,
        SniperRifle,
        LMG
    }

    [Header("Body Parts")]
    public enum BodyPart
    {
        Head,
        Body,
        Limbs
    }

    [Header("Weapon Data")]
    [System.Serializable]
    public class WeaponData
    {
        public WeaponType type;
        public string name;
        public int fireRate; // rounds per second
        public float damagePerSecond;
        public float reloadTime;
    }

    [Header("Combat Interfaces")]
    public interface IComboProvider
    {
        void OnKillScored(Vector3 position);
    }

    [Header("Killable")]
    public interface IKillable
    {
        void TakeDamage(float damage, Vector3 hitPosition, BodyPart bodyPart);
        BodyPart GetBodyPart();
        bool enabled { get; }
    }

    #endregion

    #region Events

    [Header("Combat Events")]
    public class WeaponFiredEvent : GameEventBase
    {
        public WeaponType weaponType;
    }

    #endregion
}
