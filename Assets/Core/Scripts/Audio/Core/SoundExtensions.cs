using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Audio.Core
{
    public static class SoundExtensions
    {
        public static void PlayWithFadeIn(this AudioSource source, AudioClip clip, float volume = 1, float duration = 3)
        {
            source.clip = clip;
            source.PlayWithFadeIn(volume, duration);
        }

        public static void PlayWithFadeIn(this AudioSource source, float volume = 1, float duration = 3)
        {
            source.Play();
            DOTween.To(x => { source.volume = x; }, 0, volume, duration);
        }

        public static void StopWithFadeOut(this AudioSource source, float volume = 0, float duration = 3)
        {
            DOTween.To(x => { source.volume = x; }, source.volume, volume, duration).OnComplete(source.Stop);
        }

        public static void ChangeVolume(this AudioSource source, float volume = 0, float duration = 1)
        {
            DOTween.To(x => { source.volume = x; }, source.volume, volume, duration);
        }
    }
}