using UnityEngine;

namespace Core.Scripts.BasicModules.Components
{
    public class WrappedMonoBehaviour : MonoBehaviour
    {
        private Transform cachedTransform;

        public Transform Transform
        {
            get
            {
                if (!cachedTransform)
                {
                    cachedTransform = transform;
                }

                return cachedTransform;
            }
        }

        private GameObject cachedObject;

        public GameObject GameObject
        {
            get
            {
                if (!cachedObject)
                {
                    cachedObject = gameObject;
                }

                return cachedObject;
            }
        }
    }
}