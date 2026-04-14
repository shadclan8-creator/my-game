using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TimesBaddestCat.Gameplay;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.UI
{
    /// <summary>
    /// GameOverScreen - Displays game over UI with score and options.
    /// Shows final score, high score, and replay options.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Panels")]
        [SerializeField] private GameObject gameoverPanel;

        #endregion

        #region References

        private GameManager gameManager;

        #endregion

        #region State

        private bool isShowing = false;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (gameoverPanel == null) CreateUI();
            SetupButtons();
            gameoverPanel.SetActive(false);
        }

        protected virtual void Start()
        {
            gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnGameOver += ShowGameOver;
            }
        }

        protected virtual void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnGameOver -= ShowGameOver;
            }
        }

        #endregion

        #region UI Creation

        private void CreateUI()
        {
            // Create Canvas
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 300; // Above pause menu
            GameObject canvasObj = canvas.gameObject;

            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add Graphic Raycaster
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create game over panel
            gameoverPanel = new GameObject("GameOverPanel");
            gameoverPanel.transform.SetParent(canvas.transform, false);

            RectTransform rect = gameoverPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            // Background
            CreateBackground(gameoverPanel.transform);

            // Create content
            CreateContent(gameoverPanel.transform);
        }

        private void CreateBackground(Transform parent)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(parent, false);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.85f);
        }

        private void CreateContent(Transform parent)
        {
            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);

            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.7f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(500, 100);

            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "GAME OVER";
            titleText.fontSize = 64;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(1f, 0.3f, 0.3f);

            // Score display
            GameObject scoreObj = new GameObject("Score");
            scoreObj.transform.SetParent(parent, false);

            RectTransform scoreRect = scoreObj.AddComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
            scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
            scoreRect.pivot = new Vector2(0.5f, 0.5f);
            scoreRect.anchoredPosition = Vector2.zero;
            scoreRect.sizeDelta = new Vector2(400, 80);

            scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreText.text = "SCORE: 0";
            scoreText.fontSize = 36;
            scoreText.alignment = TextAlignmentOptions.Center;
            scoreText.color = Color.white;

            // High score display
            GameObject highScoreObj = new GameObject("HighScore");
            highScoreObj.transform.SetParent(parent, false);

            RectTransform highScoreRect = highScoreObj.AddComponent<RectTransform>();
            highScoreRect.anchorMin = new Vector2(0.5f, 0.4f);
            highScoreRect.anchorMax = new Vector2(0.5f, 0.4f);
            highScoreRect.pivot = new Vector2(0.5f, 0.5f);
            highScoreRect.anchoredPosition = Vector2.zero;
            highScoreRect.sizeDelta = new Vector2(400, 50);

            highScoreText = highScoreObj.AddComponent<TextMeshProUGUI>();
            highScoreText.text = "HIGH SCORE: 0";
            highScoreText.fontSize = 24;
            highScoreText.alignment = TextAlignmentOptions.Center;
            highScoreText.color = new Color(1f, 0.8f, 0f);

            // Buttons
            float startY = 0.25f;
            float buttonSpacing = 0.1f;

            retryButton = CreateButton(parent, "RETRY", startY);
            mainMenuButton = CreateButton(parent, "MAIN MENU", startY - buttonSpacing);
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
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

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

        #endregion

        #region Button Setup

        private void SetupButtons()
        {
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetry);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenu);
            }
        }

        #endregion

        #region Button Handlers

        private void OnRetry()
        {
            HideGameOver();
            gameManager?.RestartLevel();
        }

        private void OnMainMenu()
        {
            HideGameOver();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        #endregion

        #region Show/Hide

        private void ShowGameOver()
        {
            if (isShowing) return;

            isShowing = true;
            gameoverPanel?.SetActive(true);

            // Update score display
            if (scoreText != null && gameManager != null)
            {
                scoreText.text = $"SCORE: {gameManager.TotalScore}";
            }

            // Update high score
            if (highScoreText != null)
            {
                int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
                int finalScore = gameManager?.TotalScore ?? 0;

                if (finalScore > currentHighScore)
                {
                    currentHighScore = finalScore;
                    PlayerPrefs.SetInt("HighScore", currentHighScore);
                    PlayerPrefs.Save();
                }

                highScoreText.text = $"HIGH SCORE: {currentHighScore}";
            }

            // Slow motion effect
            StartCoroutine(SlowMotionEffect());
        }

        private void HideGameOver()
        {
            isShowing = false;
            gameoverPanel?.SetActive(false);
            Time.timeScale = 1f;
        }

        private IEnumerator SlowMotionEffect()
        {
            float targetScale = 0.1f;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(1f, targetScale, elapsed / duration);
                yield return null;
            }

            Time.timeScale = targetScale;
        }

        #endregion

        #region Public API

        public bool IsShowing => isShowing;

        #endregion
    }
}
