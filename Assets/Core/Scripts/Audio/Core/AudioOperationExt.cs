using UnityEngine;

namespace Core.Scripts.Audio.Core
{
    /// <summary>
    /// all audio operation for AudioController 
    /// </summary>
    public static class AudioOperationExt
    {
        public static void MuteSound(bool muted)
        {
            AudioListener.volume = muted ? 0 : 1;
        }
    }
}