using UnityEngine;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Base class to handle ui gameobjects with a hidden and visible state
    /// </summary>
    public abstract class UIItem : MonoBehaviour
    {
        public void Show()
        {
            OnBeforeShow();
            PerformShowAction();
        }

        public abstract void PerformShowAction();

        public void Hide()
        {
            OnBeforeHide();
            PerformHideAction();
        }

        public abstract void PerformHideAction();

        public virtual void OnBeforeShow() { }
        public virtual void OnAfterShow() { }
        public virtual void OnBeforeHide() { }
        public virtual void OnAfterHide() { }
    }
}