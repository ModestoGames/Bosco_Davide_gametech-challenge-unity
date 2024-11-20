using UnityEngine;

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
