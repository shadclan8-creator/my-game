using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TimesBaddestCat.UI
{
    /// <summary>
    /// PauseMenuController - Controls the pause menu UI.
    /// Allows pausing, resuming, and restarting the game.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;

        #endregion

        #region State

        private bool isPaused = false;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (pausePanel == null) CreateUI();
            SetupButtons();
            pausePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        protected virtual void Update()
        {
            // Check for pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        #endregion

        #region UI Creation

        private void CreateUI()
        {
            // Create Canvas
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200; // Above HUD
            GameObject canvasObj = canvas.gameObject;

            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add Graphic Raycaster
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create pause panel
            pausePanel = new GameObject("PausePanel");
            pausePanel.transform.SetParent(canvas.transform, false);

            RectTransform rect = pausePanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(pausePanel.transform, false);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.7f);

            // Create menu content
            CreatePauseContent(pausePanel.transform);

            // Create settings panel (hidden by default)
            CreateSettingsPanel(canvas.transform);
        }

        private void CreatePauseContent(Transform parent)
        {
            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);

            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.7f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(400, 80);

            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "PAUSED";
            titleText.fontSize = 48;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;

            // Buttons
            string[] buttonTexts = { "RESUME", "RESTART", "SETTINGS", "MAIN MENU" };
            float startY = 0.55f;
            float buttonSpacing = 0.1f;

            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Button button = CreateButton(parent, buttonTexts[i], startY - i * buttonSpacing);

                switch (i)
                {
                    case 0: resumeButton = button; break;
                    case 1: restartButton = button; break;
                    case 2: settingsButton = button; break;
                    case 3: mainMenuButton = button; break;
                }
            }
        }

        private Button CreateButton(Transform parent, string text, float normalizedY)
        {
            GameObject buttonObj = new GameObject($"Button_{text.Replace(" ", "")}");
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, normalizedY);
            rect.anchorMax = new Vector2(0.5f, normalizedY);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(300, 60);

            // Background
            Image bgImage = buttonObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            Button button = buttonObj.AddComponent<Button>();

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.fontSize = 24;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;

            return button;
        }

        private void CreateSettingsPanel(Transform parent)
        {
            settingsPanel = new GameObject("SettingsPanel");
            settingsPanel.transform.SetParent(parent, false);
            settingsPanel.SetActive(false);

            RectTransform rect = settingsPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(settingsPanel.transform, false);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.1f, 0.1f);
            bgRect.anchorMax = new Vector2(0.9f, 0.9f);
            bgRect.sizeDelta = Vector2.zero;

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            // Back button
            CreateBackButton(bgObj.transform);
        }

        private void CreateBackButton(Transform parent)
        {
            GameObject buttonObj = new GameObject("BackButton");
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.1f);
            rect.anchorMax = new Vector2(0.5f, 0.1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(200, 50);

            Image bgImage = buttonObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);

            Button button = buttonObj.AddComponent<Button>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "BACK";
            buttonText.fontSize = 20;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;

            button.onClick.AddListener(() => {
                settingsPanel.SetActive(false);
            });
        }

        #endregion

        #region Button Setup

        private void SetupButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResume);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestart);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettings);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenu);
            }
        }

        #endregion

        #region Button Handlers

        private void OnResume()
        {
            Resume();
        }

        private void OnRestart()
        {
            Resume();
            Gameplay.GameManager.Instance?.RestartLevel();
        }

        private void OnSettings()
        {
            if (pausePanel != null && settingsPanel != null)
            {
                pausePanel.SetActive(false);
                settingsPanel.SetActive(true);
            }
        }

        private void OnMainMenu()
        {
            Resume();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        #endregion

        #region Pause Control

        public void TogglePause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (isPaused) return;

            isPaused = true;
            Time.timeScale = 0f;
            pausePanel?.SetActive(true);
            Gameplay.GameManager.Instance?.PauseGame();
        }

        public void Resume()
        {
            if (!isPaused) return;

            isPaused = false;
            Time.timeScale = 1f;
            pausePanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            Gameplay.GameManager.Instance?.ResumeGame();
        }

        #endregion

        #region Public API

        public bool IsPaused => isPaused;

        #endregion
    }
}
