using AnimationCollector;
using Core.Scripts.Audio;
using Core.Scripts.BasicModules.Components;
using Core.Scripts.Dialogs;
using UnityEngine;

namespace Example
{
    public class ExampleScript : WrappedMonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CommonDialogExample();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CollectExample();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CollectExample2();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CollectExample3();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                AudioExample();
            }
        }

        private void CommonDialogExample()
        {
            CommonMessageDialogCtx ctx = new CommonMessageDialogCtx();

            ctx.title = "Title1";
            ctx.msg = "Message1";
            ctx.btnTxtPositive = "OK1";
            ctx.btnTxtNegative = "Cancel";

            ctx.positiveAction = () => Debug.Log("positiveAction");
            ctx.negativeAction = () => Debug.Log("negativeAction");

            CommonMessageDialog.ShowDialog(ctx);
        }

        private void CollectExample()
        {
            CollectAnimation.CollectExample.balance += 1000;
            CollectorManager.Instance.Example();
        }

        private void CollectExample2()
        {
            CollectAnimation.CollectExample.balance += 500;
            CollectorManager.Instance.Example2();
        }

        private void CollectExample3()
        {
            CollectorManager.Instance.Example3();
        }

        private void AudioExample()
        {
            AudioPlayerHelper.Example();
        }
    }
}