using UnityEngine;

namespace Core.Scripts.BasicModules.Misc
{
    public struct TransformRecorder
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;

        public static TransformRecorder Create(Transform transform)
        {
            TransformRecorder transformRecorder = new TransformRecorder();
            transformRecorder.position = transform.position;
            transformRecorder.rotation = transform.rotation;
            transformRecorder.localScale = transform.localScale;
            return transformRecorder;
        }

        public void Reset(Transform transform)
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = localScale;
        }
    }
}