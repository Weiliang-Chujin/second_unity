using System;
using System.Text;
using Core.Scripts.BasicModules.Misc;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Scripts.Dialogs
{
    public class AnimatedButton : UIBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler,
        IInitializePotentialDragHandler, IScrollHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {
        }

        //可点击
        [SerializeField] private bool interactable = true;

        //点击变换颜色
        [Space(10), SerializeField] public bool colorTint = false;
        [SerializeField] public Color normalColor = new Color(1, 1, 1, 1);
        [SerializeField] public Color pressColor = new Color(0.78f, 0.78f, 0.78f, 1);
        [SerializeField] public Color disableColor = new Color(0.66f, 0.66f, 0.66f, 1);

        //点击变换图片
        [Space(10), SerializeField] public bool spriteSwap = false;
        [SerializeField] private Image btnImage;
        [NonSerialized] public Sprite normalSprite;
        [SerializeField] public Sprite clickSprite;
        [SerializeField] public Sprite disableSprite;

        //点击动画
        [Space(10), SerializeField] public bool clickAnim = true;
        [Range(0, 1)] [SerializeField] private float overshoot = 1;
        private Vector3 originScale = Vector3.one;
        private Sequence btnSequence;

        //点击Loading
        [Space(10), SerializeField] public bool clickLoading = false;
        [SerializeField] private GameObject loadingMask;

        public bool isShowLoadingMask => loadingMask && loadingMask.gameObject.activeSelf;

        //点击事件
        [Space(10), SerializeField] private ButtonClickedEvent m_onClick = new ButtonClickedEvent();

        //点击声音
        [SerializeField] public bool clickSound = true;

        //是否可快速点击（正常: 1秒点击间隔 | 快速点击: 0.1f点击间隔）
        [SerializeField] private bool quickClick = false;
        private static bool _blockInput;

        //是否只有联网时才响应点击事件
        [SerializeField] public bool onlyHandleClickOnNetworkOK = false;

        //事件回调（给外部脚本使用）
        public ButtonClickedEvent onPointUp = new ButtonClickedEvent();
        public ButtonClickedEvent onPointDown = new ButtonClickedEvent();

        public bool Interactable
        {
            private get => interactable;
            set
            {
                interactable = value;
                ResetBtnView();
            }
        }

        public ButtonClickedEvent onClick => m_onClick;

        public Image BtnImage
        {
            get
            {
                if (!btnImage)
                {
                    btnImage = GetComponent<Image>();
                }

                return btnImage;
            }
        }

        private Sequence blockSeq;

        protected override void Awake()
        {
            InitBtnView();
        }

        protected override void OnEnable()
        {
            ResetBtnView();
        }

        private void InitBtnView()
        {
            if (!btnImage)
            {
                btnImage = GetComponent<Image>();
            }

            if (btnImage)
            {
                normalSprite = btnImage.sprite;
                var originAlpha = btnImage.color.a;
                normalColor.a = originAlpha;
                pressColor.a = originAlpha;
                disableColor.a = originAlpha;
            }

            originScale = transform.localScale;

#if UNITY_EDITOR
            if (clickAnim && originScale != Vector3.one)
            {
                Transform curTrans = transform;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Insert(0, $" => {curTrans.name}");
                for (int i = 0; i < 10; i++)
                {
                    var parentTrans = curTrans.parent;
                    if (!parentTrans)
                    {
                        break;
                    }

                    curTrans = parentTrans;
                    stringBuilder.Insert(0, $" => {curTrans.name}");
                }

                Debug.LogWarning($"{stringBuilder} : AnimatedButton按钮初始 localScale 不为 1，请检查是否和按钮缩放动画冲突");
            }
#endif
        }

        private void ResetBtnView()
        {
            if (clickAnim)
            {
                transform.localScale = originScale;
            }

            if (!btnImage)
            {
                return;
            }

            if (spriteSwap && normalSprite && disableSprite)
            {
                btnImage.sprite = Interactable ? normalSprite : disableSprite;
            }

            if (colorTint)
            {
                btnImage.color = Interactable ? normalColor : disableColor;
            }
        }

        /// <summary>
        /// Called when there is a click/touch over the button.
        /// </summary>
        /// <param name="eventData">The data associated to the pointer event.</param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActiveAndEnabled(eventData) || _blockInput || isShowLoadingMask)
            {
                return;
            }

            if (LargeDrag())
            {
                return;
            }

            _blockInput = true;
            blockSeq?.Kill();
            blockSeq = DOTween.Sequence().SetUpdate(true).AppendInterval(quickClick ? 0.1f : 0.3f)
                .AppendCallback(() => _blockInput = false);

            Press();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointDown?.Invoke();

            if (!IsActiveAndEnabled(eventData))
            {
                return;
            }

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);
            }

            ChangeBtnSprite(true);
            ChangeBtnColor(true);
            ChangeBtnAnim(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointUp?.Invoke();

            if (!IsActiveAndEnabled(eventData))
            {
                return;
            }

            if (clickSound && !_blockInput)
            {
                // AudioHelper.Click1(); TODO click sound
            }

            ChangeBtnColor(false);
            ChangeBtnSprite(false);
            ChangeBtnAnim(false);
        }

        private bool IsActiveAndEnabled(PointerEventData eventData)
        {
            return Interactable && (eventData == null || eventData.button == PointerEventData.InputButton.Left);
        }

        /// <summary>
        /// Sets this button as pressed.
        /// </summary>
        private void Press()
        {
            if (!IsActive())
            {
                return;
            }

            if (onlyHandleClickOnNetworkOK && !UnityUtil.IsInternetConnected())
            {
                // TODO network error handle
                // SocketRequestStatusCenter.Instance.NetworkDisconnected(Press);
                return;
            }

            onClick.Invoke();
        }

        private void ChangeBtnSprite(bool isPressed)
        {
            if (spriteSwap && Interactable && btnImage && clickSprite)
            {
                btnImage.sprite = isPressed ? clickSprite : normalSprite;
            }
        }

        private void ChangeBtnColor(bool isPressed)
        {
            if (colorTint && Interactable && btnImage)
            {
                btnImage.color = isPressed ? pressColor : normalColor;
            }
        }

        private void ChangeBtnAnim(bool isPressed)
        {
            if (clickAnim && Interactable)
            {
                btnSequence?.Kill();
                btnSequence = DOTween.Sequence();
                if (isPressed)
                {
                    btnSequence.Append(transform.DOScale((1 + (-0.1f * overshoot)) * originScale, 2 / 30f)
                        .SetEase(Ease.InOutSine));
                }
                else
                {
                    btnSequence.Append(transform.DOScale((1 + (0.12f * overshoot)) * originScale, 2 / 30f)
                        .SetEase(Ease.InSine));
                    btnSequence.Append(transform.DOScale(1f * originScale, 4 / 30f).SetEase(Ease.OutSine));
                }

                btnSequence.SetUpdate(true);
                btnSequence.OnComplete(() => btnSequence = null);
            }
        }

        public void StopBtnAnim()
        {
            btnSequence?.Kill();
            transform.localScale = originScale;
        }

        private void ChangeBtnLoading(bool showLoading)
        {
            if (clickLoading && loadingMask)
            {
                loadingMask.gameObject.SetActiveVirtual(showLoading);
            }
        }

        public void ShowLoadingMask()
        {
            ChangeBtnLoading(true);
        }

        public void CloseLoadingMask()
        {
            ChangeBtnLoading(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            btnSequence?.Kill();
            btnSequence = null;
        }

        private bool LargeDrag()
        {
            return Vector2.SqrMagnitude(beginDragPosition - currentDragPosition) > 4000;
        }

        private Vector2 beginDragPosition;
        private Vector2 currentDragPosition;

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            ExecuteParentEventHandler<IInitializePotentialDragHandler>(x => x.OnInitializePotentialDrag(eventData));
        }

        public void OnDrag(PointerEventData eventData)
        {
            ExecuteParentEventHandler<IDragHandler>(x => x.OnDrag(eventData));

            currentDragPosition = eventData.position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ExecuteParentEventHandler<IBeginDragHandler>(x => x.OnBeginDrag(eventData));

            beginDragPosition = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ExecuteParentEventHandler<IEndDragHandler>(x => x.OnEndDrag(eventData));

            // zero
            currentDragPosition = beginDragPosition = Vector2.zero;
        }

        public void OnScroll(PointerEventData eventData)
        {
            ExecuteParentEventHandler<IScrollHandler>(x => x.OnScroll(eventData));
        }

        /// <summary>
        /// 调用父级的拖拽事件
        /// </summary>
        /// <param name="functor"></param>
        /// <typeparam name="T"></typeparam>
        private void ExecuteParentEventHandler<T>(Action<T> functor) where T : IEventSystemHandler
        {
            if (transform.parent)
            {
                T[] results = transform.parent.GetComponentsInParent<T>();

                foreach (T result in results)
                {
                    functor.Invoke(result);
                }
            }
        }
    }
}