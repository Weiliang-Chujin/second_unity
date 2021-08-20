using System;
using Core.Scripts.AnimationCollector.Assembly.Core;
using UnityEngine;

namespace Core.Scripts.AnimationCollector.Assembly.CollectItem
{
    public interface ICollectItem
    {
        void SetParent(Transform parent);

        void SetCollectorInfo(CollectorInfo collectorInfo);

        void Do(Vector3 from, Vector3 to, int index, Action onComplete);

        void Recycle();
    }
}