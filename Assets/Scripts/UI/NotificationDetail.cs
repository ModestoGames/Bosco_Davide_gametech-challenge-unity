using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Handle notification details by showing them in a popup panel
    /// </summary>
    public class NotificationDetail : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _body;
        [SerializeField] private Image _icon;
        [SerializeField] private Panel _panel;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _clip;

        public void ShowNotificationDetails(string title, string body, Sprite iconSprite)
        {
            _title.text = title;
            _body.text = body;
            _icon.sprite = iconSprite;
            _audioSource.PlayOneShot(_clip);
            _panel.Show();
        }
    }
}