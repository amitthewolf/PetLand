using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavingSystem
{
    
    public static void SaveProgress(List<Animal> Animals, int TAmount, DateTime TStamp)
    {
        Debug.Log("Saving datatime - "+TStamp);
        string path = Application.persistentDataPath + "/Petland.PL";
        SaveData data = new SaveData(Animals);
        string json = JsonUtility.ToJson(data);
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();
        PlayerPrefs.SetString("LastSave",TStamp.ToBinary().ToString());
        PlayerPrefs.SetInt("TrashAmount",TAmount);
        PlayerPrefs.Save();
    }
    
    public static void SaveAnimalProgress(List<Animal> Animals)
    {
        Debug.Log("Saving datatime - "+Time.time);
        string path = Application.persistentDataPath + "/Petland.PL";
        SaveData data = new SaveData(Animals);
        string json = JsonUtility.ToJson(data);
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();
        Debug.Log("Data Saved...");
    }

    public static SaveData LoadProgress()
    {
        string Filepath = Application.persistentDataPath + "/Petland.PL";
        if (File.Exists(Filepath))
        {
            StreamReader reader = new StreamReader(Filepath);
            string json = reader.ReadToEnd();
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Loading Progress...");
            return data;
        }
        else
        {
            Debug.Log("PET File not found");
            return null;
        }

        
    }
    
}
