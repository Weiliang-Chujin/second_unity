using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Scripts.Audio.Core
{
    /// <summary>
    /// * Controller Music & Sfx play state
    /// * Cache all AudioClip & AudioSource
    /// * Play Music & Sfx sound
    /// * Resource mapping uses the "Addressable"
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance;

        [SerializeField] private AudioSourceEx backgroundEffect;
        [SerializeField] private AudioSourceEx soundEffect;
        [SerializeField] private AudioSourceEx specialEffect;

        /// <summary>
        /// cache clips
        /// key: clip name
        /// value: audio clip
        /// </summary>
        private Dictionary<string, AudioClip> cacheAudioClips;

        /// <summary>
        /// cache all audioSource 
        /// </summary>
        private Dictionary<AudioSourceType, List<AudioSourceEx>> audioSourcesDict;

        /// <summary>
        /// cache all interval audio last play time
        /// key: sound name
        /// value: last play timeStamp
        /// </summary>
        private Dictionary<string, float> intervalAudioDict;

        public bool IsSfxOn
        {
            get => PlayerPrefs.GetInt("sound_prefs_key", 1) == 1;
            set
            {
                int state = value ? 1 : 0;
                soundEffect.AudioSource.volume = state;
                specialEffect.AudioSource.volume = state;
                PlayerPrefs.SetInt("sound_prefs_key", state);
            }
        }

        public bool IsMusicOn
        {
            get => PlayerPrefs.GetInt("music_prefs_key", 1) == 1;
            set
            {
                int state = value ? 1 : 0;
                if (audioSourcesDict.TryGetValue(AudioSourceType.BackGround, out List<AudioSourceEx> sourceEx))
                {
                    foreach (AudioSourceEx ex in sourceEx)
                    {
                        ex.AudioSource.mute = !value;
                    }
                }

                PlayerPrefs.SetInt("music_prefs_key", state);
            }
        }

        private void Awake()
        {
            Instance = this;

            cacheAudioClips = new Dictionary<string, AudioClip>();
            intervalAudioDict = new Dictionary<string, float>();

            CheckAsset();

            CacheAudioSources();
        }

        private void CheckAsset()
        {
            Assert.IsNotNull(soundEffect);
            Assert.IsNotNull(backgroundEffect);
            Assert.IsNotNull(specialEffect);
        }

        private void CacheAudioSources()
        {
            audioSourcesDict = new Dictionary<AudioSourceType, List<AudioSourceEx>>
            {
                {AudioSourceType.BackGround, new List<AudioSourceEx>() {backgroundEffect}},
                {AudioSourceType.Common, new List<AudioSourceEx>() {soundEffect}},
                {AudioSourceType.Special, new List<AudioSourceEx>() {specialEffect}},
            };
        }

        /// <summary>
        /// simple handle interval sound, only check previous sound
        /// </summary>
        public void IntervalSound(string name, float volume = 1f, float interval = 0.1f)
        {
            if (!intervalAudioDict.TryGetValue(name, out float time) || Time.realtimeSinceStartup - time >= interval)
            {
                intervalAudioDict[name] = Time.realtimeSinceStartup;
                PlaySound(name, volume);
            }
        }

        public void PlaySound(string name, float volume = 1)
        {
            PlaySoundAfterGetClip(name, volume, AudioSourceType.Common);
        }

        public void PlaySpecialSound(string name, float volume)
        {
            PlaySoundAfterGetClip(name, volume, AudioSourceType.Special);
        }

        private void PlayOneShotSound(AudioSourceEx sourceEx, AudioClip clip, string name, float volume = 1)
        {
            if (clip)
            {
                sourceEx.PlayingAudioName = name;
                sourceEx.AudioSource.PlayOneShot(clip, volume);
            }
        }

        private void PlayNormalSound(AudioSourceEx sourceEx, AudioClip clip, string name, float volume = 1)
        {
            if (clip)
            {
                sourceEx.PlayingAudioName = name;

                sourceEx.AudioSource.volume = volume;
                sourceEx.AudioSource.clip = clip;
                sourceEx.AudioSource.Play();
            }
        }

        public void PlayBackgroundMusic(string name, float soundVale = 1, bool isFadeIn = true, bool rePlay = false,
            float duration = 3)
        {
            if (GetPlayingAudioSourceEx(name, AudioSourceType.BackGround, out var sourceEx))
            {
                if (rePlay)
                {
                    PlayBackgroundMusic(sourceEx, sourceEx.AudioSource.clip, name, soundVale, isFadeIn,
                        duration);
                }
                else
                {
                    sourceEx.AudioSource.volume = soundVale;
                }
            }
            else
            {
                PlayBGMAfterGetClip(name, soundVale, isFadeIn, duration);
            }
        }

        public void PlayBGMAfterStopOthers(string name, float soundVale = 1, bool isFadeIn = true, float duration = 3)
        {
            if (IsMusicOn)
            {
                StopAllSourceEx(AudioSourceType.BackGround);

                PlayBGMAfterGetClip(name, soundVale, isFadeIn, duration);
            }
        }

        private void PlayBackgroundMusic(AudioSourceEx sourceEx, AudioClip clip, string name, float soundValue,
            bool isFadeIn = true, float duration = 3)
        {
            sourceEx.PlayingAudioName = name;

            if (isFadeIn)
            {
                sourceEx.AudioSource.PlayWithFadeIn(clip, soundValue, duration);
            }
            else
            {
                sourceEx.AudioSource.volume = soundValue;
                sourceEx.AudioSource.clip = clip;
                sourceEx.AudioSource.Play();
            }
        }

        public bool GetIsPlayingByName(string name, AudioSourceType sourceType)
        {
            if (GetPlayingAudioSourceEx(name, sourceType, out var targetSourceEx))
            {
                return true;
            }

            return false;
        }

        public void StopSourceEx(AudioSourceType sourceType, string name, bool isFadeIn = true, float duration = 3)
        {
            if (GetPlayingAudioSourceEx(name, sourceType, out var targetSourceEx))
            {
                StopAudioSourceEx(targetSourceEx, isFadeIn, duration);
            }
        }

        public void StopAllSourceEx(AudioSourceType sourceType, bool isFadeIn = true, float duration = 3)
        {
            foreach (var audioSourceEx in audioSourcesDict[sourceType])
            {
                StopAudioSourceEx(audioSourceEx, isFadeIn, duration);
            }
        }

        private void StopAudioSourceEx(AudioSourceEx audioSourceEx, bool isFadeIn, float duration)
        {
            if (!audioSourceEx.NoPlayingAudioName())
            {
                audioSourceEx.ClearPlayingAudioName();

                if (isFadeIn)
                {
                    audioSourceEx.AudioSource.StopWithFadeOut(duration: duration);
                }
                else
                {
                    audioSourceEx.AudioSource.Stop();
                }
            }
        }

        private void PlaySoundAfterGetClip(string name, float volume, AudioSourceType sourceType)
        {
            GetAudioClip(name, clip =>
            {
                AudioSourceEx sourceEx = GetPlayableAudioSourceEx(sourceType);
                if (sourceType == AudioSourceType.Common)
                {
                    PlayOneShotSound(sourceEx, clip, name, volume);
                }
                else if (sourceType == AudioSourceType.Special)
                {
                    PlayNormalSound(sourceEx, clip, name, volume);
                }
            });
        }

        private void PlayBGMAfterGetClip(string name, float soundValue, bool isFadeIn, float duration)
        {
            AudioSourceEx sourceEx = GetPlayableAudioSourceEx(AudioSourceType.BackGround);

            sourceEx.PlayingAudioName = name;

            GetAudioClip(name, clip =>
            {
                if (sourceEx.IsSameName(name))
                {
                    PlayBackgroundMusic(sourceEx, clip, name, soundValue, isFadeIn, duration);
                }
            });
        }

        private bool GetPlayingAudioSourceEx(string name, AudioSourceType sourceType, out AudioSourceEx target)
        {
            target = null;

            foreach (var audioSourceEx in audioSourcesDict[sourceType])
            {
                if (audioSourceEx.IsSameName(name))
                {
                    target = audioSourceEx;
                    return true;
                }
            }

            return false;
        }

        private AudioSourceEx GetPlayableAudioSourceEx(AudioSourceType audioSourceType)
        {
            switch (audioSourceType)
            {
                case AudioSourceType.BackGround:
                    return GetPlayableBgAudioSourceEx();
                default:
                    return audioSourcesDict[audioSourceType][0];
            }
        }

        private AudioSourceEx GetPlayableBgAudioSourceEx()
        {
            foreach (var sourceEx in audioSourcesDict[AudioSourceType.BackGround])
            {
                if (!sourceEx.AudioSource.isPlaying && sourceEx.NoPlayingAudioName())
                {
                    return sourceEx;
                }
            }

            var newSourceEx = Instantiate(backgroundEffect, transform);
            audioSourcesDict[AudioSourceType.BackGround].Add(newSourceEx);

            return newSourceEx;
        }

        private void GetAudioClip(string name, Action<AudioClip> loadSuccess, Action loadFail = null)
        {
            if (TryGetCacheAudioClip(name, out AudioClip cacheClip))
            {
                loadSuccess?.Invoke(cacheClip);
            }
            else
            {
                Addressables.LoadAssetAsync<AudioClip>(name).Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result)
                    {
                        // callback
                        loadSuccess?.Invoke(handle.Result);

                        // save cache
                        RecordCacheAudioClip(name, handle.Result);
                    }
                    else
                    {
                        loadFail?.Invoke();
                    }
                };
            }
        }

        private void RecordCacheAudioClip(string name, AudioClip clip)
        {
            if (!cacheAudioClips.TryGetValue(name, out AudioClip v))
            {
                cacheAudioClips.Add(name, clip);
            }
        }

        private bool TryGetCacheAudioClip(string name, out AudioClip clip)
        {
            return cacheAudioClips.TryGetValue(name, out clip);
        }

        public void ChangAudioVolume(string name, AudioSourceType audioSourceType,
            float endVolume = 1f, float duration = 1f)
        {
            foreach (var audioSourceEx in audioSourcesDict[audioSourceType])
            {
                if (audioSourceEx.PlayingAudioName.Equals(name))
                {
                    audioSourceEx.AudioSource.ChangeVolume(endVolume, duration);
                }
            }
        }
    }
}