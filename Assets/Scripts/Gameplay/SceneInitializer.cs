using UnityEngine;
using TimesBaddestCat.Foundation;
using TimesBaddestCat.Core;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// SceneInitializer - Initializes the game scene with all required systems.
    /// This replaces the complex Unity scene file with runtime setup.
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        [Header("Setup Settings")]
        [SerializeField] private bool setupOnStart = true;

        private void Start()
        {
            if (setupOnStart)
            {
                SetupGameScene();
            }
        }

        private void SetupGameScene()
        {
            // Create game manager if not exists
            if (FindObjectOfType<GameManager>() == null)
            {
                GameObject gm = new GameObject("GameManager");
                gm.AddComponent<GameManager>();
            }

            // Create lighting
            SetupLighting();

            // Create HUD
            if (FindObjectOfType<HUDController>() == null)
            {
                GameObject hud = new GameObject("HUD");
                hud.AddComponent<HUDController>();
            }

            // Find or create player
            SetupPlayer();

            // Find or create camera
            SetupCamera();

            // Find or create level
            SetupLevel();

            Debug.Log("Scene initialized!");
        }

        private void SetupLighting()
        {
            GameObject lightObj = GameObject.Find("DirectionalLight");
            if (lightObj == null)
            {
                lightObj = new GameObject("DirectionalLight");
            }

            if (lightObj.GetComponent<Light>() == null)
            {
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                light.shadows = LightShadows.Soft;
                light.color = new Color(1f, 0.95f, 0.9f);
                lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
            }

            RenderSettings.ambientLight = new Color(0.4f, 0.45f, 0.5f);
            RenderSettings.ambientIntensity = 0.5f;
        }

        private void SetupPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = new GameObject("Player");
                player.tag = "Player";
                player.layer = LayerMask.NameToLayer("Player");
                player.transform.position = new Vector3(0, 1f, 0);

                // Add player setup
                PlayerSetup setup = player.AddComponent<PlayerSetup>();
                setup.SetupOnAwake = false;
                setup.SetupPlayer();

                // Create player visual
                CreatePlayerVisual(player.transform);
            }
            else
            {
                // Ensure player has all components
                PlayerSetup setup = player.GetComponent<PlayerSetup>();
                if (setup == null)
                {
                    setup = player.AddComponent<PlayerSetup>();
                    setup.SetupOnAwake = false;
                    setup.SetupPlayer();
                }
            }
        }

        private void CreatePlayerVisual(Transform parent)
        {
            GameObject visual = new GameObject("PlayerVisual");
            visual.transform.SetParent(parent);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(0.5f, 1f, 0.5f);

            // Create capsule visual
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Body";
            capsule.transform.SetParent(visual.transform);
            capsule.transform.localPosition = new Vector3(0, 0.5f, 0);
            capsule.transform.localRotation = Quaternion.Euler(0, 0, 90);
            capsule.transform.localScale = new Vector3(1f, 1f, 1f);
            capsule.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.2f);

            // Create cat ears
            CreateEar(visual.transform, new Vector3(-0.15f, 0.9f, 0), -20);
            CreateEar(visual.transform, new Vector3(0.15f, 0.9f, 0), 20);

            // Create cat tail
            GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tail.name = "Tail";
            tail.transform.SetParent(visual.transform);
            tail.transform.localPosition = new Vector3(0, 0.8f, -0.5f);
            tail.transform.localRotation = Quaternion.Euler(70, 0, 0);
            tail.transform.localScale = new Vector3(0.15f, 0.15f, 0.8f);
            tail.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.2f);
        }

        private void CreateEar(Transform parent, Vector3 position, float rotation)
        {
            GameObject ear = GameObject.CreatePrimitive(PrimitiveType.Cone);
            ear.transform.SetParent(parent);
            ear.transform.localPosition = position;
            ear.transform.localRotation = Quaternion.Euler(0, 0, rotation);
            ear.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);
            ear.GetComponent<Renderer>().material.color = new Color(1f, 0.9f, 0.3f);
        }

        private void SetupCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("MainCamera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                mainCam.backgroundColor = new Color(0.5f, 0.6f, 0.8f);
                mainCam.clearFlags = CameraClearFlags.Skybox;
                mainCam.fieldOfView = 75;
                camObj.AddComponent<AudioListener>();
            }

            // Add camera system if not present
            if (mainCam.GetComponent<CameraSystem>() == null)
            {
                mainCam.gameObject.AddComponent<CameraSystem>();
            }

            // Position camera
            mainCam.transform.position = new Vector3(0, 2f, -5f);
        }

        private void SetupLevel()
        {
            LevelGenerator levelGen = FindObjectOfType<LevelGenerator>();
            if (levelGen == null)
            {
                GameObject levelObj = new GameObject("Level");
                levelGen = levelObj.AddComponent<LevelGenerator>();
            }

            levelGen.GenerateOnStart = true;
        }
    }
}
