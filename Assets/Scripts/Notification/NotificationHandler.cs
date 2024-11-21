using com.modesto.notificationhandler;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class NotificationHandler : AppStateListener
{
    [SerializeField] private NotificationDetail _notificationDetail;
    [SerializeField] private NotificationsDebugger _debugger;
    [SerializeField] private NotificationItemsHandler _notificationList;
    [Range(1, 5)]
    [SerializeField] private int _notificationNumber = 5;
    [Range(1, 10)]
    [SerializeField] private int _interval = 1;

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
        //if (currentState == ApplicationState.Main)
        //    StartCoroutine(DebugCheckNotificationData());
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
                int delay = i * _interval;
                //the java method return the dto for the scheduled notification
                var notificationDto = _notificationHandler.Call<AndroidJavaObject>("scheduleNotification", i, delay);
                _notificationList.AddItem(i, notificationDto);
                //_debugger.StartDebugging(_notificationHandler);
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
                WorkUUID = "abcdef" + i,
                Status = NotificationStatus.Pending,
                Title = "Title " + i,
                Text = "Text " + i,
                CreationTime = i,
                SchedulationTime = i * (60 * 1000)
            };
            _notificationList.AddItem(i, notificationDto);
            //_debugger.StartDebugging(_notificationHandler);
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
        _notificationList.SwitchItems(1, 4);
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
