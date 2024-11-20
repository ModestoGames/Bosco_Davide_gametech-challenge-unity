using UnityEngine;
using UnityEngine.UI;

namespace com.modesto.notificationhandler
{
    public class BackroundScroller : MonoBehaviour
    {
        [SerializeField] private RawImage _image;
        [SerializeField] private Vector2 _speed;

        private void OnApplicationFocus(bool focus)
        {
            //reset position each time is on focus
            if (focus)
                _image.uvRect = new Rect(Vector2.zero, _image.uvRect.size);
        }

        private void Update()
        {
            var position = _image.uvRect.position + _speed * Time.deltaTime;
            _image.uvRect = new Rect(position, _image.uvRect.size);
        }
    }
}