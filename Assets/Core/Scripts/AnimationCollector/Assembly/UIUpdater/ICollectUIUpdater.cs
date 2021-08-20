using Core.Scripts.AnimationCollector.Assembly.Core;
using UnityEngine;

namespace Core.Scripts.AnimationCollector.Assembly.UIUpdater
{
    public interface ICollectUIUpdater
    {
        Vector3 Position { get; }
        bool Activated { get; set; }
        bool CheckCollectorInfo(CollectorInfo collectorInfo);

        /// <summary>
        /// contains animation
        /// </summary>
        void UpdateUI();

        void ForceSyncUI();
        void ResponseCollection();
        void Reduce(long count);
    }
}