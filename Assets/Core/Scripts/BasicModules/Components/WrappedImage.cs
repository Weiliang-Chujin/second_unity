using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.BasicModules.Components
{
    public class WrappedImage : Image
    {
        [SerializeField] public Sprite[] sprites;

        public int spriteIndex
        {
            set => sprite = sprites.Length > value ? sprites[value] : null;
        }
    }
}