using UnityEngine;

namespace Core.Scripts.Audio.Core
{
    public class AudioSourceEx : MonoBehaviour
    {
        public AudioSource AudioSource;

        public string PlayingAudioName;

        public void ClearPlayingAudioName()
        {
            PlayingAudioName = "";
        }

        public bool NoPlayingAudioName()
        {
            return PlayingAudioName.Equals("");
        }

        public bool IsSameName(string name)
        {
            return PlayingAudioName.Equals(name);
        }
    }
}