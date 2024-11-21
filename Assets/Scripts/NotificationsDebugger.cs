using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsDebugger : MonoBehaviour
{
    public int interval;
    private AndroidJavaObject _notificationHandler;

    public void StartDebugging(AndroidJavaObject notificationHandler)
    {
        _notificationHandler = notificationHandler;
        InvokeRepeating("StartPrint", interval, interval);
    }

    private void StartPrint()
    {
        var result = _notificationHandler.Call<string>("getNotificationAsString");
        Debug.Log(result);
    }
}
