using com.modesto.notificationhandler;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class NotificationHandler : AppStateListener
{
    [SerializeField] private TextMeshProUGUI _text;
    [Range(1, 5)]
    [SerializeField] private int _notificationNumber = 5;
    [Range(1, 10)]
    [SerializeField] private int _interval = 1;

    private AndroidJavaObject _currentActivity;

    protected override IEnumerator OnInitialize()
    {
#if !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
#endif
        yield return null;
    }

    public override void AppStateChanged(ApplicationState previousState, ApplicationState currentState)
    {
        if (currentState == ApplicationState.Main)
            CheckForNotificationData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // L'app torna in foreground
        {
            CheckForNotificationData();
        }
    }

    public void Toast()
    {
        using (var notificationHelper = new AndroidJavaObject("com.modesto.notification_module.LocalNotificationsService", _currentActivity))
        {
            notificationHelper.Call("toast", "Test");
        }
    }

    public void ScheduleNotification()
    {
        Debug.Log("Try schedule notification");
        try
        {
            //var notificationHelper = new AndroidJavaObject("com.modesto.notification_module.LocalNotificationsService", _currentActivity);

            for (int i = 1; i <= _notificationNumber; i++)
            {
                using (var notificationHelper = new AndroidJavaObject("com.modesto.notification_module.LocalNotificationsService", _currentActivity))
                {
                    int delay = i * _interval;
                    Debug.Log("id and delay " + i + " " + delay);
                    notificationHelper.Call("scheduleNotification", i, delay);
                }
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.InnerException);
            Debug.Log(e.StackTrace);
        }
    }

    private void CheckForNotificationData()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent"))
            {
                if (intent.Call<bool>("hasExtra", "notification_id"))
                {
                    // Leggi i dati dall'intent
                    int notificationId = intent.Call<int>("getIntExtra", "notification_id", -1);
                    string title = intent.Call<string>("getStringExtra", "notification_title");
                    string text = intent.Call<string>("getStringExtra", "notification_text");
                    string unityMessage = intent.Call<string>("getStringExtra", "unity_message");
                    long timestamp = intent.Call<long>("getLongExtra", "notification_timestamp", 0L);

                    // Gestisci i dati della notifica
                    HandleNotificationData(notificationId, title, text, unityMessage, timestamp);

                    // Pulisci l'intent per evitare di processare pi� volte gli stessi dati
                    intent.Call("removeExtra", "notification_id");
                    intent.Call("removeExtra", "notification_title");
                    intent.Call("removeExtra", "notification_text");
                    intent.Call("removeExtra", "unity_message");
                    intent.Call("removeExtra", "notification_timestamp");
                }
            }
        }
    }

    private void HandleNotificationData(int id, string title, string text, string unityMessage, long timestamp)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"Notifica ricevuta - ID: {id}");
        sb.Append($"Titolo: {title}");
        sb.Append($"Testo: {text}");
        sb.Append($"Messaggio Unity: {unityMessage}");
        sb.Append($"Timestamp: {timestamp}");

        _text.text = sb.ToString();
    }

}
