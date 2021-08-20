using System.Collections.Generic;
using Core.Scripts.BasicModules.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Dialogs.BasicCore
{
    public static class DialogUtil
    {
        private static readonly List<Dialog> dialogQueue = new List<Dialog>();

        /// <summary>
        /// 通过对话框名称创建并显示对话框,对话框类型为Dialog
        /// </summary>
        /// <param name="dialogName">对话框名称</param>
        /// <param name="option">
        /// 显示选项{kStack,kReplaceCurrent,kDontShowIfOthersShowing,kToast}
        /// 参考DialogShowDialog
        /// </param>
        /// <returns>创建成功返回对话框，否则为空</returns>
        public static Dialog ShowDialog(string dialogName, DialogShowOption option = DialogShowOption.kStack)
        {
            //通过名称获取对话框预制件
            Dialog dialogPrefab = Resources.Load<Dialog>($"Dialogs/{dialogName}");
            if (dialogPrefab) //若不为空
            {
                float matchNum = UnityUtil.GetMatchWidthOrHeight();
                dialogPrefab.GetComponent<CanvasScaler>().matchWidthOrHeight = matchNum;

                //实例化预制件
                var dialog = Object.Instantiate(dialogPrefab);
                //初始化对话框
                dialog.InitWith(dialogName);
                //根据选项添加动作
                DoAddAction(dialog, option);
                dialog.Show();
                return dialog;
            }

            Debug.LogError($"no find dialog => {dialogName}");

            return null;
        }

        /// <summary>
        /// 根据对话框名称和对话框内容创建对话框并显示,对话框类型为Dialog<T>.
        /// </summary>
        /// <param name="dialogName">对话框名称</param>
        /// <param name="context">对话框内容</param>
        /// <param name="option">显示选项</param>
        /// <typeparam name="T">内容类型</typeparam>
        /// <returns>创建成功返回对话框，否则为空</returns>
        public static Dialog ShowDialog<T>(string dialogName, T context,
            DialogShowOption option = DialogShowOption.kStack) where T : DialogContext
        {
            Dialog dialogPrefab = Resources.Load<Dialog>($"Dialogs/{dialogName}");
            if (dialogPrefab)
            {
                float matchNum = UnityUtil.GetMatchWidthOrHeight();
                dialogPrefab.GetComponent<CanvasScaler>().matchWidthOrHeight = matchNum;

                Dialog dialogSource = Object.Instantiate(dialogPrefab);
                if (dialogSource is Dialog<T> dialog)
                {
                    dialog.InitWith(dialogName);
                    DoAddAction(dialogSource, option);
                    dialog.ShowWithContext(context);
                    return dialogSource;
                }

                Debug.LogError($"Load dialog type error! {dialogName}");
            }

            Debug.LogError($"no find dialog => {dialogName}");

            return null;
        }

        /// <summary>
        /// 根据显示选项为对话框添加动作
        /// </summary>
        /// <param name="wantShowDialog">想显示的对话框</param>
        /// <param name="option">显示选项</param>
        private static void DoAddAction(Dialog wantShowDialog, DialogShowOption option = DialogShowOption.kStack)
        {
            if (!wantShowDialog) //若为空，返回
            {
                return;
            }

            var topMostDialog = GetTopMostDialog(); //获取最顶层对话框
            if (topMostDialog) //若存在
            {
                topMostDialog.LoseFocus();

                switch (option)
                {
                    case DialogShowOption.kStack:
                        topMostDialog.OnHide();
                        break;
                    case DialogShowOption.kToast:
                        break;

                    case DialogShowOption.kReplaceCurrent:
                        topMostDialog.OnReplace();
                        break;

                    case DialogShowOption.kDontShowIfOthersShowing:
                        wantShowDialog.Close();
                        return;

                    default:
                        topMostDialog.Close();
                        break;
                }
            }

            //获取想显示对话框在队列中的位置
            int index = dialogQueue.FindIndex(x => x == wantShowDialog);
            if (index >= 0) //若在队列中，将起移至队尾
            {
                dialogQueue.RemoveAt(index);
                dialogQueue.Add(wantShowDialog);
                wantShowDialog.OnRestore();
            }
            else //若不在，添加至队尾
            {
                dialogQueue.Add(wantShowDialog);
                wantShowDialog.OnOpen();
            }

            wantShowDialog.RestoreFocus();
        } //DoAddAction

        /// <summary>是否有需要显示的对话框</summary>
        public static bool IsHasDialogShowing()
        {
            return dialogQueue.Count > 0; //队列不为空返回true,为空false
        }

        public static bool IsDialogShowing(string name)
        {
            return dialogQueue.FindAll(x => x.DialogName.Equals(name)).Count > 0;
        }

        /// <summary>清除对话框</summary>
        public static void Clear()
        {
            for (var i = dialogQueue.Count - 1; i >= 0; i--)
            {
                Dialog dialog = dialogQueue[i];
                if (dialog)
                {
                    dialog.Close();
                }
            }
        }

        /// <summary>
        /// 强制关闭对话框
        /// </summary>
        /// <param name="dialogName">对话框名称</param>
        /// <returns></returns>
        public static bool ForceClose(string dialogName)
        {
            if (dialogQueue.Count >= 0)
            {
                var idx = dialogQueue.FindIndex(x => x.DialogName.Equals(dialogName));
                if (idx >= 0)
                {
                    var dialog = dialogQueue[idx];
                    dialogQueue.RemoveAt(idx);
                    dialog.Close();
                    return true;
                }
            }

            return false;
        }

        /// <summary>从对话框队列中移除制定对话框</summary>
        /// <param name="dialogName">指定移除的对话框名称</param>
        /// <returns></returns>
        public static bool RemoveFromQueue(string dialogName)
        {
            if (dialogQueue.Count >= 0)
            {
                var idx = dialogQueue.FindIndex(x => x.DialogName.Equals(dialogName));
                if (idx >= 0)
                {
                    dialogQueue.RemoveAt(idx);
                    var topMostDialog = GetTopMostDialog();
                    if (topMostDialog)
                    {
                        topMostDialog.OnRestore();
                        topMostDialog.RestoreFocus();
                    }

                    return true;
                }
            }

            return false;
        }

        public static bool ProcessEscapeRequest()
        {
            if (dialogQueue.Count <= 0) return false;

            Dialog dialog = dialogQueue[dialogQueue.Count - 1];
            if (dialog && !dialog.HandleBackEvent() && dialog.Closable())
            {
                dialog.Close(true);
            }

            return true;
        }

        /// <summary>检查队列中对话框是否被完全清除</summary>
        public static void CheckAllCleared()
        {
            if (dialogQueue == null || dialogQueue.Count == 0)
            {
                // call all dialog cleared event
            }
        }

        /// <summary>获取最顶层对话框</summary>
        /// <returns>队列为空:null,不为空:队尾对话框</returns>
        private static Dialog GetTopMostDialog()
        {
            return dialogQueue.Count > 0 ? dialogQueue[dialogQueue.Count - 1] : null;
        }
    }
}