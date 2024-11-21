using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationItemsHandler : MonoBehaviour
{
    [SerializeField] private NotificationItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private RectTransform _layoutGroup;

    private List<NotificationItem> _items = new List<NotificationItem>();

    public void AddItem(int id, AndroidJavaObject androidNotificationDto)
    {
        NotificationDTO dto = new NotificationDTO(id, androidNotificationDto);
        Debug.Log(dto);
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Initialize(dto);
        _items.Add(item);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup);
    }
}
