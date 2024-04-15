using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{
    public static void SaveDataToJson(IEnumerable<ISaveable> saveables, string saveFileName = "saveFile.json") {
        SaveData saveData = new SaveData();
        foreach (ISaveable saveable in saveables) {
            saveable.PopulateSaveData(saveData);
        }

        if (WriteToFile(saveFileName, JsonUtility.ToJson(saveData))) {
            Debug.Log("Successfully Saved Data!");
        }
    }

    public static void LoadFromJson(IEnumerable<ISaveable> saveables, string saveFileName = "saveFile.json") {
        var fullPath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (LoadFromFile(saveFileName, out var loadedContent)) {
            SaveData saveData = new SaveData();
            JsonUtility.FromJsonOverwrite(loadedContent, saveData);

            foreach (ISaveable saveable in saveables) {
                saveable.LoadFromSaveData(saveData);
            }

            Debug.Log("Successfully Loaded!");
        }
    }

    public static bool WriteToFile(string fileName, string fileContents) {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);
        
        try {
            File.WriteAllText(fullPath, fileContents);
            return true;
        }catch (Exception e) {
            Debug.LogError($"Failed to write to file {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string filename, out string loadContents) {
        var fullPath = Path.Combine(Application.persistentDataPath, filename);

        try {
            loadContents = File.ReadAllText(fullPath);
            return true;
        }catch (Exception e) {
            Debug.LogError($"Failed to load from file {fullPath} with exception {e}");
            loadContents = null;
            return false;
        }
    }
}
