using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerProgressHelper
{
    public static string SaveFilePath;

    static PlayerProgressHelper()
    {
        SaveFilePath = Application.persistentDataPath + "/bbpp";
    }

    public static T ReadPlayerProgress<T>() where T : new()
    {
        string ps = null;
        T progress;

        if (File.Exists(SaveFilePath))
        {
            StreamReader reader = new StreamReader(SaveFilePath);
            ps = Encryptor.DecryptBase64(reader.ReadToEnd());

            reader.Close();
        }

        if (string.IsNullOrEmpty(ps)) // create new playerStatus
        {
            progress = new T();

            SavePlayerProgress(progress);
        }
        else
        {
            progress = JsonUtility.FromJson<T>(ps);
#if UNITY_EDITOR
            string ps1 = JsonUtility.ToJson(progress);
            Debug.Log("PLAYER STATUS: " + ps1);
#endif
        }

        return progress;
    }

    public static void SavePlayerProgress<T>(T progress)
    {
        if (progress != null)
        {
            string ps = Encryptor.EncryptBase64(JsonUtility.ToJson(progress));
            //PlayerPrefs.SetString("PlayerStatus", ps);
            //PlayerPrefs.Save();

            StreamWriter writer = new StreamWriter(SaveFilePath, false);
            writer.Write(ps);
            writer.Close();
#if UNITY_EDITOR
            Debug.Log("PLAYER STATUS: " + ps);
#endif
        }
    }
}
