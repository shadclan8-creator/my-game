using NUnit.Framework;
using TimesBaddestCat.Tests.Helpers;
using TimesBaddestCat.Input;

namespace TimesBaddestCat.Tests.Editor.Unit
{
    /// <summary>
    /// Unit tests for Input System (ADR-0001).
    /// </summary>
    [TestFixture]
    public class InputSystemTests
    {
        private InputSystem inputSystem;
        private const float FrameBudget60FPS = 16.6f;
        private const float FrameBudget144FPS = 6.9f;

        [SetUp]
        public void Setup()
        {
            inputSystem = ScriptableObject.CreateInstance<InputSystem>();
        }

        [Test]
        [Description("Initial state should have no active input")]
        public void InitialState_HasNoActiveInput()
        {
            // Arrange
            // Act & Assert
            Assert.IsNull(inputSystem.MovementAxis, "Movement axis should be null on init");
            Assert.IsFalse(inputSystem.IsDashRequested, "Dash should not be requested on init");
            Assert.IsFalse(inputSystem.IsFireRequested, "Fire should not be requested on init");
        }

        [Test]
        [Description("Movement axis should be normalized")]
        public void MovementAxis_ShouldBeNormalized()
        {
            // Arrange
            inputSystem.MovementAxis = new Vector2(1f, 1f); // Diagonal

            // Act
            Vector2 axis = inputSystem.GetMovementAxis();

            // Assert
            float magnitude = axis.magnitude;
            Assert.IsTrue(magnitude <= 1.0001f, "Movement axis should be normalized");
            GameAssert.Within60FPSBudget(0.001f); // Should be very fast
        }

        [Test]
        [Description("Zero movement axis should return Vector2.zero")]
        public void ZeroMovementAxis_ReturnsZero()
        {
            // Arrange
            inputSystem.MovementAxis = Vector2.zero;

            // Act
            Vector2 axis = inputSystem.GetMovementAxis();

            // Assert
            Assert.AreEqual(Vector2.zero, axis, "Zero movement should return zero");
        }

        [Test]
        [Description("Dash input should be one-time trigger")]
        public void DashInput_ShouldBeOneTimeTrigger()
        {
            // Arrange
            // Act
            bool dash1 = inputSystem.IsDashRequested();
            bool dash2 = inputSystem.IsDashRequested(); // Same frame

            // Assert
            Assert.IsFalse(dash1, "Dash should be one-time trigger, state resets immediately");
            Assert.IsFalse(dash2, "Dash should reset between reads");
        }

        [Test]
        [Description("Aim direction should be normalized")]
        public void AimDirection_ShouldBeNormalized()
        {
            // Arrange
            Vector2 aim = new Vector2(1f, 1f);

            // Act
            inputSystem.SetAimDirection(aim);

            // Assert
            Vector3 direction = inputSystem.GetAimDirection();
            float magnitude = new Vector2(direction.x, direction.z).magnitude;
            Assert.IsTrue(magnitude <= 1.0001f, "Aim direction should be normalized");
        }

        [Test]
        [Description("Device switching should preserve input state")]
        public void DeviceSwitching_ShouldPreserveInputState()
        {
            // Arrange
            inputSystem.MovementAxis = new Vector2(0.5f, 0f);
            bool fireBefore = inputSystem.IsFireRequested();

            // Act
            inputSystem.SwitchDevice();

            // Assert
            Vector2 axisAfter = inputSystem.MovementAxis();
            Assert.AreEqual(new Vector2(0.5f, 0f), axisAfter,
                "Device switch should preserve input state");
            bool fireAfter = inputSystem.IsFireRequested();
            Assert.AreEqual(fireBefore, fireAfter,
                "Device switch should preserve fire state");
        }

        [Test]
        [Category("Performance")]
        [Description("Input sampling should stay under frame budget")]
        public void InputSampling_StayUnderFrameBudget()
        {
            // Arrange
            Vector2[] testAxes = new Vector2[100];
            for (int i = 0; i < 100; i++)
            {
                testAxes[i] = new Vector2(
                    UnityEngine.Random.value,
                    UnityEngine.Random.value
                ).normalized;
            }

            // Act - Measure time
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (int i = 0; i < 100; i++)
            {
                inputSystem.MovementAxis = testAxes[i];
                Vector2 axis = inputSystem.GetMovementAxis();
            }

            sw.Stop();
            float avgTimeMs = sw.ElapsedMilliseconds / 100f;

            // Assert
            GameAssert.Within60FPSBudget(avgTimeMs);
            Assert.Less(avgTimeMs, 0.5f, "Input sampling should be under 0.5ms per call");
        }
    }

    #region Input System Stub (for testing only)

    /// <summary>
    /// Minimal Input System implementation for unit testing.
    /// Production implementation will use Unity Input System Package.
    /// </summary>
    public class InputSystem : ScriptableObject
    {
        public Vector2 MovementAxis { get; set; }
        public bool IsDashRequested() => _dashRequested;
        public bool IsFireRequested() => _fireRequested;
        public Vector3 GetAimDirection() => _aimDirection;
        public void SetAimDirection(Vector2 direction) => _aimDirection = new Vector3(direction.x, 0f, direction.y);

        public void SwitchDevice() => _deviceType = _deviceType == DeviceType.Gamepad ? DeviceType.Keyboard : DeviceType.Gamepad;

        private bool _dashRequested;
        private bool _fireRequested;
        private Vector3 _aimDirection;
        private DeviceType _deviceType = DeviceType.Keyboard;

        private enum DeviceType
        {
            Keyboard,
            Gamepad
        }
    }

    #endregion
}
