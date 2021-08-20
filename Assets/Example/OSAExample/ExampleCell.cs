using System;
using Core.Scripts.BasicModules.Components;
using UnityEngine;

namespace Core.Scripts.OSAExtend.Example
{
    public class ExampleCell : WrappedMonoBehaviour
    {
        private Action onClick;
        public void SetData(ExampleData data)
        {
            Debug.Log($"Count: {data.count}");
            Debug.Log($"Title: {data.title}");
        }

        public void SetupClickAction(Action onClick)
        {
            this.onClick = onClick;
        }
    }
}