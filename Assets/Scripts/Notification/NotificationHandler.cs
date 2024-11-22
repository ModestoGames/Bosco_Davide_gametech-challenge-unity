using com.modesto.notificationhandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;

public class NotificationHandler : AppStateListener
{
    [SerializeField] private NotificationDetail _notificationDetail;
    [SerializeField] private NotificationItemsHandler _notificationList;
    [Range(1, 5)]
    [SerializeField] private int _notificationNumber = 5;
    [Range(1, 10)]
    [SerializeField] private int _interval = 1;
    [SerializeField] private int _intervalSeconds = 1;

    private AndroidJavaObject _currentActivity;
    private AndroidJavaObject _notificationHandler;

    protected override IEnumerator OnInitialize()
    {
#if !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _notificationHandler = new AndroidJavaObject("com.modesto.notification_module.LocalNotificationsService", _currentActivity);
        }
#endif
        yield return null;
    }

    public override void AppStateChanged(ApplicationState previousState, ApplicationState currentState)
    {
        if (currentState == ApplicationState.Main)
        {
            Debug.Log("GO TO APPLICATION MAIN SO REFRESH");
#if UNITY_ANDROID && !UNITY_EDITOR
            RefreshNotifications();
            CheckForNotificationData();
#endif
        }
    }

    //clear and refresh the current notification status
    public void RefreshNotifications()
    {
        Debug.Log("#####Refresh Notification");
        var currentNotifications = _notificationHandler.Call<AndroidJavaObject>("getCurrentNotifications");
        Debug.Log(_notificationHandler.Call<string>("getNotificationAsString"));
        int currentNotificationsNumber = currentNotifications.Call<int>("size");
        Debug.Log($"Number of current notifications: {currentNotificationsNumber}");

        if (currentNotificationsNumber <= 0)
            return;

        _notificationList.Clear();
        Debug.Log("Clear List");

        List<NotificationDTO> notificationDtos = new List<NotificationDTO>();
        //use notification number instead of currentNotificationsNumber
        //because what you receive is a HashMap with <int, dto>
        for (int i = 1; i < _notificationNumber + 1; i++)
        {
            var key = new AndroidJavaObject("java.lang.Integer", i);
            var androidDto = currentNotifications.Call<AndroidJavaObject>("get", key);

            //add each android notification to a list
            if (androidDto != null)
            {
                notificationDtos.Add(new NotificationDTO(i, androidDto));
                Debug.Log($"Notification added with id : {i}");
            }
            else
            {
                Debug.LogWarning($"No notification found for index: {i}");
            }
        }

        Debug.Log($"Total notifications added before sorting: {notificationDtos.Count}");
        //reorder the list starting from the nearest scheduled
        notificationDtos = notificationDtos.OrderBy(n => n.DurationInSeconds).ToList();
        foreach (var dto in notificationDtos)
            Debug.Log("###########" + dto.DurationInSeconds);
        //rebuild the list already in the correct order
        foreach (NotificationDTO notificationDto in notificationDtos)
        {
            _notificationList.AddItem(notificationDto);
            Debug.Log($"Notification added to _notificationList: {notificationDto.DurationInSeconds}");
        }

        //restart list handler
        _notificationList.Initialize(_notificationHandler);
        Debug.Log("Notification list handler initialized");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // L'app torna in foreground
        {
            CheckForNotificationData();
        }
    }

    public void ScheduleNotification()
    {
        Debug.Log("Try schedule notification");
#if UNITY_EDITOR
        DebugScheduleNotifications();
#else
        try
        {
            _notificationList.Initialize(_notificationHandler);
            for (int i = 1; i <= _notificationNumber; i++)
            {
                int delay = i * _intervalSeconds;
                //the java method return the dto for the scheduled notification
                var notificationDto = _notificationHandler.Call<AndroidJavaObject>("scheduleNotification", i, delay);
                _notificationList.AddItem(i, notificationDto);
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.InnerException);
            Debug.Log(e.StackTrace);
        }
#endif
    }

    private void DebugScheduleNotifications()
    {
        for (int i = 1; i <= _notificationNumber; i++)
        {
            int delay = i * _interval;
            //the java method return the dto for the scheduled notification
            var notificationDto = new NotificationDTO(i)
            {
                WorkUUID = i.ToString(),
                Status = NotificationStatus.Pending,
                Title = "Title " + i,
                Text = "Text " + i,
                CreationTime = i,
                SchedulationTime = i * (60 * 1000)
            };
            _notificationList.AddItem(notificationDto);
        }
    }

    private void CheckForNotificationData()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Check for notification");
            using (AndroidJavaObject intent = _currentActivity.Call<AndroidJavaObject>("getIntent"))
            {
                if (intent.Call<bool>("hasExtra", "notification_id"))
                {
                    // Leggi i dati dall'intent
                    int notificationId = intent.Call<int>("getIntExtra", "notification_id", -1);
                    string title = intent.Call<string>("getStringExtra", "notification_title");
                    string text = intent.Call<string>("getStringExtra", "notification_text");

                    // Gestisci i dati della notifica
                    HandleNotificationData(notificationId, text, title);

                    // Pulisci l'intent per evitare di processare pi� volte gli stessi dati
                    intent.Call("removeExtra", "notification_id");
                    intent.Call("removeExtra", "notification_title");
                    intent.Call("removeExtra", "notification_text");
                }
            }
        }
    }

    public void TestSwitchNotification()
    {
        _notificationHandler.Call("switchNotificationSchedule", 1, 4);
        RefreshNotifications();
    }

    private void HandleNotificationData(int id, string text, string title)
    {
        try
        {
            Debug.Log("Try to create sprite");
            Sprite sprite = Utils.GetSprite(_currentActivity, id);
            _notificationDetail.ShowNotificationDetails(title, text, sprite);
        }
        catch (Exception e)
        {
            Debug.Log("Unable to create sprite");
            _notificationDetail.ShowNotificationDetails(title, text, null);
        }
    }
}
