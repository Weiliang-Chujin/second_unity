using System;
using UnityEngine;

namespace Core.Scripts.BasicModules.Misc
{
    public class MonoSingleGenerator<T> where T : MonoBehaviour
    {
        private static T instance;

        public static T Entity()
        {
            if (instance == null)
            {
                Type type = typeof(T);

                GameObject gameObject = new GameObject();

                instance = gameObject.AddComponent(type) as T;

                GameObject.DontDestroyOnLoad(gameObject);
            }

            return instance;
        }
    }
}