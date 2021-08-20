using System.Collections.Generic;
using Core.Scripts.AnimationCollector.Assembly.Collector;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Example.CollectAnimation;

namespace AnimationCollector
{
    public class CollectorManager : CollectorBase
    {
        public static CollectorManager Instance { get; } = new CollectorManager();

        protected override void InitCollectorsConfig()
        {
            collectors = new Dictionary<CollectType, IAnimationCollector>
            {
                {(CollectType) CollectExampleType.Coin, new CoinAnimationCollector()},
            };

            foreach (var keyValuePair in collectors)
            {
                keyValuePair.Value.CollectorBase = this;
            }
        }

        public void Example()
        {
            Show(ConvertCollectType(CollectExampleType.Coin), 1000);
        }

        public void Example2()
        {
            CollectorInfo info = CollectorInfo.Create(ConvertCollectType(CollectExampleType.Coin), 1000);
            AddCollectActionBuffer(new List<CollectorInfo> {info});
        }

        public void Example3()
        {
            ReleaseCollectActionBuffer();
        }

        private CollectType ConvertCollectType(CollectExampleType type)
        {
            return (CollectType) type;
        }
    }
}