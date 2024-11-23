using UnityEngine;

namespace com.modesto.notificationhandler
{

    [RequireComponent(typeof(Canvas))]
    public class CanvasUIItem : UIItem
    {
        /// <summary>
        /// Implements UIItem with Canvas
        /// </summary>
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
}