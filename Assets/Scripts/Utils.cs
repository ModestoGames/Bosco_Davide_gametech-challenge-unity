using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    public static Sprite GetSprite(AndroidJavaObject currentActivity, int id)
    {
        var androidUtils = new AndroidJavaClass("com.modesto.notification_module.Utils");
        var size = androidUtils.CallStatic<int>("getBitmapSize", currentActivity, id);
        var pixels = androidUtils.CallStatic<int[]>("getIconPixel", currentActivity, id);

        try
        {
            Texture2D texture = new Texture2D(size, size);
            Color32[] colors = new Color32[size * size];

            //// Converti i pixel Android ARGB in formato Unity RGBA
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

            Debug.Log($"Texture caricata. Dimensioni: {texture.width}x{texture.height}");

            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            Debug.Log("Crea sprite");

            Sprite sprite = Sprite.Create(texture, rect, pivot);

            pixels = null;
            colors = null;

            return sprite;

        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore nella conversione dell'immagine: " + e.Message);
            return null;
        }
    }

    static void PrintArrayValues(int[] array)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Array values: ");

        for (int i = 0; i < array.Length; i++)
        {
            sb.Append(array[i]);

            // Aggiunge una virgola tra i valori, tranne che per l'ultimo
            if (i < array.Length - 1)
            {
                sb.Append(", ");
            }
        }

        // Stampa il risultato finale
        Debug.Log(sb.ToString());
    }

    static void PrintArrayValues(byte[] array)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Array values: ");

        for (int i = 0; i < array.Length; i++)
        {
            sb.Append(array[i]);

            // Aggiunge una virgola tra i valori, tranne che per l'ultimo
            if (i < array.Length - 1)
            {
                sb.Append(", ");
            }
        }

        // Stampa il risultato finale
        Debug.Log(sb.ToString());
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