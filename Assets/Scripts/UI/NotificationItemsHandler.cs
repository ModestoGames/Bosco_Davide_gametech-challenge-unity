using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationItemsHandler : MonoBehaviour
{
    [SerializeField] private GameObject _scheduleNotificationButton;
    [SerializeField] private NotificationItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private RectTransform _layoutGroup;

    [Tooltip("How many fixedupdate steps are required for time text to update")]
    //used to avoid update text at every fixedupdate
    [SerializeField] private int _updateTimeInterval;

    private List<NotificationItem> _items = new List<NotificationItem>();
    private AndroidJavaObject _notificationHandler;
    private int _timer;
    private bool _initialized;


    public void Initialize(AndroidJavaObject notificationHandler)
    {
        _notificationHandler = notificationHandler;
        _timer = _updateTimeInterval;
        _initialized = true;
    }


    private void FixedUpdate()
    {
        if (!_initialized)
            return;

        _timer--;

        if (_timer <= 0)
        {
            var currentSystemTime = _notificationHandler.Call<long>("getCurrentSystemTime");

            foreach(var item in _items)
                item.UpdateTime(currentSystemTime);
        }
    }

    public void AddItem(int id, AndroidJavaObject androidNotificationDto)
    {
        NotificationDTO dto = new NotificationDTO(id, androidNotificationDto);
        Debug.Log(dto);
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Initialize(dto, this);
        _items.Add(item);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup);
    }

    public void AddItem(int id, NotificationDTO notificationDto)
    {
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Initialize(notificationDto, this);
        _items.Add(item);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup);
    }

    public void RemoveAll()
    {
        for(int i = _items.Count -1; i >= 0; i--)
            _items[i].RemoveItem();

        Disable();
    }

    public void RemoveItem(NotificationItem item)
    {
        _items.Remove(item);
        _notificationHandler.Call("deleteScheduledNotification", item.Id);

        if (_items.Count <= 0)
            Disable();
    }

    private void Disable()
    {
        _items = new List<NotificationItem>();
        _scheduleNotificationButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
