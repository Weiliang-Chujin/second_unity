using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.AnimationCollector.Assembly.Collector;
using Core.Scripts.AnimationCollector.Assembly.UIUpdater;
using Core.Scripts.BasicModules.Misc;
using UnityEngine;

namespace Core.Scripts.AnimationCollector.Assembly.Core
{
    public abstract class CollectorBase
    {
        protected Dictionary<CollectType, List<ICollectUIUpdater>> collectUpdaters;
        protected Dictionary<CollectType, IAnimationCollector> collectors;
        protected Queue<CollectorInfo> collectActionBuffer;

        protected CollectorBase()
        {
            collectActionBuffer = new Queue<CollectorInfo>();
            collectUpdaters = new Dictionary<CollectType, List<ICollectUIUpdater>>();

            // ReSharper disable once VirtualMemberCallInConstructor
            InitCollectorsConfig();
        }

        protected abstract void InitCollectorsConfig();

        public void Show(CollectType type, int count)
        {
            Show(type, count, Vector3.zero, null);
        }

        private void Show(CollectType type, int count, Vector3 startPosition, Action complete)
        {
            CollectorInfo collectorInfo = new CollectorInfo
            {
                collectType = type,
                totalCount = count,
                @from = startPosition,
                collectCallback = new CollectCallback {onCompleteAll = complete}
            };

            Show(collectorInfo);
        }

        public void Show(CollectorInfo collectorInfo)
        {
            IAnimationCollector collector = FindCollector(collectorInfo.collectType);
            ICollectUIUpdater targetUpdater = null;
            if (collector != null)
            {
                targetUpdater = collector.GetCollectTargetUpdater(collectorInfo);
            }

            Show(collectorInfo, collector, targetUpdater);
        }

        private void Show(CollectorInfo collectorInfo, IAnimationCollector collector, ICollectUIUpdater uiUpdater)
        {
            if (!collector.IsNull())
            {
                // TODO ensure not null
                collectorInfo.collectCallback = collectorInfo.collectCallback ?? new CollectCallback();

                if (collectorInfo.totalCount > 0)
                {
                    bool isExecuted = collector?.Do(collectorInfo, uiUpdater) ?? false;

                    // call real time sync updates
                    CallRealTimeSyncUpdaters(collectorInfo.collectType, isExecuted);
                }
                else
                {
                    // reduce 
                    ReduceCollection(collectorInfo);
                }
            }
        }

