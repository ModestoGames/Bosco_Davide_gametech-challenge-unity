using TMPro;
using UnityEngine;

public class NotificationItem : MonoBehaviour
{
    public int Id => _notification.Id;

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
        if(_notification.CreationTime >= _notification.SchedulationTime)
        {
            RemoveItem();
        }
    }

    public void RemoveItem()
    {
        _handler.RemoveItem(this);
        _animator.SetTrigger(Strings.AnimatorKeys.Hide);
    }

    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
