using Core.Scripts.AnimationCollector.Assembly.CollectItem;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.AnimationCollector.Assembly.UIUpdater;

namespace Core.Scripts.AnimationCollector.Assembly.Collector
{
    public interface IAnimationCollector
    {
        CollectorBase CollectorBase { get; set; }
        float IntervalForSequence { get; }
        CollectType CollectType { get; }
        ICollectItem GetCollectItem();
        ICollectUIUpdater GetCollectTargetUpdater(CollectorInfo collectorInfo);
        bool Do(CollectorInfo collectorInfo, ICollectUIUpdater uiUpdater);
    }
}