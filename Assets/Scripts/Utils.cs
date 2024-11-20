using UnityEngine;

public static class Utils
    
{
    public static int GetSDKLevel()
    {
        var clazz = AndroidJNI.FindClass("android/os/Build$VERSION");
        var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
        var sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
        return sdkLevel;
    }

    public static Sprite GetSprite(AndroidJavaObject currentActivity, string packageName, string resourceName)
    {
        AndroidJavaObject resources = currentActivity.Call<AndroidJavaObject>("getResources");
        int resourceId = resources.Call<int>("getIdentifier", resourceName, "drawable", packageName);

        if (resourceId != 0)
        {
            Debug.Log("Risorsa trovata con ID: " + resourceId);
        }
        else
        {
            Debug.LogError("Risorsa non trovata");
            return null;
        }

        AndroidJavaObject bitmap = null;
        try
        {
            // Carica il bitmap
            AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory");
            bitmap = bitmapFactory.CallStatic<AndroidJavaObject>("decodeResource", resources, resourceId);

            // Ottieni le dimensioni del bitmap
            int width = bitmap.Call<int>("getWidth");
            int height = bitmap.Call<int>("getHeight");

            // Crea array di byte per i pixel
            int[] pixels = new int[width * height];
            bitmap.Call("getPixels", pixels, 0, width, 0, 0, width, height);

            // Crea texture Unity
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color32[] colors = new Color32[width * height];

            // Converti i pixel Android ARGB in formato Unity RGBA
            for (int i = 0; i < pixels.Length; i++)
            {
                int pixel = pixels[i];
                byte a = (byte)((pixel >> 24) & 0xff);
                byte r = (byte)((pixel >> 16) & 0xff);
                byte g = (byte)((pixel >> 8) & 0xff);
                byte b = (byte)(pixel & 0xff);
                colors[i] = new Color32(r, g, b, a);
            }

            texture.SetPixels32(colors);
            texture.Apply();

            Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            Sprite sprite = Sprite.Create(texture, rect, pivot);
            return sprite;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore nella conversione dell'immagine: " + e.Message);
            return null;
        }
        finally
        {
            if (bitmap != null)
            {
                bitmap.Call("recycle");
            }
        }
    }
}
