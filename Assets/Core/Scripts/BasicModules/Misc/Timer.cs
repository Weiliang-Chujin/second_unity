using System;
using System.Collections;
using UnityEngine;

namespace Core.Scripts.BasicModules.Misc
{
    public class Timer : MonoBehaviour
    {
        private static MonoBehaviour TimerLauncher => MonoSingleGenerator<Timer>.Entity();

        public static Coroutine Schedule(float delay, Action task)
        {
            return TimerLauncher.StartCoroutine(DoTask(task, delay));
        }

        private static IEnumerator DoTask(Action task, float delay)
        {
            yield return new WaitForSeconds(delay);
            task?.Invoke();
        }

        public static void ScheduleRealtime(float delay, Action task)
        {
            TimerLauncher.StartCoroutine(DoTaskRealtime(task, delay));
        }

        private static IEnumerator DoTaskRealtime(Action task, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            task();
        }

        public static Coroutine ScheduleEndOfFrame(Action task)
        {
            return TimerLauncher.StartCoroutine(DoTaskEndOfFrame(task));
        }

        private static IEnumerator DoTaskEndOfFrame(Action task)
        {
            yield return new WaitForEndOfFrame();
            task();
        }
    }
}