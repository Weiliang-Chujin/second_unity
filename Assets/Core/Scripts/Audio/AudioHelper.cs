using Core.Scripts.Audio.Core;

namespace Core.Scripts.Audio
{
    public class AudioHelper
    {
        // 默认背景音乐
        public static void PlayBaseHomeBGM()
        {
            PlayBGMAfterStopOthers(AudioConstant.home_background, 1f, true);
        }

        // 播放音频（使用PlayOneShot 不支持停止播放）
        public static void PlaySound(string name, float volume = 1)
        {
            if (AudioController.Instance.IsSfxOn)
            {
                AudioController.Instance.PlaySound(name, volume);
            }
        }

        /// <summary>
        /// 通过名称获取当前音效是否正在Common播放器播放
        /// </summary>
        public static bool GetIsPlayingByName(string name)
        {
            return AudioController.Instance.GetIsPlayingByName(name, AudioSourceType.Common);
        }

        // 间隔播放音频（间隔时间内只会播放一次）
        public static void IntervalSound(string name, float volume = 1f, float interval = 0.1f)
        {
            if (AudioController.Instance.IsSfxOn)
            {
                AudioController.Instance.IntervalSound(name, volume, interval);
            }
        }

        // 播放特殊音频（支持停止播放）
        public static void PlaySpecialSound(string name, float volume = 1)
        {
            if (AudioController.Instance.IsSfxOn)
            {
                AudioController.Instance.PlaySpecialSound(name, volume);
            }
        }

        // 停止播放特殊音频
        public static void StopSpecialMusic()
        {
            if (AudioController.Instance.IsSfxOn)
            {
                AudioController.Instance.StopAllSourceEx(AudioSourceType.Special, false);
            }
        }

        // 播放背景音乐
        public static void PlayBackgroundMusic(string name, float volume = 1f, bool isFadeIn = true, bool rePlay = true,
            float duration = 3)
        {
            if (AudioController.Instance.IsMusicOn)
            {
                AudioController.Instance.PlayBackgroundMusic(name, volume, isFadeIn, rePlay, duration);
            }
        }

        // 在关闭其他背景音乐后，播放当前背景音乐
        public static void PlayBGMAfterStopOthers(string name, float volume = 1f, bool isFadeIn = true,
            float duration = 3)
        {
            if (AudioController.Instance.IsMusicOn)
            {
                AudioController.Instance.PlayBGMAfterStopOthers(name, volume, isFadeIn, duration);
            }
        }

        // 关闭指定背景音乐
        public static void StopBackgroundMusic(string name, bool isFadeIn = true, float duration = 3)
        {
            if (AudioController.Instance.IsMusicOn)
            {
                AudioController.Instance.StopSourceEx(AudioSourceType.BackGround, name, isFadeIn, duration);
            }
        }

        // 关闭全部背景音乐
        public static void StopAllBackgroundMusic(bool isFadeIn = true)
        {
            if (AudioController.Instance.IsMusicOn)
            {
                AudioController.Instance.StopAllSourceEx(AudioSourceType.BackGround, isFadeIn);
            }
        }

        /// <summary>
        /// 改变指定背景音乐音量
        /// </summary>
        /// <param name="endVolume"></param>
        /// <param name="duration"></param>
        public static void ChangeBaseHomeBgmVolume(string name, float endVolume = 1f, float duration = 1f)
        {
            if (AudioController.Instance.IsMusicOn)
            {
                AudioController.Instance.ChangAudioVolume(name, AudioSourceType.BackGround,
                    endVolume, duration);
            }
        }
    }
}