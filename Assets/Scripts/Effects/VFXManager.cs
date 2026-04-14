using UnityEngine;
using TimesBaddestCat.Foundation;

namespace TimesBaddestCat.Effects
{
    /// <summary>
    /// VFXManager - Manages all visual effects in the game.
    /// Handles particle systems, impact effects, and visual feedback.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        #region Singleton

        private static VFXManager _instance;
        public static VFXManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VFXManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("VFXManager");
                        _instance = go.AddComponent<VFXManager>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Serialized Fields

        [Header("Particle Prefabs")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private GameObject bulletTrailPrefab;
        [SerializeField] private GameObject impactDustPrefab;
        [SerializeField] private GameObject bloodHitPrefab;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject enemyDeathPrefab;
        [SerializeField] private GameObject dashTrailPrefab;
        [SerializeField] private GameObject footstepDustPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 50;
        [SerializeField] private float particleLifetime = 2f;

        #endregion

        #region Pool System

        private GameObject[] particlePool;
        private int poolIndex = 0;
        private System.Collections.Generic.Dictionary<string, GameObject> activeEffects =
            new System.Collections.Generic.Dictionary<string, GameObject>();

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

            InitializeParticlePool();
        }

        #endregion

        #region Initialization

        private void InitializeParticlePool()
        {
            particlePool = new GameObject[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                GameObject particle = new GameObject($"Particle_{i}");
                particle.transform.SetParent(transform);
                particle.SetActive(false);
                particlePool[i] = particle;
            }
        }

