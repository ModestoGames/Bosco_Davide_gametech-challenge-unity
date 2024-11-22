using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotificationItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Id => _notification.Id;

    public float PosY => _draggable.anchoredPosition.y;
    public float SiblingIndex => _draggable.GetSiblingIndex();

    public long SchedulationTime
    {
        get { return _notification.SchedulationTime; }
        set { _notification.SchedulationTime = value; }
    }

    [SerializeField] private RectTransform _draggable;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private Animator _animator;

    private NotificationDTO _notification;
    private NotificationItemsHandler _handler;

    public void Initialize(NotificationDTO notification, NotificationItemsHandler handler)
    {
        _notification = notification;
        _handler = handler;
        _name.text = "PUSH " + _notification.Id;
        _time.text = _notification.GetPrettyDurationString();
    }

    public void UpdateTime(long androidSystemTime)
    {
        _notification.CreationTime = androidSystemTime;
        _time.text = _notification.GetPrettyDurationString();

        //check if all time elapsed
        if (_notification.CreationTime >= _notification.SchedulationTime)
        {
            RemoveItem(alsoRemoveScheduledNotification: false);
        }
    }

    public void RemoveItem(bool alsoRemoveScheduledNotification = true)
    {
        _handler.RemoveItem(this, alsoRemoveScheduledNotification);
        _animator.SetTrigger(Strings.AnimatorKeys.Hide);
    }

    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _handler.OnBeginDragItem(transform.GetSiblingIndex(), _draggable.anchoredPosition.y, _draggable.sizeDelta.y);
        Debug.Log("Initial sibling index is " + transform.GetSiblingIndex());
    }

    public void OnDrag(PointerEventData eventData)
    {
        _draggable.transform.position = new Vector2(_draggable.transform.position.x, Input.mousePosition.y);
        _handler.OnDragItem(transform, _draggable.sizeDelta.y);
    }

    public void ResetPosition(float posY)
    {
        _draggable.anchoredPosition = new Vector2(_draggable.anchoredPosition.x, posY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _handler.OnDropItem();
    }
}
