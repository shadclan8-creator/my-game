using UnityEngine;
using TimesBaddestCat.Tests.Helpers;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Combo System - Core layer system for scoring and multipliers.
    ///
    /// Implements ADR-0007: Combo System Architecture
    /// </summary>
    public class ComboSystem : MonoBehaviour
    {
        #region Constants

        [Header("Combo Constants")]
        private const float COMBO_DECAY_RATE = 0.3f;
        private const float MAX_COMBO_TIMER = 30f;
        private const float MILESTONE_1_MULTIPLIER = 10f;
        private const float MILESTONE_2_MULTIPLIER = 25f;
        private const float MILESTONE_5_MULTIPLIER = 50f;

        #endregion

        #region Serialized Data

        [Header("Combo State")]
        [SerializeField]
        private int currentCombo = 0;
        [SerializeField]
        private float comboMultiplier = 1f;
        [SerializeField]
        private float comboTimer = 0f;

        #endregion

        #region Events

        [Header("Combo Events")]
        [Header("Combo Count Changed")]
        public event Action<int> OnComboCountChanged;

        [Header("Multiplier Changed")]
        public event Action<float> OnMultiplierChanged;

        [Header("Milestone Reached")]
        public event Action OnMilestone10x;
        public event Action OnMilestone25x;
        public event Action OnMilestone50x;

        #endregion

        #region Dependency References

        [Header("Dependencies")]
        private IMovementProvider movementSystem;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            currentCombo = 0;
            comboMultiplier = 1f;
            comboTimer = 0f;
        }

        protected virtual void Start()
        {
            CacheDependencies();
        }

        protected virtual void OnEnable()
        {
            // Start timer when enabled
            if (comboTimer <= 0f)
            {
                comboTimer = MAX_COMBO_TIMER;
            }
        }

        #endregion

        #region Combo Logic

        [Header("Combo Logic")]
        public void OnKillScored(Vector3 position)
        {
            currentCombo++;
            comboTimer = MAX_COMBO_TIMER;
            comboMultiplier = 1f + (currentCombo * 0.05f);

            // Clamp multiplier to max
            if (comboMultiplier > MILESTONE_5_MULTIPLIER)
            {
                comboMultiplier = MILESTONE_5_MULTIPLIER;
            }

            GameAssert.IsValidComboMultiplier(comboMultiplier);

            // Check milestones
            if (currentCombo == 10) OnMilestone10x?.Invoke();
            else if (currentCombo == 25) OnMilestone25x?.Invoke();
            else if (currentCombo == 50) OnMilestone50x?.Invoke();

            OnComboCountChanged?.Invoke(currentCombo);
            OnMultiplierChanged?.Invoke(comboMultiplier);

            Debug.Log($"Kill scored! Combo: x{comboMultiplier:F1} (Count: {currentCombo})");
        }

        public void OnMovementSustained()
        {
            // Extend combo timer when player keeps moving
            if (comboTimer > 0f && comboTimer < MAX_COMBO_TIMER)
            {
                comboTimer += Time.deltaTime * COMBO_DECAY_RATE;
            }
            else
            {
                ResetCombo();
            }
        }

        public void ResetCombo()
        {
            currentCombo = 0;
            comboMultiplier = 1f;
            comboTimer = MAX_COMBO_TIMER;

            OnComboCountChanged?.Invoke(0);
            OnMultiplierChanged?.Invoke(1f);
        }

        public bool IsComboActive()
        {
            return comboTimer > 0f && comboMultiplier > 1f;
        }

        public int GetCurrentCombo()
        {
            return currentCombo;
        }

        public float GetComboMultiplier()
        {
            return comboMultiplier;
        }

        public float GetComboTimer()
        {
            return comboTimer;
        }

        #endregion

        #region Update Loop

        [Header("Update Loop")]
        private void Update()
        {
            // Decay combo timer
            if (IsComboActive())
            {
                comboTimer -= Time.deltaTime * COMBO_DECAY_RATE;

                if (comboTimer <= 0f)
                    {
                        ResetCombo();
                    }
            }
            }
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Combo: {GetCurrentCombo()} (x{GetComboMultiplier():F1})");
            GUILayout.Label($"Timer: {GetComboTimer():F1}");
        }
        #endif
    }
}
