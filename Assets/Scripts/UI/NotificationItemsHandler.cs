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

    public void SwitchItems(int item1Id, int item2Id)
    {
        item1Id--; item2Id-- ;
        int index1 = _items[item1Id].transform.GetSiblingIndex();
        Debug.Log("Index 1 is " + index1);
        int index2 = _items[item2Id].transform.GetSiblingIndex();
        Debug.Log("Index 2 is " + index2);
        _items[item1Id].transform.SetSiblingIndex(index2);
        _items[item2Id].transform.SetSiblingIndex(index1);

        //switch schedulation times
        var tempSchedulationTime = _items[item1Id].SchedulationTime;
        _items[item1Id].SchedulationTime = _items[item2Id].SchedulationTime;
        _items[item2Id].SchedulationTime = tempSchedulationTime;

        //switch elements in list
        var temp = _items[item1Id];
        _items[item1Id] = _items[item2Id];
        _items[item2Id] = temp;
    }

    private void Disable()
    {
        _items = new List<NotificationItem>();
        _scheduleNotificationButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
