using System;
using System.Collections.Generic;
using UnityEngine;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Core
{
    /// <summary>
    /// Enemy AI Provider - Manages all enemy AI instances in the scene.
    /// Provides centralized enemy management for player combat.
    /// </summary>
    public class EnemyAIProvider : MonoBehaviour, IEnemyAIProvider
    {
        #region Singleton

        private static EnemyAIProvider _instance;
        public static EnemyAIProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EnemyAIProvider>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("EnemyAIProvider");
                        _instance = go.AddComponent<EnemyAIProvider>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Serialized Fields

        [Header("Enemy Management")]
        [SerializeField]
        private List<EnemyAI> activeEnemies = new List<EnemyAI>();

        #endregion

        #region Public Events

        public event Action<int> OnEnemyCountChanged;
        public event Action<EnemyAI> OnEnemySpawned;
        public event Action<EnemyAI> OnEnemyKilled;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        protected virtual void Start()
        {
            // Find any existing enemies in the scene
            EnemyAI[] existingEnemies = FindObjectsOfType<EnemyAI>();
            foreach (var enemy in existingEnemies)
            {
                RegisterEnemy(enemy);
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }

        #endregion

        #region IEnemyAIProvider Implementation

        public void RegisterEnemy(EnemyAI enemy)
        {
            if (enemy == null) return;
            if (activeEnemies.Contains(enemy)) return;

            activeEnemies.Add(enemy);

            // Subscribe to enemy death event
            enemy.OnEnemyDeath += HandleEnemyDeath;

            OnEnemyCountChanged?.Invoke(activeEnemies.Count);
            OnEnemySpawned?.Invoke(enemy);

            Debug.Log($"Enemy {enemy.name} registered. Total enemies: {activeEnemies.Count}");
        }

        public void UnregisterEnemy(EnemyAI enemy)
        {
            if (enemy == null) return;
            if (!activeEnemies.Contains(enemy)) return;

            activeEnemies.Remove(enemy);

            // Unsubscribe from enemy death event
            enemy.OnEnemyDeath -= HandleEnemyDeath;

            OnEnemyCountChanged?.Invoke(activeEnemies.Count);

            Debug.Log($"Enemy {enemy.name} unregistered. Remaining enemies: {activeEnemies.Count}");
        }

        public EnemyAI GetNearestEnemy(Vector3 position, float maxDistance)
        {
            EnemyAI nearest = null;
            float nearestDistance = maxDistance;

            foreach (var enemy in activeEnemies)
            {
                if (enemy == null || enemy.IsDead()) continue;

                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        public void AlertEnemiesInRange(Vector3 position, float radius)
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy == null || enemy.IsDead()) continue;

                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance <= radius)
                {
                    // Enemy would switch to alerted/chase state
                    Debug.Log($"Alerting enemy {enemy.name} at distance {distance:F2}");
                }
            }
        }

        #endregion

        #region Event Handlers

        private void HandleEnemyDeath(EnemyAI enemy)
        {
            OnEnemyKilled?.Invoke(enemy);
            UnregisterEnemy(enemy);
        }

        #endregion

        #region Public API

        public int GetEnemyCount() => activeEnemies.Count;
        public List<EnemyAI> GetAllEnemies() => new List<EnemyAI>(activeEnemies);
        public List<EnemyAI> GetEnemiesInRange(Vector3 position, float radius)
        {
            List<EnemyAI> enemiesInRange = new List<EnemyAI>();
            foreach (var enemy in activeEnemies)
            {
                if (enemy == null || enemy.IsDead()) continue;

                if (Vector3.Distance(position, enemy.transform.position) <= radius)
                {
                    enemiesInRange.Add(enemy);
                }
            }
            return enemiesInRange;
        }

        public void ClearAllEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.OnEnemyDeath -= HandleEnemyDeath;
                }
            }
            activeEnemies.Clear();
            OnEnemyCountChanged?.Invoke(0);
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Active Enemies: {activeEnemies.Count}");
            GUILayout.Label($"Nearest Enemy: {GetNearestEnemy(Vector3.zero, 100f)?.name ?? "None"}");
        }
        #endif
    }
}
