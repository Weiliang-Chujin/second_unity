using Core.Scripts.AnimationCollector.Assembly.CollectItem;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.AnimationCollector.Assembly.UIUpdater;
using UnityEngine;

namespace Core.Scripts.AnimationCollector.Assembly.Collector
{
    public abstract class AnimationCollectorBase : IAnimationCollector
    {
        public CollectorBase CollectorBase { get; set; }
        public virtual float IntervalForSequence => 12 / 30f;
        public abstract CollectType CollectType { get; }

        public abstract ICollectItem GetCollectItem();

        public abstract bool Do(CollectorInfo collectorInfo, ICollectUIUpdater uiUpdater);

        public ICollectUIUpdater GetCollectTargetUpdater(CollectorInfo collectorInfo)
        {
            return CollectorBase.FindCollectUIUpdater(CollectType, collectorInfo);
        }

        // 具体显示每一个奖励，
        protected virtual bool ShowCollectItems(long count, CollectorInfo collectorInfo, ICollectUIUpdater updater)
        {
            collectorInfo.collectCallback.onCollectBegin?.Invoke();

            if (!updater.IsNull())
            {
                Vector3 targetPos = updater.Position;
                if (collectorInfo.toPos != null)
                {
                    targetPos = collectorInfo.toPos.Value;
                }

                for (var i = 0; i < count; i++)
                {
                    ICollectItem collectItem = GetCollectItem();
                    int index = i;
                    bool isEnd = i >= count - 1;

                    collectItem?.SetCollectorInfo(collectorInfo);
                    collectItem?.Do(collectorInfo.from, targetPos, i, () =>
                    {
                        collectorInfo.collectCallback?.onUpdate?.Invoke(index + 1, isEnd);

                        if (index == 0)
                        {
                            if (!updater.IsNull())
                            {
                                updater.UpdateUI();
                            }

                            collectorInfo.collectCallback?.onCompleteFirst?.Invoke();
                        }

                        if (!updater.IsNull())
                        {
                            updater.ResponseCollection();
                        }

                        if (isEnd)
                        {
                            collectorInfo.collectCallback?.onCompleteAll?.Invoke();
                        }
                    });
                }

                return true;
            }

            Debug.Log($"get collect target updater error. collector type :{collectorInfo.collectType}");
            return false;
        }
    }
}