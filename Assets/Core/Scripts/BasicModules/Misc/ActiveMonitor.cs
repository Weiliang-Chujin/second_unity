using UnityEngine;

namespace Core.Scripts.BasicModules.Misc
{
    public class ActiveMonitor : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log($"gameObject enable.");
        }

        private void OnDisable()
        {
            Debug.Log($"gameObject disable.");
        }
    }
}
