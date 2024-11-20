using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _body;
    [SerializeField] private Image _icon;
    [SerializeField] private Panel _panel;
    [SerializeField] private AudioSource _audioSource;

    public void ShowNotificationDetails(string title, string body, Sprite iconSprite)
    {
        _title.text = title;
        _body.text = body;
        _icon.sprite = iconSprite;
        _audioSource.Play();
        _panel.Show();
    }
}