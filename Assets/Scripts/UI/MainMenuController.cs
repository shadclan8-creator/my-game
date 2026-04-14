using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TimesBaddestCat.UI
{
    /// <summary>
    /// MainMenuController - Controls the main menu UI.
    /// Handles title display, menu navigation, and game start.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Dropdown resolutionDropdown;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (titleText == null) CreateUI();
            SetupButtons();
        }

        protected virtual void Start()
        {
            LoadSettings();
            UpdateVersionText();
        }

        #endregion

        #region UI Creation

        private void CreateUI()
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
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create main panel
            mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(canvas.transform, false);

            // Create title
            CreateTitle(mainPanel.transform);

            // Create menu buttons
            CreateMenuButtons(mainPanel.transform);

            // Create version text
            versionText = CreateVersionText(mainPanel.transform);

            // Create settings panel (hidden by default)
            CreateSettingsPanel(canvas.transform);
        }

        private void CreateTitle(Transform parent)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);

            RectTransform rect = titleObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.8f);
            rect.anchorMax = new Vector2(0.5f, 0.8f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(600, 100);

            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "TIME'S BADDEST CAT";
            titleText.fontSize = 48;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(1f, 0.8f, 0.2f); // Orange cat color
        }

        private void CreateMenuButtons(Transform parent)
        {
            string[] buttonTexts = { "START GAME", "SETTINGS", "QUIT" };
            float startY = 0.5f;
            float buttonSpacing = 0.1f;
            float buttonHeight = 60;

            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Button button = CreateButton(parent, buttonTexts[i], startY - i * buttonSpacing, buttonHeight);

                switch (i)
                {
                    case 0: startButton = button; break;
                    case 1: settingsButton = button; break;
                    case 2: quitButton = button; break;
                }
            }
        }

        private Button CreateButton(Transform parent, string text, float normalizedY, float height)
        {
            GameObject buttonObj = new GameObject($"Button_{text}");
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, normalizedY);
            rect.anchorMax = new Vector2(0.5f, normalizedY);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(300, height);

            // Create background image
            Image bgImage = buttonObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            // Create button component
            Button button = buttonObj.AddComponent<Button>();

            // Create text
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

        private TextMeshProUGUI CreateVersionText(Transform parent)
        {
            GameObject versionObj = new GameObject("Version");
            versionObj.transform.SetParent(parent, false);

            RectTransform rect = versionObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.05f);
            rect.anchorMax = new Vector2(0.5f, 0.05f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 30);

            TextMeshProUGUI versionText = versionObj.AddComponent<TextMeshProUGUI>();
            versionText.fontSize = 14;
            versionText.alignment = TextAlignmentOptions.Center;
            versionText.color = new Color(0.6f, 0.6f, 0.6f);

            return versionText;
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

            // Create settings controls
            CreateSettingsSliders(bgObj.transform);
            CreateSettingsToggles(bgObj.transform);
            CreateBackButton(bgObj.transform);
        }

        private void CreateSettingsSliders(Transform parent)
        {
            float startY = 0.7f;
            float spacing = 0.15f;

            // Master volume
            masterVolumeSlider = CreateVolumeSlider(parent, "Master Volume", startY);
            // Music volume
            musicVolumeSlider = CreateVolumeSlider(parent, "Music Volume", startY - spacing);
            // SFX volume
            sfxVolumeSlider = CreateVolumeSlider(parent, "SFX Volume", startY - spacing * 2);
        }

        private Slider CreateVolumeSlider(Transform parent, string label, float normalizedY)
        {
            GameObject container = new GameObject($"Slider_{label.Replace(" ", "")}");
            container.transform.SetParent(parent, false);

            RectTransform rect = container.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, normalizedY);
            rect.anchorMax = new Vector2(0.8f, normalizedY);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 40);

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(container.transform, false);

            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0.3f, 0.5f);
            labelRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 18;
            labelText.alignment = TextAlignmentOptions.Left;
            labelText.color = Color.white;

            // Slider
            GameObject sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(container.transform, false);

            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.4f, 0.5f);
            sliderRect.anchorMax = new Vector2(1f, 0.5f);
            sliderRect.sizeDelta = Vector2.zero;

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.7f;

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = new Vector2(-10, 0);

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);

            // Fill area
            GameObject fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform, false);

            RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);

            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.sizeDelta = Vector2(0, -4);

            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(1f, 0.8f, 0.2f);
            slider.fillRect = fillRect;

            // Handle
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(sliderObj.transform, false);

            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = Color.white;
            slider.handleRect = handleRect;

            return slider;
        }

        private void CreateSettingsToggles(Transform parent)
        {
            float startY = 0.3f;
            float spacing = 0.12f;

            // Fullscreen toggle
            fullscreenToggle = CreateToggle(parent, "Fullscreen", startY);
        }

        private Toggle CreateToggle(Transform parent, string label, float normalizedY)
        {
            GameObject container = new GameObject($"Toggle_{label.Replace(" ", "")}");
            container.transform.SetParent(parent, false);

            RectTransform rect = container.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, normalizedY);
            rect.anchorMax = new Vector2(0.8f, normalizedY);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 40);

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(container.transform, false);

            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 18;
            labelText.alignment = TextAlignmentOptions.Left;
            labelText.color = Color.white;

            // Toggle
            GameObject toggleObj = new GameObject("Toggle");
            toggleObj.transform.SetParent(container.transform, false);

            RectTransform toggleRect = toggleObj.AddComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.6f, 0.5f);
            toggleRect.anchorMax = new Vector2(0.6f, 0.5f);
            toggleRect.sizeDelta = new Vector2(30, 30);

            Toggle toggle = toggleObj.AddComponent<Toggle>();
            toggle.isOn = Screen.fullScreen;

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(toggleObj.transform, false);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = new Vector2(-6, -6);

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);
            toggle.targetGraphic = bgImage;

            // Checkmark
            GameObject checkObj = new GameObject("Checkmark");
            checkObj.transform.SetParent(toggleObj.transform, false);

            RectTransform checkRect = checkObj.AddComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0, 0.5f);
            checkRect.anchorMax = new Vector2(1, 0.5f);
            checkRect.sizeDelta = new Vector2(0, 0);

            Image checkImage = checkObj.AddComponent<Image>();
            checkImage.color = new Color(1f, 0.8f, 0.2f);
            toggle.graphic = checkImage;

            return toggle;
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
                mainPanel.SetActive(true);
            });
        }

        #endregion

        #region Button Setup

        private void SetupButtons()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartGame);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettings);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuit);
            }
        }

        #endregion

        #region Button Handlers

        private void OnStartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }

        private void OnSettings()
        {
            if (mainPanel != null && settingsPanel != null)
            {
                mainPanel.SetActive(false);
                settingsPanel.SetActive(true);
            }
        }

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion

        #region Settings

        private void LoadSettings()
        {
            // Load volume settings
            float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

            if (masterVolumeSlider != null) masterVolumeSlider.value = masterVol;
            if (musicVolumeSlider != null) musicVolumeSlider.value = musicVol;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVol;

            // Load fullscreen setting
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

            // Apply settings
            ApplySettings();
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider?.value ?? 1f);
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider?.value ?? 0.7f);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider?.value ?? 1f);
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle?.isOn == true ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void ApplySettings()
        {
            // Apply volumes
            Foundation.AudioManager audioMgr = Foundation.AudioManager.Instance;
            if (audioMgr != null)
            {
                audioMgr.SetMasterVolume(masterVolumeSlider?.value ?? 1f);
                audioMgr.SetMusicVolume(musicVolumeSlider?.value ?? 0.7f);
                audioMgr.SetSFXVolume(sfxVolumeSlider?.value ?? 1f);
            }

            // Apply fullscreen
            Screen.fullScreen = fullscreenToggle?.isOn ?? true;
        }

        #endregion

        #region Version

        private void UpdateVersionText()
        {
            if (versionText != null)
            {
                versionText.text = $"v{Application.version} - Build {Application.buildNumber}";
            }
        }

        #endregion
    }
}
