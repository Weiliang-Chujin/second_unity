using AnimationCollector.Mediator;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Core.Scripts.BasicModules.Misc;
using DG.Tweening;
using Example.CollectAnimation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AnimationCollector.UIUpdater
{
    public class CurrencyUIUpdater : UIUpdaterBase
    {
        [SerializeField] private Image currencyIcon;
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private CurrencyType currency;
        private long curAmount;

        public UnityEvent onClickEvent;

        public UnityEvent onUpdateUI;

        protected override CollectType CollectType => CollectorInfoConvert.ConvertCollectType(currency);

        private Tweener updateTweener;

        private void Start()
        {
            RefreshBalance();
        }

        private void RefreshBalance()
        {
            curAmount = CollectExample.balance;
            currencyText.text = UnityUtil.NumberFormat(curAmount);
        }

        public void OnClickEvent()
        {
            onClickEvent?.Invoke();
        }

        public override Vector3 Position => currencyIcon.transform.position;

        public override void UpdateUI()
        {
            long to = CollectExample.balance;
            PlayUpdateAnimation(to);

            onUpdateUI?.Invoke();
        }

        public override void ForceSyncUI()
        {
            updateTweener?.Complete();

            RefreshBalance();
        }

        public override void Reduce(long count)
        {
            UpdateUI();
        }

        private void PlayUpdateAnimation(long to)
        {
            if (to != curAmount)
            {
                updateTweener?.Complete();
                updateTweener =
                    DOTween.To(
                        x => { currencyText.text = UnityUtil.NumberFormat((long) x); },
                        curAmount, to, 1.7f);
                curAmount = to;
            }
        }

        public override void ResponseCollection()
        {
            ResponseCollectionAnimation(currencyIcon.transform);
        }
    }
}