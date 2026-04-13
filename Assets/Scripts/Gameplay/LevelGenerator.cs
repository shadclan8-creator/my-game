using UnityEngine;

namespace TimesBaddestCat.Gameplay
{
    /// <summary>
    /// LevelGenerator - Procedurally generates the 1950s Diner level.
    /// Creates basic geometry, spawn points, and traversable surfaces.
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private float levelSize = 50f;

        [Header("1950s Diner Colors")]
        [SerializeField] private Color floorColor = new Color(0.9f, 0.85f, 0.7f);
        [SerializeField] private Color wallColor = new Color(0.95f, 0.9f, 0.8f);
        [SerializeField] private Color boothColor = new Color(0.8f, 0.3f, 0.3f);
        [SerializeField] private Color counterColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color traversableColor = new Color(0f, 1f, 1f); // Cyan for wall-running

        [Header("Generated Objects")]
        [SerializeField] private Transform levelRoot;

        protected virtual void Start()
        {
            if (generateOnStart)
            {
                Generate1950sDiner();
            }
        }

        [ContextMenu("Generate 1950s Diner")]
        public void Generate1950sDiner()
        {
            if (levelRoot == null)
            {
                GameObject root = new GameObject("1950s_Diner_Level");
                levelRoot = root.transform;
            }
            else
            {
                // Clear existing
                foreach (Transform child in levelRoot)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            // Create floor
            CreateFloor();

            // Create walls
            CreateWalls();

            // Create diner booths
            CreateBooths();

            // Create counter area
            CreateCounter();

            // Create traversable surfaces (for wall-running)
            CreateTraversableSurfaces();

            // Create spawn points
            CreateSpawnPoints();

            Debug.Log("1950s Diner level generated!");
        }

        private void CreateFloor()
        {
            GameObject floor = CreatePrimitive("Floor", new Vector3(0, -0.5f, 0), Quaternion.identity, new Vector3(levelSize, 1f, levelSize));
            floor.GetComponent<Renderer>().material.color = floorColor;
            floor.layer = LayerMask.NameToLayer("Ground");
            floor.tag = "Ground";
        }

        private void CreateWalls()
        {
            float wallHeight = 8f;
            float wallThickness = 1f;

            // North wall
            CreateWall("Wall_North", new Vector3(0, wallHeight / 2, -levelSize / 2), new Vector3(levelSize, wallHeight, wallThickness), wallColor);

            // South wall
            CreateWall("Wall_South", new Vector3(0, wallHeight / 2, levelSize / 2), new Vector3(levelSize, wallHeight, wallThickness), wallColor);

            // East wall
            CreateWall("Wall_East", new Vector3(levelSize / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, levelSize), wallColor);

            // West wall
            CreateWall("Wall_West", new Vector3(-levelSize / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, levelSize), wallColor);
        }

        private void CreateWall(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject wall = CreatePrimitive(name, position, Quaternion.identity, scale);
            wall.GetComponent<Renderer>().material.color = color;
            wall.layer = LayerMask.NameToLayer("Environment");
        }

        private void CreateBooths()
        {
            // Create booth seating along the walls
            int boothCount = 6;
            float boothSpacing = levelSize / (boothCount + 1);

            for (int i = 0; i < boothCount; i++)
            {
                float zPos = -levelSize / 2 + boothSpacing * (i + 1);

                // Booth on left side
                CreateBooth($"Booth_L_{i}", new Vector3(-levelSize / 2 + 2f, 0.5f, zPos), boothColor);

                // Booth on right side
                CreateBooth($"Booth_R_{i}", new Vector3(levelSize / 2 - 2f, 0.5f, zPos), boothColor);
            }
        }

        private void CreateBooth(string name, Vector3 position, Color color)
        {
            // Create booth seating
            GameObject booth = CreatePrimitive(name, position, Quaternion.identity, new Vector3(2f, 1f, 3f));
            booth.GetComponent<Renderer>().material.color = color;
            booth.layer = LayerMask.NameToLayer("Environment");

            // Create table
            GameObject table = CreatePrimitive($"{name}_Table", new Vector3(position.x, 1f, position.z + 2f), Quaternion.identity, new Vector3(1.5f, 0.1f, 1f));
            table.GetComponent<Renderer>().material.color = counterColor;
            table.transform.parent = booth.transform;
            table.layer = LayerMask.NameToLayer("Environment");
        }

        private void CreateCounter()
        {
            // Main counter
            GameObject counter = CreatePrimitive("Counter", new Vector3(0, 1f, 10f), Quaternion.identity, new Vector3(30f, 2f, 2f));
            counter.GetComponent<Renderer>().material.color = counterColor;
            counter.layer = LayerMask.NameToLayer("Environment");

            // Counter stools
            for (int i = 0; i < 8; i++)
            {
                float xPos = -10f + i * 3f;
                CreateStool($"Stool_{i}", new Vector3(xPos, 0.75f, 8f));
            }
        }

        private void CreateStool(string name, Vector3 position)
        {
            GameObject stool = CreatePrimitive(name, position, Quaternion.identity, new Vector3(0.5f, 1.5f, 0.5f));
            stool.GetComponent<Renderer>().material.color = boothColor;
            stool.layer = LayerMask.NameToLayer("Environment");
        }

        private void CreateTraversableSurfaces()
        {
            // Create cyan-marked surfaces for wall-running
            // These are vertical strips on walls where the cat can wall-run

            float wallHeight = 4f;
            float traversableWidth = 3f;
            int traversableCount = 4;
            float spacing = levelSize / (traversableCount + 1);

            for (int i = 0; i < traversableCount; i++)
            {
                float zPos = -levelSize / 2 + spacing * (i + 1);

                // Traversable surface on north wall
                CreateTraversableSurface($"Traversable_N_{i}", new Vector3(0, wallHeight, -levelSize / 2 + 0.5f), new Vector3(traversableWidth, wallHeight * 2, 0.2f));

                // Traversable surface on south wall
                CreateTraversableSurface($"Traversable_S_{i}", new Vector3(0, wallHeight, levelSize / 2 - 0.5f), new Vector3(traversableWidth, wallHeight * 2, 0.2f));

                // Traversable surface on east wall
                CreateTraversableSurface($"Traversable_E_{i}", new Vector3(levelSize / 2 - 0.5f, wallHeight, 0), new Vector3(0.2f, wallHeight * 2, traversableWidth));

                // Traversable surface on west wall
                CreateTraversableSurface($"Traversable_W_{i}", new Vector3(-levelSize / 2 + 0.5f, wallHeight, 0), new Vector3(0.2f, wallHeight * 2, traversableWidth));
            }
        }

        private void CreateTraversableSurface(string name, Vector3 position, Vector3 scale)
        {
            GameObject surface = CreatePrimitive(name, position, Quaternion.identity, scale);
            surface.GetComponent<Renderer>().material.color = traversableColor;

            // Make it slightly transparent
            Renderer renderer = surface.GetComponent<Renderer>();
            if (renderer.material.HasProperty("_Color"))
            {
                Color color = traversableColor;
                color.a = 0.5f;
                renderer.material.color = color;
            }

            surface.layer = LayerMask.NameToLayer("Traversable");
            surface.tag = "Traversable";
        }

        private void CreateSpawnPoints()
        {
            // Player spawn point
            GameObject playerSpawn = new GameObject("PlayerSpawn");
            playerSpawn.transform.position = new Vector3(0, 1f, 0);
            playerSpawn.transform.parent = levelRoot;

            // Enemy spawn points
            for (int i = 0; i < 6; i++)
            {
                float angle = (360f / 6) * i;
                float radius = 15f;
                float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                float z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;

                GameObject enemySpawn = new GameObject($"EnemySpawn_{i}");
                enemySpawn.transform.position = new Vector3(x, 1f, z);
                enemySpawn.transform.parent = levelRoot;

                // Spawn an enemy at this location
                SpawnEnemy(enemySpawn.transform.position);
            }

            // Target human spawn
            GameObject targetSpawn = new GameObject("TargetSpawn");
            targetSpawn.transform.position = new Vector3(0, 1f, 15f);
            targetSpawn.transform.parent = levelRoot;
        }

        private void SpawnEnemy(Vector3 position)
        {
            GameObject enemyObj = new GameObject($"Enemy_{Random.Range(1000, 9999)}");
            enemyObj.transform.position = position;
            enemyObj.transform.parent = levelRoot;

            // Add enemy components
            enemyObj.AddComponent<Core.EnemyAI>();
            enemyObj.tag = "Enemy";
            enemyObj.layer = LayerMask.NameToLayer("Enemy");

            // Add visual representation
            GameObject visual = CreatePrimitive($"{enemyObj.name}_Visual", Vector3.zero, Quaternion.identity, new Vector3(1f, 2f, 1f));
            visual.transform.parent = enemyObj.transform;
            visual.GetComponent<Renderer>().material.color = Color.red;
        }

        private GameObject CreatePrimitive(string name, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.localScale = scale;

            if (levelRoot != null)
            {
                obj.transform.parent = levelRoot;
            }

            return obj;
        }
    }
}
