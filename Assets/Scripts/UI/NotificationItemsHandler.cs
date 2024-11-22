using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationItemsHandler : MonoBehaviour
{
    [SerializeField] private NotificationHandler _notificationHandler;
    [SerializeField] private GameObject _scheduleNotificationButton;
    [SerializeField] private GameObject _deleteAllButton;
    [SerializeField] private NotificationItem _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private VerticalLayoutGroup _layoutGroup;

    [Tooltip("How many fixedupdate steps are required for time text to update")]
    //used to avoid update text at every fixedupdate
    [SerializeField] private int _updateTimeInterval;

    private List<NotificationItem> _items = new List<NotificationItem>();
    private AndroidJavaObject _notificationService;
    private int _timer;
    private bool _initialized;

    private List<int> _oldSorting;
    private List<int> _newSorting;

    private int _initialSiblingIndex;
    private int _currentSiblingIndex;
    private int _initialYPos;
    private float _listSectionHeight;

    public void Initialize(AndroidJavaObject notificationService)
    {
        _notificationService = notificationService;
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
            var currentSystemTime = _notificationService.Call<long>("getCurrentSystemTime");

            for(int i = 0; i < _items.Count; i++)
            {
                _items[i].UpdateTime(currentSystemTime);
            }
        }
    }

    public void AddItem(int id, AndroidJavaObject androidNotificationDto)
    {
        NotificationDTO dto = new NotificationDTO(id, androidNotificationDto);
        Debug.Log(dto);
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Initialize(dto, this);
        item.gameObject.name = id.ToString();
        _items.Add(item);
    }

    public void AddItem(NotificationDTO notificationDto)
    {
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Initialize(notificationDto, this);
        item.gameObject.name = notificationDto.WorkUUID;
        _items.Add(item);
    }

    //this method remove all the notifications from Unity and then from Android
    public void RemoveAll()
    {
        for (int i = _items.Count - 1; i >= 0; i--)
            _items[i].RemoveItem();

        StartCoroutine(DisableWithDelay());
    }

    //this method remove a single notification from Unity and then from Android
    public void RemoveItem(NotificationItem item, bool alsoRemoveScheduledNotification = true)
    {
        _items.Remove(item);
#if !UNITY_EDITOR
        if(alsoRemoveScheduledNotification)
            _notificationService.Call("deleteScheduledNotification", item.Id);
#endif
        if (_items.Count <= 0)
            StartCoroutine(DisableWithDelay());
    }

    private IEnumerator DisableWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Disable();
    }

    //this method remove notifications only from Unity.
    //Use to refresh notification after a reschedule or application restart
    public void Clear()
    {
        _initialized = false;

        if (_items.Count <= 0)
            return;

        for (int i = _items.Count - 1; i >= 0; i--)
            Destroy(_items[i].gameObject);

        _items = new List<NotificationItem>();
    }

    private void Disable()
    {
        _items = new List<NotificationItem>();
        _scheduleNotificationButton.SetActive(true);
        _deleteAllButton.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnBeginDragItem(int draggableItemTransformIndex, float initialYPosition, float height)
    {
        //initialize the two sorting lists
        _oldSorting = new List<int>();
        _newSorting = new List<int>();

        //create the list with the sorting before drag
        for(int i = 0; i < _itemContainer.childCount; i++)
        {
            int id = _itemContainer.GetChild(i).GetComponent<NotificationItem>().Id;
            _oldSorting.Add(id);
        }

        //store the initial position
        _initialSiblingIndex = draggableItemTransformIndex;
        //get the height of a single element in layout
        _listSectionHeight = _itemContainer.GetComponent<RectTransform>().sizeDelta.y / _items.Count;

    }

    public void OnDragItem(Transform transform, float itemHeight)
    {
        float normalizedY = _listSectionHeight / 2f - (_itemContainer.GetChild(transform.GetSiblingIndex())
            .GetComponent<RectTransform>().anchoredPosition.y + _layoutGroup.spacing);

        int spaceIndex = Mathf.Clamp((int)(normalizedY / _listSectionHeight), 0, _items.Count);

        if (transform.GetSiblingIndex() != spaceIndex)
            transform.SetSiblingIndex(spaceIndex);
    }

    public void OnDropItem()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_itemContainer.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        for(int i = 0; i < _itemContainer.childCount; i++)
        {
            int id = _itemContainer.GetChild(i).GetComponent<NotificationItem>().Id;
            _newSorting.Add(id);
        }

        if (!Utils.ListsSortingIsEqual(_oldSorting, _newSorting))
        {
            var oldSorting = _oldSorting.ToArray();
            var newSorting = _newSorting.ToArray();

            Debug.Log(oldSorting);
            Debug.Log(newSorting);

            _notificationService.Call("setOldSorting", oldSorting);
            _notificationService.Call("setNewSorting", newSorting);
            _notificationHandler.RefreshNotifications();
        }
    }

    public void PrintArray(List<int> _newSorting)
    {
        // Controlla se _newSorting � vuoto
        if (_newSorting == null || _newSorting.Count == 0)
        {
            Debug.Log("New sorting array is empty or null.");
            return;
        }

        // Costruisce una stringa con tutti gli ID
        string sortingOutput = "New Sorting: [";
        for (int i = 0; i < _newSorting.Count; i++)
        {
            sortingOutput += _newSorting[i];
            if (i < _newSorting.Count - 1)
            {
                sortingOutput += ", ";
            }
        }
        sortingOutput += "]";

        // Stampa nel log
        Debug.Log(sortingOutput);
    }
}