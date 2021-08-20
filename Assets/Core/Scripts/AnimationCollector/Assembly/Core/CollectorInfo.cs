using UnityEngine;

namespace Core.Scripts.AnimationCollector.Assembly.Core
{
    public class CollectorInfo
    {
        public CollectType collectType;
        public long totalCount;
        public Vector3 from;
        public Vector3? toPos;
        public CollectCallback collectCallback;

        public static CollectorInfo Create(CollectType collectType, long count)
        {
            CollectorInfo info = new CollectorInfo();
            info.collectType = collectType;
            info.totalCount = count;
            info.collectCallback = new CollectCallback();
            return info;
        }
    }
}