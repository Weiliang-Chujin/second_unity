using Core.Scripts.AnimationCollector.Assembly.CollectItem;
using Core.Scripts.AnimationCollector.Assembly.Collector;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.AnimationCollector.Assembly.UIUpdater;
using UnityEngine;

namespace AnimationCollector
{
    public abstract class BaseAnimationCollector : AnimationCollectorBase
    {
        protected const int minCount = 1;
        protected const int maxCount = 15;

        public override ICollectItem GetCollectItem()
        {
            CoinCollectItem item = Resources.Load<CoinCollectItem>("CoinCollection"); // TODO for example
            return Object.Instantiate(item);
        }

        public override bool Do(CollectorInfo collectorInfo, ICollectUIUpdater uiUpdater)
        {
            return ShowCollectItems(CalculateCount(collectorInfo.totalCount), collectorInfo, uiUpdater);
        }

        protected virtual int CalculateCount(long totalCount)
        {
            float tempTotalCount = Mathf.Sqrt(totalCount);

            int count = Mathf.RoundToInt(tempTotalCount);
            count = Mathf.Clamp(count, minCount, maxCount);
            return count;
        }
    }

    public abstract class BaseAnimationCollector<T> : BaseAnimationCollector where T : CollectorInfo
    {
        public sealed override bool Do(CollectorInfo collectorInfo, ICollectUIUpdater uiUpdater)
        {
            return Do(collectorInfo as T, uiUpdater);
        }

        protected virtual bool Do(T collectorInfo, ICollectUIUpdater uiUpdater)
        {
            return base.Do(collectorInfo, uiUpdater);
        }
    }
}