        public void CallRealTimeSyncUpdaters(CollectType collectType, bool ignoreTopUpdater)
        {
            List<ICollectUIUpdater> updaters = FindRealTimeSyncUpdaters(collectType, ignoreTopUpdater);
            updaters?.ForEach(update => update.ForceSyncUI());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectorInfoList"> possible has null items </param>
        public void AddCollectActionBuffer(List<CollectorInfo> collectorInfoList)
        {
            collectorInfoList.Sort((a, b) =>
            {
                // check null
                if (a == null || b == null)
                {
                    return 0;
                }

                return a.collectType.CompareTo(b.collectType);
            });

            collectorInfoList.ForEach(AddCollectActionBuffer);
        }

        private void AddCollectActionBuffer(CollectorInfo collectorInfo)
        {
            if (collectorInfo != null)
            {
                foreach (var info in collectActionBuffer)
                {
                    if (info.collectType == collectorInfo.collectType)
                    {
                        info.totalCount += collectorInfo.totalCount;
                        return;
                    }
                }

                collectActionBuffer.Enqueue(collectorInfo);
            }
        }

        public bool ReleaseCollectActionBuffer(Action releaseComplete = null)
        {
            bool released = collectActionBuffer.Count > 0;
            float delayTime = 0;
            if (released)
            {
                while (collectActionBuffer.Count > 0)
                {
                    CollectorInfo collectorInfo = collectActionBuffer.Dequeue();
                    if (collectActionBuffer.Count <= 0) // last
                    {
                        collectorInfo.collectCallback = collectorInfo.collectCallback ?? new CollectCallback();
                        collectorInfo.collectCallback.onCompleteAll += releaseComplete; // add complete callback
                    }

                    IAnimationCollector collector = FindCollector(collectorInfo.collectType);
                    ICollectUIUpdater targetUpdater = null;
                    if (collector != null)
                    {
                        targetUpdater = collector.GetCollectTargetUpdater(collectorInfo);
                    }

                    Timer.Schedule(delayTime, () => { Show(collectorInfo, collector, targetUpdater); });

                    delayTime += collector?.IntervalForSequence ?? 0;
                }
            }
            else
            {
                releaseComplete?.Invoke();
            }

            return released;
        }

        public CollectorInfo GetCollectorInfoFromBuffer(CollectType collectType)
        {
            foreach (CollectorInfo info in collectActionBuffer)
            {
                if (info.collectType == collectType)
                {
                    return info;
                }
            }

            return null;
        }

        public void ClearBuffer()
        {
            foreach (var collectorInfo in collectActionBuffer)
            {
                CallRealTimeSyncUpdaters(collectorInfo.collectType, false);
            }

            collectActionBuffer.Clear();
        }

        public bool HasCollectActionBuffer(CollectType collectType)
        {
            foreach (var collectorInfo in collectActionBuffer)
            {
                if (collectorInfo.collectType == collectType)
                {
                    return true;
                }
            }

            return false;
        }

        public ICollectUIUpdater ReduceCollection(CollectorInfo collectorInfo)
        {
            ICollectUIUpdater uiUpdater = ReduceCollection(collectorInfo.collectType, collectorInfo.totalCount);
            collectorInfo.collectCallback.onCompleteAll?.Invoke();
            return uiUpdater;
        }

        private ICollectUIUpdater ReduceCollection(CollectType type, long count)
        {
            ICollectUIUpdater updater = FindCollectUIUpdater(type);
            updater?.Reduce(count);

            // call real time sync updates
            CallRealTimeSyncUpdaters(type, true);
            return updater;
        }

        #region Regsiter & Get Collect Target

        public void RegisterCollectTarget(CollectType type, ICollectUIUpdater updater)
        {
            if (!collectUpdaters.TryGetValue(type, out List<ICollectUIUpdater> updaters))
            {
                updaters = new List<ICollectUIUpdater>();
                collectUpdaters.Add(type, updaters);
            }

            // move to last in updaters
            updaters.Remove(updater);
            updaters.Add(updater);

            // set activated
            updater.Activated = true;
        }

        public void UnRegisterCollectTarget(CollectType type, ICollectUIUpdater updater)
        {
            if (collectUpdaters.TryGetValue(type, out List<ICollectUIUpdater> updaters))
            {
                // set activated
                updater.Activated = false;
            }
        }

        public void DestroyCollectTarget(CollectType type, ICollectUIUpdater updater)
        {
            if (collectUpdaters.TryGetValue(type, out List<ICollectUIUpdater> updaters))
            {
                updaters.Remove(updater);
            }
        }

        private IAnimationCollector FindCollector(CollectType type)
        {
            collectors.TryGetValue(type, out IAnimationCollector collector);
            return collector;
        }

        public ICollectUIUpdater FindCollectUIUpdater(CollectType type, CollectorInfo collectorInfo = null)
        {
            if (collectUpdaters.TryGetValue(type, out List<ICollectUIUpdater> updaters))
            {
                ICollectUIUpdater temp = FindTopActivatedUpdate(updaters, collectorInfo);

                if (temp.IsNull() && updaters.Count > 0)
                {
                    temp = updaters.Last();
                }

                return temp;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreTopUpdater"> whether get current showed updater. (last in the updaters list) </param>
        /// <returns></returns>
        private List<ICollectUIUpdater> FindRealTimeSyncUpdaters(CollectType type, bool ignoreTopUpdater)
        {
            if (collectUpdaters.TryGetValue(type, out List<ICollectUIUpdater> target))
            {
                return ignoreTopUpdater ? FindUpdatersIgnoreTopUpdater(target) : target.FindAll(x => !x.IsNull());
            }

            return null;
        }

        /// <summary>
        /// will clear null updater
        /// </summary>
        /// <param name="target"></param>
        /// <param name="collectorInfo"></param>
        /// <returns></returns>
        private ICollectUIUpdater FindTopActivatedUpdate(List<ICollectUIUpdater> target, CollectorInfo collectorInfo)
        {
            for (int i = target.Count - 1; i >= 0; i--)
            {
                if (target[i] == null)
                {
                    target.RemoveAt(i);
                    continue;
                }

                if (target[i].Activated && target[i].CheckCollectorInfo(collectorInfo))
                {
                    return target[i];
                }
            }

            return null;
        }

        /// <summary>
        /// will clear null updater
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private List<ICollectUIUpdater> FindUpdatersIgnoreTopUpdater(List<ICollectUIUpdater> target)
        {
            List<ICollectUIUpdater> updaters = new List<ICollectUIUpdater>();
            for (int i = target.Count - 1; i >= 0; i--)
            {
                if (target[i] == null)
                {
                    target.RemoveAt(i);
                    continue;
                }

                updaters.Add(target[i]);
            }

            // ignore top
            if (updaters.Count > 0)
            {
                updaters.RemoveAt(0);
            }

            return updaters;
        }

        #endregion
    }
}