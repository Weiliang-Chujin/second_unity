using UnityEngine;

namespace Core.Scripts.BasicModules.Misc
{
    public static class AnimatorSetter
    {
        public static void SmartSetTrigger(this Animator animator, string trigger)
        {
            ResetAllActiveTriggers(animator);
            animator.SetTrigger(trigger);
        }

        private static void ResetAllActiveTriggers(Animator animator)
        {
            AnimatorControllerParameter[] aps = animator.parameters;
            for (int i = 0; i < aps.Length; i++)
            {
                AnimatorControllerParameter paramItem = aps[i];
                if (paramItem.type == AnimatorControllerParameterType.Trigger)
                {
                    string triggerName = paramItem.name;
                    bool isActive = animator.GetBool(triggerName);
                    if (isActive)
                    {
                        animator.ResetTrigger(triggerName);
                    }
                }
            }
        }
    }
}