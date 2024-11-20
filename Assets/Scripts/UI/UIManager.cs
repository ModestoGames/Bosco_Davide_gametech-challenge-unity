using com.modesto.notificationhandler;
using System.Collections;
using UnityEngine;

public class UIManager : AppStateListener
{
    [Header("General")]
    [SerializeField] private PermissionsHandler _permissionHandler;

    [Header("Panels")]
    [SerializeField] private Panel _permissionPrepromptPanel;
    [SerializeField] private Panel _noPermissionPanel;
    [SerializeField] private UIItem _mainPanel;

    protected override IEnumerator OnInitialize()
    {
        _noPermissionPanel.Hide();
        _permissionPrepromptPanel.Hide();
        _mainPanel.Hide();
        yield return null;
    }

    public override void AppStateChanged(ApplicationState previousState, ApplicationState currentState)
    {
        HandleChangeState(previousState, currentState);
    }

    private void HandleChangeState(ApplicationState previousState, ApplicationState currentState)
    {
        switch (currentState)
        {
            case ApplicationState.PrePrompt:
                _noPermissionPanel.Hide();
                _permissionPrepromptPanel.Show();
                _mainPanel.Hide();
                break;
            case ApplicationState.NoPermission:

                if(!_permissionHandler.CanRequestPermission)
                    _noPermissionPanel.ForceTurnOffSecondaryButton();

                _permissionPrepromptPanel.Hide();
                _noPermissionPanel.Show();
                _mainPanel.Hide();
                break;
            case ApplicationState.Main:
                _permissionPrepromptPanel.Hide();
                _noPermissionPanel.Hide();
                _mainPanel.Show();
                break;
        }

    }
}