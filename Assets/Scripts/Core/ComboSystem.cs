using UnityEngine;

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
        private const float MAX_COMBO_MULTIPLIER = 50f;
        private const float MILESTONE_1X = 10f;
        private const float MILESTONE_2_5X = 25f;
        private const float MILESTONE_5_0X = 50f;

        #endregion

        #region Serialized Data

        [Header("Combo State")]
        [SerializeField]
        private int currentComboCount = 0;
        [SerializeField]
        private float comboMultiplier = 1f;
        [SerializeField]
        private float comboTimer = MAX_COMBO_TIMER;
        [SerializeField]
        private int maxCombo = 0;

        #endregion

        #region Events

        [Header("Combo Events")]
        [Header("Kill Scored")]
        public event Action<int> OnComboCountChanged;
        public event Action<float> OnMultiplierChanged;
        [Header("Milestone Reached")]
        public event Action OnMilestone10x;
        public event Action OnMilestone25x;
        public event Action OnMilestone50x;

        #endregion

        #region Combo Logic

        [Header("Combo Logic")]
        public void OnKillScored(Vector3 position)
        {
            currentComboCount++;
            comboTimer = MAX_COMBO_TIMER;

            // Calculate multiplier based on combo count
            comboMultiplier = 1f + (currentComboCount * 0.5f);
            GameAssert.IsValidComboMultiplier(comboMultiplier);

            // Check milestones
            if (currentComboCount == (int)MILESTONE_1X)
            {
                OnMilestone10x?.Invoke();
                SpawnMilestoneEffect(position);
            }
            else if (currentComboCount == (int)MILESTONE_2_5X)
            {
                OnMilestone25x?.Invoke();
                SpawnMilestoneEffect(position);
            }
            else if (currentComboCount == (int)MILESTONE_5_0X)
            {
                OnMilestone50x?.Invoke();
                SpawnMilestoneEffect(position);
            }

            // Update max combo
            if (currentComboCount > maxCombo)
            {
                maxCombo = currentComboCount;
            }

            OnComboCountChanged?.Invoke(currentComboCount);
            OnMultiplierChanged?.Invoke(comboMultiplier);

            Debug.Log($"Kill scored! Combo: x{comboMultiplier:F1} (Count: {currentComboCount})");
        }

        public void OnMovementSustained()
        {
            // Extend combo timer when player keeps moving
            if (comboTimer > 0f)
            {
                comboTimer += Time.deltaTime * COMBO_DECAY_RATE;
            }
        }

        public void ResetCombo()
        {
            currentComboCount = 0;
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
            return currentComboCount;
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
            if (IsComboActive())
            {
                // Decay combo timer
                comboTimer -= Time.deltaTime;

                if (comboTimer <= 0f)
                {
                    ResetCombo();
                }
            }

            // Debug info
            if (IsComboActive())
            {
                Debug.Log($"Combo: x{comboMultiplier:F1} (Timer: {comboTimer:F2})");
            }
        }

        #endregion

        #region Visual Effects

        [Header("Visual Effects")]
        private void SpawnMilestoneEffect(Vector3 position)
        {
            // VFX system would handle this
            // For now, spawn debug marker
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.2f;
            Destroy(marker, 2f);

            Debug.Log($"Milestone at {position}");
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Current Combo: {GetCurrentCombo()} (x{GetComboMultiplier():F1})");
            GUILayout.Label($"Combo Timer: {GetComboTimer():F2}");
            GUILayout.Label($"Max Combo: {maxCombo}");
        }
        #endif
    }
}
