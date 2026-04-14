using UnityEngine;

namespace TimesBaddestCat.Foundation
{
    /// <summary>
    /// Weapon types available in the game.
    /// </summary>
    public enum WeaponType
    {
        AssaultRifle,
        SMG,
        Shotgun,
        SniperRifle,
        LMG
    }

    /// <summary>
    /// Movement modes for gameplay states.
    /// </summary>
    public enum MovementMode
    {
        Normal,
        Combat,
        Cinematic
    }

    /// <summary>
    /// Body parts for hit detection.
    /// </summary>
    public enum BodyPart
    {
        Head,
        Body,
        Limbs
    }

    /// <summary>
    /// Device types for input.
    /// </summary>
    public enum DeviceType
    {
        KeyboardMouse,
        Gamepad
    }

    /// <summary>
    /// Provides input functionality to other systems.
    /// </summary>
    public interface IInputProvider
    {
        Vector2 GetMovementAxis();
        Vector2 GetAimDirection();
        bool IsDashRequested();
        bool IsFireRequested();
        bool IsReloadRequested();
        MovementMode GetMovementMode();
    }

    /// <summary>
    /// Provides physics and collision functionality.
    /// </summary>
    public interface IPhysicsProvider
    {
        bool IsGrounded(Vector3 position);
        bool CanWallRun(Vector3 position, Vector3 direction);
        bool CanClimb(Vector3 position);
        Vector3 GetWallRunAttachmentPoint(Vector3 position, Vector3 direction, float maxDistance = 10f);
        Vector3 GetWallNormal(Vector3 position);
    }

    /// <summary>
    /// Provides combat and weapon functionality.
    /// </summary>
    public interface ICombatProvider
    {
        void FireWeapon();
        void ReloadWeapon();
        void EquipWeapon(WeaponType weaponType);
        int GetCurrentAmmo();
        int GetMaxAmmo();
        bool IsReloading();
    }

    /// <summary>
    /// Provides camera functionality.
    /// </summary>
    public interface ICameraProvider
    {
        void SetPlayerTarget(Transform target);
        void SetAimDirection(Vector2 direction);
        void AddCameraShake(float intensity);
        void EnableAimMode(bool enable);
    }

    /// <summary>
    /// Provides combo scoring functionality.
    /// </summary>
    public interface IComboProvider
    {
        void OnKillScored(Vector3 position);
        void OnMovementSustained();
        void ResetCombo();
        int GetCurrentCombo();
        float GetComboMultiplier();
        float GetComboTimer();
        bool IsComboActive();
    }

    /// <summary>
    /// Provides enemy AI functionality.
    /// </summary>
    public interface IEnemyAIProvider
    {
        void RegisterEnemy(EnemyAI enemy);
        void UnregisterEnemy(EnemyAI enemy);
        EnemyAI GetNearestEnemy(Vector3 position, float maxDistance);
        void AlertEnemiesInRange(Vector3 position, float radius);
    }

    /// <summary>
    /// Provides movement functionality to other systems.
    /// </summary>
    public interface IMovementProvider
    {
        Vector3 GetVelocity();
        float GetCurrentSpeed();
        bool IsParkourStateActive();
        bool IsWallRunning();
        bool IsClimbing();
        bool IsDashing();
        bool IsSliding();
        void SetMovementMode(MovementMode mode);
        Vector3 GetAimDirection();
    }
}
