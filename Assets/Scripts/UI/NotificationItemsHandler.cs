using System.Collections.Generic;
using UnityEngine;

public class NotificationItemsHandler : MonoBehaviour
{
    [SerializeField] private NotificationItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;

    private List<NotificationItem> _items = new List<NotificationItem>();

    public void AddItem(int id, AndroidJavaObject androidNotificationDto)
    {
        Debug.Log("###ADD ITEM");
        NotificationDTO dto = new NotificationDTO(id, androidNotificationDto);
        Debug.Log(dto);
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Initialize(dto);
        _items.Add(item);
    }
}
