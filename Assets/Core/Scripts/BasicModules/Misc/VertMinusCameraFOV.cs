using UnityEngine;

namespace Core.Scripts.BasicModules.Misc
{
    [RequireComponent(typeof(Camera))]
    public class VertMinusCameraFOV : MonoBehaviour
    {
        private static int designTimeWidth => UnityUtil.IsPad() ? 1440 : 1080;
        private static int designTimeHeight => 1920;

        [SerializeField] private float designTimeVerticalFieldOfView;

        private void Start()
        {
            Camera mainCamera = GetComponent<Camera>();
            mainCamera.fieldOfView = AdjustFieldOfView(designTimeVerticalFieldOfView);
        }

        public static float AdjustFieldOfView(float designTimeVerticalFieldOfView)
        {
            float aspectRatio = designTimeWidth / (float) designTimeHeight;
            float vFOVInRads = designTimeVerticalFieldOfView * Mathf.Deg2Rad;
            float hFOVInRads = 2f * Mathf.Atan(Mathf.Tan(vFOVInRads / 2f) * aspectRatio);

            return adjustFieldOfView(hFOVInRads);
        }

        private static float adjustFieldOfView(float hFOVInRads)
        {
            float aspectRatio = Screen.width / (float) Screen.height;
            float vFOVInRads = 2f * Mathf.Atan(Mathf.Tan(hFOVInRads / 2f) / aspectRatio);
            return vFOVInRads * Mathf.Rad2Deg;
        }

        public static float GetMatchWidthOrHeight()
        {
            float aspectRatio = GetAspectRatio();
            // 标准尺寸 1080x1920，宽高比0.5625f，大于此比例，UI高适配
            return aspectRatio > 0.5625f ? 1 : 0;
        }

        public static bool IsPad()
        {
            return GetMatchWidthOrHeight() > 0;
        }

        //返回宽高比
        public static float GetAspectRatio()
        {
            float width = Screen.width;
            float height = Screen.height;
            return width / height;
        }
    }
}