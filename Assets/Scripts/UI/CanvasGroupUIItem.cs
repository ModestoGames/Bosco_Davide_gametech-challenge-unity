using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class CanvasGroupUIItem : UIItem
{
    [Header("Graphic")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float _maxAlpha;
    [SerializeField] private bool _interactableWhenEnabled;

    public override void PerformShowAction()
    {
        _canvasGroup.alpha = _maxAlpha;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = _interactableWhenEnabled;
    }

    public override void OnBeforeShow()
    {
        base.OnBeforeShow();
        _canvasGroup.enabled = true;
    }

    public override void PerformHideAction()
    {
        _canvasGroup.alpha = 0.0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        OnAfterHide();
    }
}
