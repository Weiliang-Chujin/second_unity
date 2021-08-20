using System;
using Core.Scripts.AnimationCollector.Assembly.CollectItem;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.BasicModules.Components;
using Core.Scripts.BasicModules.Misc;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnimationCollector
{
    public abstract class BaseCollectItem : WrappedMonoBehaviour, ICollectItem
    {
        [SerializeField] protected SpriteRenderer render;

        private Sequence mainTween;
        protected float fps = 30;

        protected virtual float targetScale => 1f;

        public virtual void SetCollectorInfo(CollectorInfo collectorInfo)
        {
        }

        public void Do(Vector3 from, Vector3 to, int index, Action onComplete)
        {
            float interval = 0.1f;
            Transform ownTransform = transform;

            Vector3 offset = Random.insideUnitCircle * 0.1f;
            ownTransform.position = from + offset;
            ownTransform.localScale = Vector3.zero;

            GetShiftingPos(out var shiftingPos);
            Vector3 end = ownTransform.position + shiftingPos;

            render.SetAlpha(0);

            mainTween = DOTween.Sequence();
            if (index >= 3)
            {
                mainTween.AppendInterval(index / 3 * interval);
            }
            else
            {
                mainTween.AppendInterval(index * interval);
            }

            mainTween.Append(render.DOFade(1f, ShowDuration));
            mainTween.Join(ownTransform.DOLocalMove(end, ShowDuration));
            mainTween.Join(ownTransform.DOScale(targetScale, ShowDuration));
            mainTween.AppendInterval(StayDuration);
            mainTween.Append(ownTransform.DOMove(to, FlyDuration).SetEase(Ease.InSine));
            mainTween.OnComplete(() =>
            {
                onComplete?.Invoke();
                Complete();
                Recycle();
            });
        }

        protected virtual void Complete()
        {
        }

        private void GetShiftingPos(out Vector3 shiftingPos)
        {
            int randomX = Random.Range(1, 3);
            int randomY = Random.Range(1, 3);
            float shiftingX = randomX == 1 ? Random.Range(-0.2f, -1.2f) : Random.Range(0.2f, 1.2f);
            float shiftingY = randomY == 1 ? Random.Range(-0.1f, -0.5f) : Random.Range(0.1f, 0.5f);

            shiftingPos = new Vector3(shiftingX, shiftingY, 0);
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void Recycle()
        {
            Destroy(GameObject); // TODO for example
            // ObjectPool<BaseCollectItem>.PushObject(name, this);
        }

        private void OnDestroy()
        {
            mainTween?.Kill();
        }

        protected virtual float ShowDuration => 12 / fps;
        protected virtual float StayDuration => 6 / fps;
        protected virtual float FlyDuration => 18 / fps;
    }

    public abstract class BaseCollectItem<T> : BaseCollectItem where T : CollectorInfo
    {
        public sealed override void SetCollectorInfo(CollectorInfo collectorInfo)
        {
            SetCollectorInfo(collectorInfo as T);
        }

        protected virtual void SetCollectorInfo(T collectorInfo)
        {
        }
    }
}