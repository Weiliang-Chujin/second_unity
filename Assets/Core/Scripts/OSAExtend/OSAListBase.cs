using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.DataHelpers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Scripts.OSAExtend
{
    public abstract class OSAListBase<TData, TViewsHolder> : OSA<BaseParamsWithPrefabII, TViewsHolder>
        where TViewsHolder : BaseItemViewsHolder, new()
    {
        public SimpleDataHelper<TData> Data { get; protected set; }

        public UnityEvent OnItemsUpdated;

        protected virtual float DistanceOffsetY { get; set; } = 80;

        protected bool isPullDown;
        protected bool isPullUp;

        private Vector2 originOffsetMax;
        private Vector2 originOffsetMin;

        protected override void Awake()
        {
            Data = new SimpleDataHelper<TData>(this);
            base.Awake();
        }

        public virtual void Setup()
        {
            originOffsetMax = Viewport.offsetMax;
            originOffsetMin = Viewport.offsetMin;
        }

        public virtual void SetItems(IList<TData> items)
        {
            Data.ResetItems(items);
        }

        protected override TViewsHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new TViewsHolder();
            instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

            OnCreateViewsHolderComplete(instance);
            return instance;
        }

        protected virtual void OnCreateViewsHolderComplete(TViewsHolder tViewsHolder)
        {
        }

        protected override void UpdateViewsHolder(TViewsHolder newOrRecycled)
        {
            TData model = Data[newOrRecycled.ItemIndex];
            SetViewsHolderData(newOrRecycled, model);
        }

        protected abstract void SetViewsHolderData(TViewsHolder viewsHolder, TData model);

        public virtual void FinishPullToRefresh()
        {
            OnItemsUpdated?.Invoke();
        }

        public virtual void PullDown()
        {
            isPullDown = true;
            Viewport.offsetMax = new Vector2(originOffsetMax.x, originOffsetMax.y - DistanceOffsetY);
        }

        public virtual void PullUp()
        {
            isPullUp = true;
            Viewport.offsetMin = new Vector2(originOffsetMin.x, originOffsetMin.y + DistanceOffsetY);
        }

        public virtual void PullComplete()
        {
            if (isPullDown)
            {
                isPullDown = false;
                DOTween.To(() => Viewport.offsetMax.y, y => Viewport.offsetMax = new Vector2(originOffsetMax.x, y),
                    originOffsetMax.y, 0.3f);
            }
            else if (isPullUp)
            {
                isPullUp = false;
                Viewport.offsetMin = new Vector2(originOffsetMin.x, originOffsetMin.y);
            }

            Parameters.SetDragEnable(true);
            FinishPullToRefresh();
        }
    }
}