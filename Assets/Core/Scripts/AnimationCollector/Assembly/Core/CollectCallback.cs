using System;

namespace Core.Scripts.AnimationCollector.Assembly.Core
{
    public class CollectCallback
    {
        public Action onCollectBegin;
        public Action onCompleteFirst;
        public Action<int, bool> onUpdate; // param 1: collect count, param 2: isEnd
        public Action onCompleteAll;
    }
}