using System;

namespace Core.Scripts.Dialogs.BasicCore
{
    public class DialogContext
    {
        public bool playOpenSound = true;

        public Action onCloseAction;

        public virtual void Dispose()
        {
        }
    }

    
    public class DialogContext<T> : DialogContext
    {
        public T AttachedData { get; protected set; }

        public DialogContext(T data = default(T))
        {
            AttachedData = data;
        }
    }
}