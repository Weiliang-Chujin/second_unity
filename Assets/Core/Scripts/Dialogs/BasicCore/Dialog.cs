using System.Linq;
using Core.Scripts.BasicModules.Components;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Dialogs.BasicCore
{
    public abstract class Dialog : WrappedMonoBehaviour
    {
        //对话框背景图片
        [SerializeField] protected Image background;

        //对话框的RecTransform组件
        [SerializeField] protected RectTransform viewTransform;

        //对话框动画：true展示,false不展示
        [SerializeField] protected bool isShowAnim = true;

        protected Vector3 OriginalScaleValue;

        public string DialogName { get; private set; }

        /// <summary>
        /// 根据名称初始化对话框
        /// </summary>
        /// <param name="dialogName">对话框名称</param>
        public void InitWith(string dialogName)
        {
            DialogName = dialogName;
            OriginalScaleValue = viewTransform.localScale;

            var canvas = GetComponent<Canvas>();
            if (canvas)
            {
                canvas.worldCamera = Camera.allCameras.FirstOrDefault(x => x.tag.Equals("UICamera"));
            }
        }

        //对话框显示(虚函数)
        public virtual void Show()
        {
        }

        public virtual void OnStackIn()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnStackOut()
        {
            gameObject.SetActive(true);
        }

        public virtual bool HandleBackEvent()
        {
            return false;
        }

        public virtual bool Closable()
        {
            return true;
        }

        //失去顶层位置
        public virtual void LoseFocus()
        {
        }

        //获得顶层位置
        public virtual void RestoreFocus()
        {
        }

        //隐藏对话框
        public virtual void OnHide()
        {
            //不激活
            gameObject.SetActive(false);
        }

        public virtual void OnRestore()
        {
            gameObject.SetActive(true);
        }

        public void OnReplace()
        {
            Close(false, false);
        }

        public void OnOpen()
        {
            var showAnim = GetShowAnimation();
            showAnim?.OnComplete(OpenComplete);
        }

        public virtual void OpenComplete()
        {
        }

        public virtual void Close()
        {
            Close(true, true);
        }

        public void Close(bool animated, bool playSound = true)
        {
            DoCloseAction(animated);

            bool res = DialogUtil.RemoveFromQueue(DialogName);

            if (res && playSound)
            {
                // todo close sound
            }
        }

        private void DoCloseAction(bool isAnimate = true)
        {
            var seq = GetHideAnimation();
            if (seq != null && isAnimate)
            {
                seq.OnComplete(DidClose);
            }
            else
            {
                DidClose();
            }
        }

        protected virtual void DidClose()
        {
            Dispose();
            DialogUtil.CheckAllCleared();
            DestroySelf();
        }

        public virtual void Dispose()
        {
        }

        public void DestroySelf()
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 动画显示
        /// </summary>
        /// <returns></returns>
        public virtual Sequence GetShowAnimation()
        {
            if (!isShowAnim) //若不显示返回空
            {
                return null;
            }

            CanvasGroup viewCanvasGroup = GetComponent<CanvasGroup>();
            if (!viewCanvasGroup)
            {
                viewCanvasGroup = viewTransform.GetComponent<CanvasGroup>();
                if (viewCanvasGroup)
                {
                    viewCanvasGroup.alpha = 0;
                }
            }

            viewTransform.localScale = 0.001f * Vector3.one;
            var showAnim = DOTween.Sequence();
            showAnim.AppendInterval(SetShowDelay());

            if (viewTransform)
            {
                showAnim.Append(viewTransform.DOScale(OriginalScaleValue, 0.5f).SetEase(Ease.OutBack));
            }

            if (background)
            {
                showAnim.Join(background.DOFade(GetAlpha(), 2 / 30f).SetEase(Ease.InOutSine));
            }

            if (viewCanvasGroup)
            {
                showAnim.Join(viewCanvasGroup.DOFade(1f, 0.15f));
            }

            showAnim.SetUpdate(true);
            showAnim.timeScale = 1f;
            return showAnim;
        }

        public virtual Sequence GetHideAnimation()
        {
            return null;
        }

        public Sequence GetFullHideAnimation()
        {
            var viewCanvasGroup = GetComponent<CanvasGroup>();
            if (null == viewCanvasGroup)
            {
                viewCanvasGroup = viewTransform.GetComponent<CanvasGroup>();
            }

            var hideAnim = DOTween.Sequence();
            if (null != viewCanvasGroup)
            {
                hideAnim.Join(viewCanvasGroup.DOFade(0f, 0.1f));
            }

            hideAnim.SetUpdate(true);
            return hideAnim;
        }

        protected virtual float SetShowDelay()
        {
            return 0f;
        }

        protected virtual float GetAlpha()
        {
            return 0.8f;
        }
    }

    public abstract class Dialog<T> : Dialog where T : DialogContext
    {
        public T dialogContext;

        public void ShowWithContext(DialogContext context)
        {
            dialogContext = context as T;
            Show();
        }

        public override void Dispose()
        {
            base.Dispose();
            dialogContext?.Dispose();
        }

        public override void Close()
        {
            base.Close();
            dialogContext?.onCloseAction?.Invoke();
        }
    }
}