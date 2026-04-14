using UnityEngine;

namespace TimesBaddestCat.Foundation
{
    /// <summary>
    /// AudioManager - Handles all audio playback in the game.
    /// Supports sound effects, background music, and dynamic audio mixing.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Singleton

        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        _instance = go.AddComponent<AudioManager>();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Serialized Fields

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float ambientVolume = 0.5f;

        [Header("Pitch Variations")]
        [SerializeField] private float pitchVariation = 0.1f;
        [SerializeField] private float volumeVariation = 0.1f;

        #endregion

        #region Audio Clips

        [Header("Music Clips")]
        [SerializeField] private AudioClip[] musicClips;

        [Header("SFX - Player")]
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip dashClip;
        [SerializeField] private AudioClip landClip;

        [Header("SFX - Combat")]
        [SerializeField] private AudioClip[] fireClips;
        [SerializeField] private AudioClip reloadClip;
        [SerializeField] private AudioClip[] hitClips;
        [SerializeField] private AudioClip[] killClips;
        [SerializeField] private AudioClip emptyClip;

        [Header("SFX - Enemy")]
        [SerializeField] private AudioClip[] enemyAlertClips;
        [SerializeField] private AudioClip[] enemyDeathClips;

        [Header("Ambient Clips")]
        [SerializeField] private AudioClip[] ambientClips;

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

            InitializeAudioSources();
            PlayBackgroundMusic();
        }

        #endregion

        #region Initialization

        private void InitializeAudioSources()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        #endregion

        #region Music

        public void PlayBackgroundMusic()
        {
            if (musicClips != null && musicClips.Length > 0)
            {
                AudioClip clip = musicClips[Random.Range(0, musicClips.Length)];
                if (!musicSource.clip.Equals(clip))
                {
                    musicSource.clip = clip;
                    musicSource.Play();
                }
            }
        }

        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        #endregion

        #region Sound Effects

        public void PlayFootstep()
        {
            if (footstepClips != null && footstepClips.Length > 0)
            {
                PlayRandomSFX(footstepClips);
            }
        }

        public void PlayJump()
        {
            if (jumpClip != null)
            {
                PlaySFX(jumpClip);
            }
        }

        public void PlayDash()
        {
            if (dashClip != null)
            {
                PlaySFX(dashClip);
            }
        }

        public void PlayLand()
        {
            if (landClip != null)
            {
                PlaySFX(landClip);
            }
        }

        public void PlayFire()
        {
            if (fireClips != null && fireClips.Length > 0)
            {
                PlayRandomSFX(fireClips);
            }
        }

        public void PlayReload()
        {
            if (reloadClip != null)
            {
                PlaySFX(reloadClip);
            }
        }

        public void PlayHit()
        {
            if (hitClips != null && hitClips.Length > 0)
            {
                PlayRandomSFX(hitClips);
            }
        }

        public void PlayKill()
        {
            if (killClips != null && killClips.Length > 0)
            {
                PlayRandomSFX(killClips);
            }
        }

        public void PlayEmpty()
        {
            if (emptyClip != null)
            {
                PlaySFX(emptyClip);
            }
        }

        public void PlayEnemyAlert()
        {
            if (enemyAlertClips != null && enemyAlertClips.Length > 0)
            {
                PlayRandomSFX(enemyAlertClips);
            }
        }

        public void PlayEnemyDeath()
        {
            if (enemyDeathClips != null && enemyDeathClips.Length > 0)
            {
                PlayRandomSFX(enemyDeathClips);
            }
        }

        #endregion

        #region Ambient

        public void PlayAmbient()
        {
            if (ambientClips != null && ambientClips.Length > 0)
            {
                AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
                if (!ambientSource.clip.Equals(clip))
                {
                    ambientSource.clip = clip;
                    ambientSource.Play();
                }
            }
        }

        #endregion

        #region Helpers

        private void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;

            sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            sfxSource.volume = sfxVolume * masterVolume * (1f + Random.Range(-volumeVariation, volumeVariation));
            sfxSource.PlayOneShot(clip);
        }

        private void PlayRandomSFX(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return;

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            PlaySFX(clip);
        }

        private void UpdateVolumes()
        {
            musicSource.volume = musicVolume * masterVolume;
            sfxSource.volume = sfxVolume * masterVolume;
            ambientSource.volume = ambientVolume * masterVolume;
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        #endregion

        #region Public API

        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;

        #endregion
    }
}
