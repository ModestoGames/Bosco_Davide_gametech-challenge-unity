using System.Collections.Generic;
using UnityEngine;

namespace com.modesto.notificationhandler
{
    public static class Utils

    {
        public static int GetSDKLevel()
        {
            var clazz = AndroidJNI.FindClass("android/os/Build$VERSION");
            var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
            var sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
            return sdkLevel;
        }

        public static Sprite GetSprite(AndroidJavaObject currentActivity, int id)
        {
            //get drawable bitmap data
            var androidUtils = new AndroidJavaClass("com.modesto.notification_module.Utils");
            var size = androidUtils.CallStatic<int>("getBitmapSize", currentActivity, id);
            var pixels = androidUtils.CallStatic<int[]>("getIconPixel", currentActivity, id);

            try
            {
                Texture2D texture = new Texture2D(size, size);
                Color32[] colors = new Color32[size * size];

                //// Convert Android ARGB pixels in Unity RGBA
                for (int i = 0; i < pixels.Length; i++)
                {
                    int pixel = pixels[i];
                    byte a = (byte)(pixel >> 24 & 0xff);
                    byte r = (byte)(pixel >> 16 & 0xff);
                    byte g = (byte)(pixel >> 8 & 0xff);
                    byte b = (byte)(pixel & 0xff);
                    colors[i] = new Color32(r, g, b, a);
                }

                //create texture
                texture.SetPixels32(colors);
                texture.Apply();

                //create sprite from texture
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(texture, rect, pivot);

                //dispose
                pixels = null;
                colors = null;
                texture = null;

                return sprite;
            }
            catch
            {
                return null;
            }
        }

        //check if two lists of integer has the same values at each index
        public static bool ListsSortingIsEqual(List<int> list1, List<int> list2)
        {
            bool result = true;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}