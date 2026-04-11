using UnityEngine;
using NUnit.Framework;
using System.Collections;

namespace TimesBaddestCat.Tests.Helpers
{
    /// <summary>
    /// Custom test assertions specific to Time's Baddest Cat game mechanics.
    /// </summary>
    public static class GameAssert
    {
        #region Combo System

        /// <summary>
        /// Asserts that the combo multiplier is valid (>= 1.0x).
        /// </summary>
        public static void IsValidComboMultiplier(float multiplier)
        {
            Assert.GreaterOrEqual(multiplier, 1.0f,
                $"Combo multiplier must be >= 1.0x, but was {multiplier:F2}x");
        }

        /// <summary>
        /// Asserts that combo timer is within valid range (0-30 seconds).
        /// </summary>
        public static void ComboTimerInRange(float timer)
        {
            Assert.GreaterOrEqual(timer, 0f, "Combo timer must be >= 0");
            Assert.LessOrEqual(timer, 30f, "Combo timer must be <= 30s");
        }

        #endregion

        #region Combat System

        /// <summary>
        /// Asserts that damage is positive.
        /// </summary>
        public static void IsValidDamage(float damage)
        {
            Assert.Greater(damage, 0f, $"Damage must be positive, but was {damage:F2}");
        }

        /// <summary>
        /// Asserts that ammo count is within valid range (0-max).
        /// </summary>
        public static void AmmoInRange(int ammo, int max)
        {
            Assert.GreaterOrEqual(ammo, 0, "Ammo must be >= 0");
            Assert.LessOrEqual(ammo, max, $"Ammo {ammo} exceeds max {max}");
        }

        #endregion

        #region Movement System

        /// <summary>
        /// Asserts that player position is within world bounds.
        /// </summary>
        public static void IsWithinWorldBounds(Vector3 position, float worldSize = 100f)
        {
            float halfSize = worldSize / 2f;
            Assert.IsTrue(
                Mathf.Abs(position.x) <= halfSize &&
                Mathf.Abs(position.y) <= halfSize &&
                Mathf.Abs(position.z) <= halfSize,
                $"Position {position} is outside world bounds +/-{halfSize}");
        }

        /// <summary>
        /// Asserts that velocity is within parkour limits.
        /// </summary>
        public static void VelocityWithinParkourLimits(Vector3 velocity, float maxSpeed = 50f)
        {
            float speed = velocity.magnitude;
            Assert.LessOrEqual(speed, maxSpeed,
                $"Velocity {speed:F2} exceeds max parkour speed {maxSpeed:F2}");
        }

        #endregion

        #region Era System

        /// <summary>
        /// Asserts that era type is valid enum value.
        /// </summary>
        public static void IsValidEra(EraType era)
        {
            Assert.IsTrue(
                era == EraType.NineteenFifties ||
                era == EraType.NineteenEighties ||
                era == EraType.NineteenTwenties ||
                era == EraType.Future,
                $"Invalid era type: {era}");
        }

        #endregion

        #region Frame Budget

        /// <summary>
        /// Asserts that frame time is within 60 FPS budget (16.6ms).
        /// </summary>
        public static void Within60FPSBudget(float frameTimeMs)
        {
            Assert.LessOrEqual(frameTimeMs, 16.6f,
                $"Frame time {frameTimeMs:F2}ms exceeds 60 FPS budget (16.6ms)");
        }

        /// <summary>
        /// Asserts that frame time is within 144 FPS budget (6.9ms).
        /// </summary>
        public static void Within144FPSBudget(float frameTimeMs)
        {
            Assert.LessOrEqual(frameTimeMs, 6.9f,
                $"Frame time {frameTimeMs:F2}ms exceeds 144 FPS budget (6.9ms)");
        }

        #endregion
    }

    #region Era Enum

    /// <summary>
    /// Available time periods in Time's Baddest Cat.
    /// </summary>
    public enum EraType
    {
        NineteenFifties,
        NineteenEighties,
        NineteenTwenties,
        Future
    }

    #endregion
}
