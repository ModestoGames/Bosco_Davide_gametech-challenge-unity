using System.Collections;
using UnityEngine;
using UnityEngine.Android;

namespace com.modesto.notificationhandler
{
    public class PermissionsHandler : GameInitBehaviour
    {
        //utils property for other classes to get
        public bool CanRequestPermission => _numberOfPermissionRequests < _maxPermissionRequestsNumber;

        [SerializeField] private AppStateManager _appStateManager;

        [Header("Settings")]
        [SerializeField] private int _maxPermissionRequestsNumber;

        private bool _isGoingToSettings;

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (_isGoingToSettings)
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
            InitMaxPermissionRequestsNumber();
            CheckForPermissions();
            yield return null;
        }

        private void InitMaxPermissionRequestsNumber()
        {
            if (!PlayerPrefs.HasKey(Strings.PlayerPrefs.HowMayTimesPermissionAsked))
                _numberOfPermissionRequests = 0;
        }

        public void CheckForPermissions()
        {
#if UNITY_EDITOR
            _appStateManager.ChangeApplicationState(ApplicationState.Main);
#else

        if (Utils.GetSDKLevel() < 33)
        {
            _appStateManager.ChangeApplicationState(ApplicationState.Main);
            return;
        }

        if (Permission.HasUserAuthorizedPermission(Strings.Permissions.NotificationPermission))
        {
            _appStateManager.ChangeApplicationState(ApplicationState.Main);
            return;
        }

        if (!CanRequestPermission)
        {
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
            _appStateManager.ChangeApplicationState(ApplicationState.NoPermission);
        }

        private void PermissionCallbacks_PermissionGranted(string permission)
        {
            _appStateManager.ChangeApplicationState(ApplicationState.Main);
        }

        private void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permission)
        {
            _numberOfPermissionRequests = _maxPermissionRequestsNumber;
            _appStateManager.ChangeApplicationState(ApplicationState.NoPermission);
        }
    }
}