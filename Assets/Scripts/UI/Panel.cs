using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [Header("Graphic")]
    [SerializeField] private UIItem _uiItem;
    [Header("Secondary Button")]
    [SerializeField] private bool _hasSecondaryButton;
    [SerializeField] private GameObject _secondaryButton;
    [Header("Events")]
    [SerializeField] private UnityEvent _onMainButtonClick;
    [SerializeField] private UnityEvent _onSecondaryButtonClick;

    private void OnEnable()
    {
        _secondaryButton.SetActive(_hasSecondaryButton);
    }

    public void ForceTurnOffSecondaryButton()
    {
        _hasSecondaryButton = false;
    }

    public void MainButtonClicked()
    {
        _onMainButtonClick?.Invoke();
    }

    public void SecondaryButtonClicked()
    {
        _onSecondaryButtonClick?.Invoke();
    }

    public void Show()
    {
        _uiItem.Show();
    }

    public void Hide()
    {
        _uiItem.Hide();
    }
}
