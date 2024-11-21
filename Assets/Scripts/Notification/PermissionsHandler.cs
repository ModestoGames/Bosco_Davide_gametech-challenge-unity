using com.modesto.notificationhandler;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class PermissionsHandler : GameInitBehaviour
{
    //utils property for other classes to get
    public bool CanRequestPermission => _numberOfPermissionRequests < _maxPermissionRequestsNumber;

    [SerializeField] private AppStateManager _appStateManager;
    [SerializeField] private Panel _noPermissionPanel;

    [Header("Settings")]
    [SerializeField] private int _maxPermissionRequestsNumber;

    private bool _isGoingToSettings;

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            if(_isGoingToSettings)
            {
                _isGoingToSettings = false;
                AskForPermission();
            }
        }
    }

    //handle number of permissions request through playerprefs
    private int _numberOfPermissionRequests
    {
        get
        {
            return PlayerPrefs.GetInt(Strings.PlayerPrefs.HowMayTimesPermissionAsked);
        }

        set
        {
            PlayerPrefs.SetInt(Strings.PlayerPrefs.HowMayTimesPermissionAsked, value);
        }
    }

    public override IEnumerator Initialize()
    {
        Debug.Log("Permission handler initialize");
        InitMaxPermissionRequestsNumber();
        CheckForPermissions();
        yield return null;
    }

    private void InitMaxPermissionRequestsNumber()
    {
        if (!PlayerPrefs.HasKey(Strings.PlayerPrefs.HowMayTimesPermissionAsked))
        {
            _numberOfPermissionRequests = 0;
            Debug.Log("Number of permission request is " + _numberOfPermissionRequests);
        }
    }

    public void CheckForPermissions()
    {
        Debug.Log("Permission handler check for permissions");
#if UNITY_EDITOR
        _appStateManager.ChangeApplicationState(ApplicationState.Main);
#else

        if (Utils.GetSDKLevel() < 33)
        {
            Debug.Log("Permission handler no need to request permissions for sdk " + Utils.GetSDKLevel());
            _appStateManager.ChangeApplicationState(ApplicationState.Main);
            return;
        }

        if (Permission.HasUserAuthorizedPermission(Strings.Permissions.NotificationPermission))
        {
            Debug.Log("Permission handler already has permission");
            _appStateManager.ChangeApplicationState(ApplicationState.Main);
            return;
        }

        if (!CanRequestPermission)
        {
            Debug.Log("Permission handler cant request permissions");
            _appStateManager.ChangeApplicationState(ApplicationState.NoPermission);
            return;
        }

        ShowPreprompt();
#endif

    }

    public void ShowPreprompt()
    {
        _appStateManager.ChangeApplicationState(ApplicationState.PrePrompt);
    }

    public void AskForPermission()
    {
        _maxPermissionRequestsNumber++;
        Debug.Log($"Ask for permission for the {_maxPermissionRequestsNumber} time");
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
        Permission.RequestUserPermission(Strings.Permissions.NotificationPermission, callbacks);
    }

    public void GoToAppSettings()
    {
        _isGoingToSettings = true;
        using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        string packageName = currentActivityObject.Call<string>("getPackageName");
        using var uriClass = new AndroidJavaClass("android.net.Uri");
        using AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null);
        using var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject);
        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
        currentActivityObject.Call("startActivity", intentObject);
    }

    private void PermissionCallbacks_PermissionDenied(string permission)
    {
        Debug.Log("Permission callback - permission denied");
        _appStateManager.ChangeApplicationState(ApplicationState.NoPermission);

        if(_numberOfPermissionRequests == _maxPermissionRequestsNumber)
            _noPermissionPanel.ForceTurnOffSecondaryButton();
    }

    private void PermissionCallbacks_PermissionGranted(string permission)
    {
        Debug.Log("Permission callback - permission granted");
        _appStateManager.ChangeApplicationState(ApplicationState.Main);
    }

    private void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permission)
    {
        Debug.Log("Permission callback - permission denied and do not ask again");
        _numberOfPermissionRequests = _maxPermissionRequestsNumber;
        _appStateManager.ChangeApplicationState(ApplicationState.NoPermission);
    }
}
