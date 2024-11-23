using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.modesto.notificationhandler
{
    public class NotificationHandler : AppStateListener
    {
        [Header("Components")]
        [SerializeField] private NotificationDetail _notificationDetail;
        [SerializeField] private NotificationItemsHandler _notificationList;
        [Header("Settings")]
        [Range(1, 5)]
        [SerializeField] private int _notificationNumber = 5;
        [Range(1, 60)]
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
#if UNITY_ANDROID && !UNITY_EDITOR
            RefreshNotifications();
            CheckForNotificationData();
#endif
            }
        }

        //clear and refresh the current notification status
        public void RefreshNotifications()
        {
            var currentNotifications = _notificationHandler.Call<AndroidJavaObject>("getCurrentNotifications");
            int currentNotificationsNumber = currentNotifications.Call<int>("size");

            if (currentNotificationsNumber <= 0)
                return;

            _notificationList.Clear();

            List<NotificationDTO> notificationDtos = new List<NotificationDTO>();

            //use notification number instead of currentNotificationsNumber
            //because what you receive is a HashMap with <int, dto>
            for (int i = 1; i < _notificationNumber + 1; i++)
            {
                var key = new AndroidJavaObject("java.lang.Integer", i);
                var androidDto = currentNotifications.Call<AndroidJavaObject>("get", key);

                //add each android notification to a list
                if (androidDto != null)
                    notificationDtos.Add(new NotificationDTO(i, androidDto));
            }

            //reorder the list starting from the nearest scheduled
            notificationDtos = notificationDtos.OrderBy(n => n.DurationInSeconds).ToList();

            //rebuild the list already sorted
            foreach (NotificationDTO notificationDto in notificationDtos)
                _notificationList.AddItem(notificationDto);

            //restart list handler
            _notificationList.Initialize(_notificationHandler);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
                CheckForNotificationData();
        }

        public void ScheduleNotification()
        {
#if !UNITY_EDITOR
            _notificationList.Initialize(_notificationHandler);
            for (int i = 1; i <= _notificationNumber; i++)
            {
                int delay = i * _intervalSeconds;
                //the java method return the dto for the scheduled notification
                var notificationDto = _notificationHandler.Call<AndroidJavaObject>("scheduleNotification", i, delay);
                _notificationList.AddItem(i, notificationDto);
            }
#endif
        }

        //method called when opening application by tapping on notification
        private void CheckForNotificationData()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaObject intent = _currentActivity.Call<AndroidJavaObject>("getIntent"))
                {
                    if (intent.Call<bool>("hasExtra", "notification_id"))
                    {
                        //get intent data
                        int notificationId = intent.Call<int>("getIntExtra", "notification_id", -1);
                        string title = intent.Call<string>("getStringExtra", "notification_title");
                        string text = intent.Call<string>("getStringExtra", "notification_text");

                        HandleNotificationData(notificationId, text, title);

                        // dispose intent
                        intent.Call("removeExtra", "notification_id");
                        intent.Call("removeExtra", "notification_title");
                        intent.Call("removeExtra", "notification_text");
                    }
                }
            }
        }

        private void HandleNotificationData(int id, string text, string title)
        {
            try
            {
                Sprite sprite = Utils.GetSprite(_currentActivity, id);
                _notificationDetail.ShowNotificationDetails(title, text, sprite);
            }
            catch
            {
                _notificationDetail.ShowNotificationDetails(title, text, null);
            }
        }
    }
}