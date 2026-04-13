using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// HUDController - Manages all HUD elements for the game.
    /// Displays health, ammo, combo, and score information.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Health Display")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private UnityEngine.UI.Image healthBar;

        [Header("Ammo Display")]
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private UnityEngine.UI.Image ammoBar;

        [Header("Combo Display")]
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private CanvasGroup comboPanel;

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Crosshair")]
        [SerializeField] private RectTransform crosshair;
        [SerializeField] private float crosshairSize = 20f;

        [Header("References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private ICombatProvider combatSystem;
        [SerializeField] private IComboProvider comboSystem;
        [SerializeField] private GameManager gameManager;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            // Create HUD if not set
            if (healthText == null) CreateHUD();

            // Find references
            FindReferences();
        }

        protected virtual void Start()
        {
            // Subscribe to events
            SubscribeToEvents();

            // Initial update
            UpdateHUD();
        }

        protected virtual void Update()
        {
            UpdateHUD();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        #endregion

        #region Initialization

        private void FindReferences()
        {
            if (player == null) player = FindObjectOfType<PlayerController>();
            if (combatSystem == null) combatSystem = FindObjectOfType<ICombatProvider>();
            if (comboSystem == null) comboSystem = FindObjectOfType<IComboProvider>();
            if (gameManager == null) gameManager = GameManager.Instance;
        }

        private void SubscribeToEvents()
        {
            if (player != null)
            {
                // Player.OnHealthChanged would go here
            }

            if (gameManager != null)
            {
                gameManager.OnScoreChanged += UpdateScoreDisplay;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (gameManager != null)
            {
                gameManager.OnScoreChanged -= UpdateScoreDisplay;
            }
        }

        private void CreateHUD()
        {
            // Create Canvas
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            GameObject canvasObj = canvas.gameObject;

            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add Graphic Raycaster
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Create HUD container
            GameObject hudContainer = new GameObject("HUDContainer");
            hudContainer.transform.SetParent(canvas.transform, false);

            // Create Health Display (bottom left)
            CreateHealthDisplay(hudContainer);

            // Create Ammo Display (bottom right)
            CreateAmmoDisplay(hudContainer);

            // Create Combo Display (center top)
            CreateComboDisplay(hudContainer);

            // Create Score Display (top left)
            CreateScoreDisplay(hudContainer);

            // Create Crosshair (center)
            CreateCrosshair(canvas.transform);
        }

        private void CreateHealthDisplay(GameObject parent)
        {
            GameObject healthObj = new GameObject("HealthDisplay");
            healthObj.transform.SetParent(parent.transform, false);
            RectTransform rect = healthObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(20, 20);
            rect.sizeDelta = new Vector2(200, 50);

            // Health bar background
            GameObject bgObj = new GameObject("HealthBarBG");
            bgObj.transform.SetParent(healthObj.transform, false);
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Health bar fill
            GameObject fillObj = new GameObject("HealthBarFill");
            fillObj.transform.SetParent(healthObj.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.sizeDelta = new Vector2(-4, -4);

            healthBar = fillObj.AddComponent<UnityEngine.UI.Image>();
            healthBar.color = Color.red;
            healthBar.type = UnityEngine.UI.Image.Type.Filled;
            healthBar.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            healthBar.fillAmount = 1f;

            // Health text
            GameObject textObj = new GameObject("HealthText");
            textObj.transform.SetParent(healthObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            healthText = textObj.AddComponent<TextMeshProUGUI>();
            healthText.text = "100 / 100";
            healthText.fontSize = 24;
            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.white;
        }

        private void CreateAmmoDisplay(GameObject parent)
        {
            GameObject ammoObj = new GameObject("AmmoDisplay");
            ammoObj.transform.SetParent(parent.transform, false);
            RectTransform rect = ammoObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(-20, 20);
            rect.sizeDelta = new Vector2(150, 50);

            // Ammo text
            GameObject textObj = new GameObject("AmmoText");
            textObj.transform.SetParent(ammoObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            ammoText = textObj.AddComponent<TextMeshProUGUI>();
            ammoText.text = "30 / 30";
            ammoText.fontSize = 24;
            ammoText.alignment = TextAlignmentOptions.MiddleRight;
            ammoText.color = Color.white;
        }

        private void CreateComboDisplay(GameObject parent)
        {
            GameObject comboObj = new GameObject("ComboDisplay");
            comboObj.transform.SetParent(parent.transform, false);
            RectTransform rect = comboObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -20);
            rect.sizeDelta = new Vector2(300, 100);

            comboPanel = comboObj.AddComponent<CanvasGroup>();
            comboPanel.alpha = 0f;

            // Combo text
            GameObject comboTextObj = new GameObject("ComboText");
            comboTextObj.transform.SetParent(comboObj.transform, false);
            RectTransform comboTextRect = comboTextObj.AddComponent<RectTransform>();
            comboTextRect.anchorMin = new Vector2(0, 0.5f);
            comboTextRect.anchorMax = new Vector2(1, 0.5f);
            comboTextRect.sizeDelta = new Vector2(0, 50);

            comboText = comboTextObj.AddComponent<TextMeshProUGUI>();
            comboText.text = "COMBO x0";
            comboText.fontSize = 36;
            comboText.alignment = TextAlignmentOptions.Center;
            comboText.color = new Color(1f, 0.8f, 0f); // Gold
            comboText.fontStyle = FontStyles.Bold;

            // Multiplier text
            GameObject multTextObj = new GameObject("MultiplierText");
            multTextObj.transform.SetParent(comboObj.transform, false);
            RectTransform multTextRect = multTextObj.AddComponent<RectTransform>();
            multTextRect.anchorMin = new Vector2(0, 0);
            multTextRect.anchorMax = new Vector2(1, 0.5f);
            multTextRect.sizeDelta = new Vector2(0, 40);

            multiplierText = multTextObj.AddComponent<TextMeshProUGUI>();
            multiplierText.text = "x1.0";
            multiplierText.fontSize = 28;
            multiplierText.alignment = TextAlignmentOptions.Center;
            multiplierText.color = Color.cyan;
        }

        private void CreateScoreDisplay(GameObject parent)
        {
            GameObject scoreObj = new GameObject("ScoreDisplay");
            scoreObj.transform.SetParent(parent.transform, false);
            RectTransform rect = scoreObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(20, -20);
            rect.sizeDelta = new Vector2(200, 50);

            // Score text
            GameObject textObj = new GameObject("ScoreText");
            textObj.transform.SetParent(scoreObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            scoreText = textObj.AddComponent<TextMeshProUGUI>();
            scoreText.text = "SCORE: 0";
            scoreText.fontSize = 24;
            scoreText.alignment = TextAlignmentOptions.Left;
            scoreText.color = Color.white;
        }

        private void CreateCrosshair(Transform parent)
        {
            GameObject crosshairObj = new GameObject("Crosshair");
            crosshairObj.transform.SetParent(parent, false);
            RectTransform rect = crosshairObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(crosshairSize, crosshairSize);

            crosshair = rect;

            // Create crosshair image
            UnityEngine.UI.Image image = crosshairObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(1f, 1f, 1f, 0.8f);

            // Note: In production, use a sprite for the crosshair
            // For now, we'll use a small square
        }

        #endregion

        #region HUD Updates

        private void UpdateHUD()
        {
            UpdateHealthDisplay();
            UpdateAmmoDisplay();
            UpdateComboDisplay();
            UpdateScoreDisplay();
            UpdateCrosshair();
        }

        private void UpdateHealthDisplay()
        {
            if (player != null && healthText != null && healthBar != null)
            {
                // This would get actual health from player
                float healthPercent = 0.8f; // Placeholder
                healthText.text = $"80 / 100";
                healthBar.fillAmount = healthPercent;
            }
        }

        private void UpdateAmmoDisplay()
        {
            if (combatSystem != null && ammoText != null)
            {
                int current = combatSystem.GetCurrentAmmo();
                int max = combatSystem.GetMaxAmmo();
                ammoText.text = $"{current} / {max}";
            }
        }

        private void UpdateComboDisplay()
        {
            if (comboSystem != null && comboText != null && multiplierText != null && comboPanel != null)
            {
                int combo = comboSystem.GetCurrentCombo();
                float multiplier = comboSystem.GetComboMultiplier();

                comboText.text = $"COMBO x{combo}";
                multiplierText.text = $"x{multiplier:F1}";

                // Fade in/out based on combo
                comboPanel.alpha = combo > 0 ? 1f : 0f;
            }
        }

        private void UpdateScoreDisplay()
        {
            if (gameManager != null && scoreText != null)
            {
                scoreText.text = $"SCORE: {gameManager.TotalScore}";
            }
        }

        private void UpdateCrosshair()
        {
            // Crosshair could expand when shooting, etc.
        }

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE: {score}";
            }
        }

        #endregion
    }
}
