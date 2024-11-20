using UnityEngine;

[RequireComponent (typeof(Canvas))]
public class CanvasUIItem : UIItem
{
    [Header("Graphic")]
    [SerializeField] private Canvas _canvas;

    public override void PerformHideAction()
    {
        _canvas.enabled = false;
    }

    public override void PerformShowAction()
    {
        _canvas.enabled = true;
    }
}
