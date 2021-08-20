using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
#if UNITY_EDITOR

#endif

namespace Core.Scripts.BasicModules.Misc
{
    public abstract class CaptureScreenshotSimple : MonoBehaviour
    {
        public static bool isRunning;

#if UNITY_EDITOR

        protected void Do()
        {
            StartCoroutine(StartCapture());
        }

        private IEnumerator StartCapture()
        {
            isRunning = true;
            string path = Application.persistentDataPath + "/ScreenshotsTemp/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            yield return Execute(name =>
            {
                string fileName = path + GetSaveFileName(name);
                ScreenCapture.CaptureScreenshot(fileName);
            });

            isRunning = false;
            AssetDatabase.Refresh();
            Application.Quit();
        }

        protected virtual string GetSaveFileName(string name)
        {
            return $"{name}.jpg";
        }

        protected abstract IEnumerator Execute(Action<string> captureAction);
#endif
    }
}