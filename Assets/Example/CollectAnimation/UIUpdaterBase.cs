using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.AnimationCollector.Assembly.UIUpdater;
using DG.Tweening;
using UnityEngine;

namespace AnimationCollector.UIUpdater
{
    public abstract class UIUpdaterBase : MonoBehaviour, ICollectUIUpdater
    {
        protected virtual void OnEnable()
        {
            CollectorManager.Instance.RegisterCollectTarget(CollectType, this);
        }

        protected virtual void OnDisable()
        {
            CollectorManager.Instance.UnRegisterCollectTarget(CollectType, this);
        }

        protected virtual void OnDestroy()
        {
            CollectorManager.Instance.DestroyCollectTarget(CollectType, this);
        }

        protected abstract CollectType CollectType { get; }
        public bool Activated { get; set; }
        public abstract Vector3 Position { get; }

        public virtual bool CheckCollectorInfo(CollectorInfo collectorInfo)
        {
            return true;
        }

        public abstract void UpdateUI();
        public abstract void ForceSyncUI();
        public abstract void Reduce(long count);

        public virtual void ResponseCollection()
        {
        }

        private Sequence responseSequence;

        protected void ResponseCollectionAnimation(Transform transform)
        {
            float fps = 30f;
            responseSequence?.Complete();
            responseSequence = DOTween.Sequence();
            responseSequence.Append(transform.DOScale(0.8f, 3 / fps));
            responseSequence.Append(transform.DOScale(1f, 2 / fps));
        }
    }

    public abstract class UIUpdaterBase<T> : UIUpdaterBase where T : CollectorInfo
    {
        public sealed override bool CheckCollectorInfo(CollectorInfo collectorInfo)
        {
            return CheckCollectorInfo(collectorInfo as T);
        }

        protected virtual bool CheckCollectorInfo(T collectorInfo)
        {
            return base.CheckCollectorInfo(collectorInfo);
        }
    }
}