        private GameObject GetParticleFromPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                poolIndex = (poolIndex + 1) % poolSize;
                if (!particlePool[poolIndex].activeInHierarchy)
                {
                    return particlePool[poolIndex];
                }
            }

            // If pool is full, recycle oldest
            GameObject particle = particlePool[0];
            particle.SetActive(false);
            return particle;
        }

        private void ReturnParticleToPool(GameObject particle)
        {
            if (particle != null)
            {
                particle.SetActive(false);
                particle.transform.SetParent(transform);
            }
        }

        #endregion

        #region Combat VFX

        public void PlayMuzzleFlash(Vector3 position, Quaternion rotation, WeaponType weaponType)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;
            particle.transform.rotation = rotation;

            // Create muzzle flash visual
            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.1f;
                main.loop = false;
                main.startLifetime = 0.1f;
                main.startSize = GetMuzzleFlashSize(weaponType);
                main.startColor = new Color(1f, 0.8f, 0.3f, 1f); // Orange muzzle flash
                main.startColor = new Color(1f, 0.8f, 0.3f, 0f); // Fade to orange

                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(20, 0f)
                }, 1);
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.2f));
        }

        public void PlayBulletTrail(Vector3 start, Vector3 end)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = start;
            particle.transform.LookAt(end);

            // Create trail
            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.3f;
                main.loop = false;
                main.startLifetime = 0.3f;
                main.startSpeed = 0f;
                main.startSize = 0.05f;
                main.startColor = new Color(1f, 1f, 0.5f, 1f); // Yellow bullet trail
                main.startColor = new Color(1f, 1f, 0.5f, 0f);
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.3f));
        }

        public void PlayImpact(Vector3 position, Vector3 normal, BodyPart bodyPart)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;
            particle.transform.rotation = Quaternion.LookRotation(normal);

            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.5f;
                main.loop = false;
                main.startLifetime = 0.5f;
                main.startSize = 0.3f;
                main.gravityModifier = -0.5f;

                // Color based on hit type
                Color hitColor = GetHitColor(bodyPart);
                main.startColor = new Color(hitColor.r, hitColor.g, hitColor.b, 1f);
                main.startColor = new Color(hitColor.r, hitColor.g, hitColor.b, 0f);

                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(30, 0f)
                }, 1);
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.5f));
        }

        public void PlayBloodHit(Vector3 position, Vector3 normal)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;
            particle.transform.rotation = Quaternion.LookRotation(normal);

            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.8f;
                main.loop = false;
                main.startLifetime = 0.8f;
                main.startSize = 0.2f;
                main.gravityModifier = -2f;

                main.startColor = new Color(0.8f, 0.1f, 0.1f, 1f); // Red blood
                main.startColor = new Color(0.8f, 0.1f, 0.1f, 0f);

                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(50, 0f)
                }, 1);

                var shape = ps.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Sphere;
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.8f));
        }

        public void PlayEnemyDeath(Vector3 position)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;

            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 1.5f;
                main.loop = false;
                main.startLifetime = 1.5f;
                main.startSize = 0.5f;
                main.gravityModifier = -1f;

                main.startColor = new Color(0.8f, 0.1f, 0.1f, 1f);
                main.startColor = new Color(0.8f, 0.1f, 0.1f, 0f);

                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(100, 0f)
                }, 1);

                var shape = ps.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Sphere;
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 1.5f));
        }

        #endregion

        #region Movement VFX

        public void PlayDashTrail(Vector3 position, Vector3 direction)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;
            particle.transform.rotation = Quaternion.LookRotation(direction);

            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.3f;
                main.loop = false;
                main.startLifetime = 0.3f;
                main.startSize = 0.4f;
                main.startSpeed = 5f;

                main.startColor = new Color(0.2f, 0.8f, 1f, 1f); // Cyan dash trail
                main.startColor = new Color(0.2f, 0.8f, 1f, 0f);

                var emission = ps.emission;
                emission.rateOverTime = 50;
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.3f));
        }

        public void PlayFootstepDust(Vector3 position)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;

            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.5f;
                main.loop = false;
                main.startLifetime = 0.5f;
                main.startSize = 0.2f;
                main.gravityModifier = -0.5f;

                main.startColor = new Color(0.9f, 0.85f, 0.7f, 1f); // Floor color
                main.startColor = new Color(0.9f, 0.85f, 0.7f, 0f);

                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(15, 0f)
                }, 1);

                var shape = ps.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.5f));
        }

        #endregion

        #region Environmental VFX

        public void PlayWallRunTrail(Vector3 position, Vector3 wallNormal)
        {
            GameObject particle = GetParticleFromPool();
            particle.transform.position = position;
            particle.transform.rotation = Quaternion.LookRotation(wallNormal);

            ParticleSystem ps = EnsureParticleSystem(particle);
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.5f;
                main.loop = true;
                main.startLifetime = 0.5f;
                main.startSize = 0.3f;
                main.startSpeed = 2f;

                main.startColor = new Color(0f, 1f, 1f, 1f); // Cyan wall run trail
                main.startColor = new Color(0f, 1f, 1f, 0f);

                var emission = ps.emission;
                emission.rateOverTime = 30;
            }

            StartCoroutine(ReturnParticleAfterDelay(particle, 0.5f));
        }

        #endregion

        #region Helpers

        private ParticleSystem EnsureParticleSystem(GameObject obj)
        {
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                ps = obj.AddComponent<ParticleSystem>();
            }
            return ps;
        }

        private System.Collections.IEnumerator ReturnParticleAfterDelay(GameObject particle, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnParticleToPool(particle);
        }

        private float GetMuzzleFlashSize(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Shotgun: return 3f;
                case WeaponType.SniperRifle: return 4f;
                case WeaponType.LMG: return 2.5f;
                default: return 2f; // AR, SMG
            }
        }

        private Color GetHitColor(BodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case BodyPart.Head: return new Color(1f, 1f, 0f, 1f); // Yellow for headshot
                case BodyPart.Body: return new Color(1f, 0.5f, 0f, 1f); // Orange for body
                case BodyPart.Limbs: return new Color(1f, 0.3f, 0f, 1f); // Red-orange for limbs
                default: return Color.white;
            }
        }

        #endregion

        #region Public API

        public int ActiveEffectCount => activeEffects.Count;

        #endregion
    }
}
