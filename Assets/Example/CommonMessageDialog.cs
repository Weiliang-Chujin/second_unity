using System;
using Core.Scripts.BasicModules.Misc;
using Core.Scripts.Dialogs.BasicCore;
using TMPro;
using UnityEngine;

namespace Core.Scripts.Dialogs
{
    public class CommonMessageDialogCtx : DialogContext
    {
        public string title;
        public string msg;
        public string btnTxtPositive;
        public string btnTxtNegative;

        public Action positiveAction;
        public Action negativeAction;
        public Action closeClickAction;

        public bool showCloseBtn = true;
    }

    public class CommonMessageDialog : Dialog<CommonMessageDialogCtx>
    {
        [Header("Custom")] [SerializeField] private TextMeshProUGUI titleTxt;
        [SerializeField] private TextMeshProUGUI msgTxt;
        [SerializeField] private TextMeshProUGUI btnPositiveTxt;
        [SerializeField] private TextMeshProUGUI btnNegativeTxt;
        [SerializeField] private GameObject positiveBtn;
        [SerializeField] private GameObject negativeBtn;
        [SerializeField] private GameObject closeBtn;
        [SerializeField] private AnimatedButton emptyButton;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="buttonStr"> default value = "OK" </param>
        /// <param name="onConfirmAction"></param>
        /// <param name="onCloseAction"></param>
        public static void ShowDialog(string title, string msg, string buttonStr = "OK", Action onConfirmAction = null,
            Action onCloseAction = null)
        {
            CommonMessageDialogCtx dialogContext = new CommonMessageDialogCtx
            {
                title = title,
                msg = msg,
                btnTxtPositive = buttonStr,
                positiveAction = onConfirmAction,
                onCloseAction = onCloseAction,
            };
            ShowDialog(dialogContext);
        }

        public static Dialog ShowDialog(CommonMessageDialogCtx ctx)
        {
            string prefab = "CommonMessageDialog";
            return DialogUtil.ShowDialog(prefab, ctx, DialogShowOption.kToast);
        }

        public override void Show()
        {
            base.Show();
            SetContent();
        }

        protected virtual void SetContent()
        {
            titleTxt.text = dialogContext.title;
            msgTxt.text = dialogContext.msg;
            btnPositiveTxt.text = dialogContext.btnTxtPositive;
            btnNegativeTxt.text = dialogContext.btnTxtNegative;

            if (string.IsNullOrEmpty(dialogContext.btnTxtPositive))
            {
                positiveBtn.SetActive(false);
            }

            if (string.IsNullOrEmpty(dialogContext.btnTxtNegative))
            {
                negativeBtn.SetActive(false);
            }

            emptyButton.enabled = dialogContext.showCloseBtn;
            closeBtn.gameObject.SetActiveVirtual(dialogContext.showCloseBtn);
        }

        public void OnCloseBtnClick()
        {
            // callback in closing before
            dialogContext.closeClickAction?.Invoke();

            Close();
        }

        public void OnPositiveBtnClick()
        {
            // callback in closing before
            dialogContext.positiveAction?.Invoke();
            Close();
        }

        public void OnNegativeBtnClick()
        {
            // callback in closing before
            dialogContext.negativeAction?.Invoke();
            Close();
        }
    }
}