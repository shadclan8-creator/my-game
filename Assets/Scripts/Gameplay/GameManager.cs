using System;
using System.Collections;
using UnityEngine;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// GameManager - Central game state management for Time's Baddest Cat.
    /// Manages game flow, scoring, level loading, and session state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Serialized Fields

        [Header("Game State")]
        [SerializeField] private bool isPaused = false;
        [SerializeField] private bool isGameActive = false;
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int totalScore = 0;

        [Header("References")]
        [SerializeField] private PlayerController player;

        #endregion

        #region Public Events

        public event Action<bool> OnPauseChanged;
        public event Action<int> OnScoreChanged;
        public event Action OnLevelComplete;
        public event Action OnGameOver;
        public event Action OnGameStart;

        #endregion

        #region Properties

        public bool IsPaused => isPaused;
        public bool IsGameActive => isGameActive;
        public int CurrentLevel => currentLevel;
        public int TotalScore => totalScore;

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
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Start()
        {
            // Find player if not set
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }

            // Auto-start game for MVP
            StartGame();
        }

        protected virtual void Update()
        {
            HandlePauseInput();
        }

        #endregion

        #region Game Flow

        public void StartGame()
        {
            isGameActive = true;
            isPaused = false;
            currentLevel = 1;
            totalScore = 0;

            OnGameStart?.Invoke();
            Debug.Log("Game Started!");
        }

        public void PauseGame()
        {
            if (!isGameActive) return;

            isPaused = true;
            Time.timeScale = 0f;
            OnPauseChanged?.Invoke(true);
            Debug.Log("Game Paused");
        }

        public void ResumeGame()
        {
            if (!isGameActive) return;

            isPaused = false;
            Time.timeScale = 1f;
            OnPauseChanged?.Invoke(false);
            Debug.Log("Game Resumed");
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void GameOver()
        {
            isGameActive = false;
            isPaused = false;
            Time.timeScale = 1f;

            OnGameOver?.Invoke();
            Debug.Log("Game Over!");
        }

        public void LevelComplete()
        {
            currentLevel++;
            OnLevelComplete?.Invoke();
            Debug.Log($"Level Complete! Next level: {currentLevel}");
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            isPaused = false;
            isGameActive = true;

            // Reset player
            if (player != null)
            {
                player.transform.position = Vector3.zero;
            }

            Debug.Log("Level Restarted");
        }

        #endregion

        #region Scoring

        public void AddScore(int points)
        {
            totalScore += points;
            OnScoreChanged?.Invoke(totalScore);
        }

        public void ResetScore()
        {
            totalScore = 0;
            OnScoreChanged?.Invoke(0);
        }

        #endregion

        #region Input Handling

        private void HandlePauseInput()
        {
            // Escape to pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        #endregion

        #region Player Management

        public void SetPlayer(PlayerController playerController)
        {
            player = playerController;
        }

        public PlayerController GetPlayer()
        {
            return player;
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        [Header("Debug Info")]
        private void OnGUI()
        {
            GUILayout.Label($"Game Active: {isGameActive}");
            GUILayout.Label($"Paused: {isPaused}");
            GUILayout.Label($"Level: {currentLevel}");
            GUILayout.Label($"Score: {totalScore}");
            GUILayout.Label($"Time Scale: {Time.timeScale}");

            if (GUILayout.Button("Toggle Pause"))
            {
                TogglePause();
            }

            if (GUILayout.Button("Restart Level"))
            {
                RestartLevel();
            }
        }
        #endif
    }
}
