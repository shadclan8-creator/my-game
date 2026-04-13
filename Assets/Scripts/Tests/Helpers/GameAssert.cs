using UnityEngine;

namespace TimesBaddestCat.Tests.Helpers
{
    /// <summary>
    /// GameAssert - Runtime assertion utilities for gameplay systems.
    /// Used for validating game state invariants during development and testing.
    /// </summary>
    public static class GameAssert
    {
        #region Combo System Assertions

        /// <summary>
        /// Validates that combo multiplier is within acceptable bounds.
        /// </summary>
        public static void IsValidComboMultiplier(float multiplier)
        {
            if (multiplier < 1f || multiplier > 50f)
            {
                Debug.LogError($"Invalid combo multiplier: {multiplier}. Must be between 1 and 50.");
            }
        }

        #endregion

        #region Movement System Assertions

        /// <summary>
        /// Validates that player velocity is within parkour speed limits.
        /// </summary>
        public static void VelocityWithinParkourLimits(Vector3 velocity, float maxSpeed)
        {
            if (velocity.magnitude > maxSpeed * 1.5f)
            {
                Debug.LogWarning($"Player velocity {velocity.magnitude:F2} exceeds parkour limits ({maxSpeed:F2}). This may indicate a physics bug.");
            }
        }

        /// <summary>
        /// Validates that player position is within world bounds.
        /// </summary>
        public static void PositionWithinWorldBounds(Vector3 position, Vector3 minBounds, Vector3 maxBounds)
        {
            if (position.x < minBounds.x || position.x > maxBounds.x ||
                position.y < minBounds.y || position.y > maxBounds.y ||
                position.z < minBounds.z || position.z > maxBounds.z)
            {
                Debug.LogWarning($"Player position {position} is outside world bounds.");
            }
        }

        #endregion

        #region Combat System Assertions

        /// <summary>
        /// Validates that damage value is positive.
        /// </summary>
        public static void IsValidDamage(float damage)
        {
            if (damage < 0f)
            {
                Debug.LogError($"Invalid damage value: {damage}. Damage must be non-negative.");
            }
        }

        /// <summary>
        /// Validates that ammo count is within magazine limits.
        /// </summary>
        public static void IsValidAmmoCount(int current, int max)
        {
            if (current < 0 || current > max)
            {
                Debug.LogError($"Invalid ammo count: {current}/{max}. Current must be between 0 and max.");
            }
        }

        #endregion

        #region Health System Assertions

        /// <summary>
        /// Validates that health value is within valid range.
        /// </summary>
        public static void IsValidHealth(float current, float max)
        {
            if (current < 0f || current > max)
            {
                Debug.LogError($"Invalid health: {current}/{max}. Health must be between 0 and max.");
            }
        }

        #endregion

        #region General Assertions

        /// <summary>
        /// Generic null check with custom error message.
        /// </summary>
        public static void NotNull<T>(T value, string context) where T : class
        {
            if (value == null)
            {
                Debug.LogError($"Null reference in {context}.");
            }
        }

        #endregion
    }
}
