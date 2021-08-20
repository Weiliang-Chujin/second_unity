using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.AnimationCollector.Assembly.UIUpdater;

namespace AnimationCollector
{
    public class CoinAnimationCollector : BaseAnimationCollector
    {
        public override CollectType CollectType => (CollectType) 1;

        protected override bool ShowCollectItems(long count, CollectorInfo collectorInfo, ICollectUIUpdater uiUpdater)
        {
            collectorInfo.collectCallback.onCompleteFirst += () =>
            {
                // TODO sound
            };

            return base.ShowCollectItems(count, collectorInfo, uiUpdater);
        }
    }
}