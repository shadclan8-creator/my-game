using UnityEngine;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// SceneSetup - Sets up the entire game scene with all required GameObjects.
    /// Run this to create a complete playable scene.
    /// </summary>
    public class SceneSetup : MonoBehaviour
    {
        [Header("Setup Settings")]
        [SerializeField] private bool setupOnAwake = true;

        [Header("Generated Objects")]
        [SerializeField] private GameObject playerObject;
        [SerializeField] private GameObject cameraObject;
        [SerializeField] private GameObject levelObject;
        [SerializeField] private GameObject hudObject;
        [SerializeField] private GameObject gameManagerObject;

        protected virtual void Awake()
        {
            if (setupOnAwake)
            {
                SetupScene();
            }
        }

        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            // Clear existing setup if any
            ClearExistingSetup();

            // Create game manager
            CreateGameManager();

            // Create level
            CreateLevel();

            // Create player
            CreatePlayer();

            // Create camera
            CreateCamera();

            // Create HUD
            CreateHUD();

            // Setup lighting
            SetupLighting();

            Debug.Log("Scene setup complete!");
        }

        private void ClearExistingSetup()
        {
            // Find and destroy existing setup objects
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) DestroyImmediate(player);

            var manager = FindObjectOfType<GameManager>();
            if (manager != null) DestroyImmediate(manager.gameObject);

            var levelGen = FindObjectOfType<LevelGenerator>();
            if (levelGen != null) DestroyImmediate(levelGen.gameObject);

            var hud = FindObjectOfType<HUDController>();
            if (hud != null) DestroyImmediate(hud.gameObject);
        }

        private void CreateGameManager()
        {
            gameManagerObject = new GameObject("GameManager");
            gameManagerObject.AddComponent<GameManager>();
        }

        private void CreatePlayer()
        {
            playerObject = new GameObject("Player");
            playerObject.tag = "Player";
            playerObject.layer = LayerMask.NameToLayer("Player");

            // Add player setup
            PlayerSetup setup = playerObject.AddComponent<PlayerSetup>();
            setup.SetupPlayer();

            // Add character controller or rigidbody
            Rigidbody rb = playerObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = playerObject.AddComponent<Rigidbody>();
                rb.mass = 70f;
                rb.drag = 0f;
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            // Add capsule collider
            CapsuleCollider col = playerObject.GetComponent<CapsuleCollider>();
            if (col == null)
            {
                col = playerObject.AddComponent<CapsuleCollider>();
                col.height = 2f;
                col.radius = 0.5f;
                col.center = new Vector3(0, 1f, 0);
            }

            // Add visual
            GameObject playerVisual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerVisual.name = "PlayerVisual";
            playerVisual.transform.SetParent(playerObject.transform);
            playerVisual.transform.localPosition = Vector3.zero;
            playerVisual.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            playerVisual.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.2f); // Orange cat

            // Add ears
            GameObject leftEar = GameObject.CreatePrimitive(PrimitiveType.Cone);
            leftEar.name = "LeftEar";
            leftEar.transform.SetParent(playerVisual.transform);
            leftEar.transform.localPosition = new Vector3(-0.15f, 0.8f, 0);
            leftEar.transform.localRotation = Quaternion.Euler(0, 0, -20);
            leftEar.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);

            GameObject rightEar = GameObject.CreatePrimitive(PrimitiveType.Cone);
            rightEar.name = "RightEar";
            rightEar.transform.SetParent(playerVisual.transform);
            rightEar.transform.localPosition = new Vector3(0.15f, 0.8f, 0);
            rightEar.transform.localRotation = Quaternion.Euler(0, 0, 20);
            rightEar.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);

            // Position at spawn point
            playerObject.transform.position = new Vector3(0, 1f, 0);
        }

        private void CreateCamera()
        {
            cameraObject = new GameObject("MainCamera");
            cameraObject.tag = "MainCamera";

            Camera cam = cameraObject.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.backgroundColor = new Color(0.5f, 0.6f, 0.8f);
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 1000f;

            AudioListener listener = cameraObject.AddComponent<AudioListener>();

            // Add camera system
            Core.CameraSystem camSystem = cameraObject.AddComponent<Core.CameraSystem>();

            // Position behind player
            cameraObject.transform.position = new Vector3(0, 2f, -5f);
            cameraObject.transform.LookAt(playerObject != null ? playerObject.transform.position : Vector3.zero);
        }

        private void CreateLevel()
        {
            levelObject = new GameObject("LevelRoot");
            LevelGenerator generator = levelObject.AddComponent<LevelGenerator>();
            generator.GenerateOnStart = true;
        }

        private void CreateHUD()
        {
            hudObject = new GameObject("HUD");
            hudObject.AddComponent<HUDController>();
        }

        private void SetupLighting()
        {
            // Main directional light
            GameObject lightObj = new GameObject("DirectionalLight");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            light.color = new Color(1f, 0.95f, 0.9f); // Warm daylight

            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            // Ambient light
            RenderSettings.ambientLight = new Color(0.4f, 0.45f, 0.5f);
            RenderSettings.ambientIntensity = 0.5f;

            // Skybox (use default)
        }
    }
}
