using TMPro;
using UnityEngine;

public class NotificationItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _time;

    private NotificationDTO _notification;

    public void Initialize(NotificationDTO notification)
    {
        _notification = notification;
        _name.text = "PUSH " + _notification.Id;
        _time.text = _notification.GetPrettyDurationString();
    }
}
