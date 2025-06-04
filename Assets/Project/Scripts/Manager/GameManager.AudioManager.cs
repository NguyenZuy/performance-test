using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using TriInspector;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        [Title("Audio Manager")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        private MotionHandle _musicFadeHandle;

        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;

        private void InitializeAudio()
        {
            // Create audio sources if they don't exist
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.loop = false;
                _sfxSource.playOnAwake = false;
            }

            // Load saved volume settings
            LoadAudioSettings();
        }

        private void LoadAudioSettings()
        {
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            UpdateAudioVolumes();
        }

        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            PlayerPrefs.Save();
        }

        private void UpdateAudioVolumes()
        {
            if (_musicSource != null)
                _musicSource.volume = _musicVolume;

            if (_sfxSource != null)
                _sfxSource.volume = _sfxVolume;
        }

        #region Public Audio Methods

        public void PlayMusic(AudioClip musicClip, bool fadeIn = true, float fadeDuration = 1f)
        {
            if (_musicSource == null || musicClip == null) return;

            // Cancel any ongoing fade operations
            _musicFadeHandle.Cancel();

            if (fadeIn)
            {
                if (_musicSource.isPlaying && _musicSource.clip == musicClip)
                    return; // Already playing the same clip

                _musicSource.clip = musicClip;
                _musicSource.Play();

                // Use LitMotion for smooth fade in
                _musicFadeHandle = LMotion.Create(0f, _musicVolume, fadeDuration)
                    .WithEase(Ease.OutQuad)
                    .BindToVolume(_musicSource);
            }
            else
            {
                _musicSource.clip = musicClip;
                _musicSource.volume = _musicVolume;
                _musicSource.Play();
            }
        }

        public void StopMusic(bool fadeOut = true, float fadeDuration = 1f)
        {
            if (_musicSource == null || !_musicSource.isPlaying) return;

            // Cancel any ongoing fade operations
            _musicFadeHandle.Cancel();

            if (fadeOut)
            {
                _musicFadeHandle = LMotion.Create(_musicSource.volume, 0f, fadeDuration)
                    .WithEase(Ease.OutQuad)
                    .WithOnComplete(() => _musicSource.Stop())
                    .BindToVolume(_musicSource);
            }
            else
            {
                _musicSource.Stop();
            }
        }

        public void PlaySFX(AudioClip sfxClip, float volumeMultiplier = 1f)
        {
            if (_sfxSource == null || sfxClip == null) return;

            _sfxSource.PlayOneShot(sfxClip, volumeMultiplier * _sfxVolume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateAudioVolumes();
            SaveAudioSettings();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            UpdateAudioVolumes();
            SaveAudioSettings();
        }

        public void CrossFadeMusic(AudioClip newClip, float fadeDuration = 1f)
        {
            if (_musicSource == null || newClip == null) return;

            // Cancel any ongoing fade operations
            _musicFadeHandle.Cancel();

            if (!_musicSource.isPlaying || _musicSource.clip == null)
            {
                PlayMusic(newClip, true, fadeDuration);
                return;
            }

            // Create a temporary AudioSource for the crossfade
            var tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.clip = _musicSource.clip;
            tempSource.volume = _musicSource.volume;
            tempSource.loop = _musicSource.loop;
            tempSource.Play();

            // Set up the new music
            _musicSource.clip = newClip;
            _musicSource.volume = 0f;
            _musicSource.Play();

            // Fade out the old music
            LMotion.Create(tempSource.volume, 0f, fadeDuration)
                .WithEase(Ease.OutQuad)
                .WithOnComplete(() => Destroy(tempSource))
                .BindToVolume(tempSource);

            // Fade in the new music
            _musicFadeHandle = LMotion.Create(0f, _musicVolume, fadeDuration)
                .WithEase(Ease.OutQuad)
                .BindToVolume(_musicSource);
        }

        #endregion
    }
}