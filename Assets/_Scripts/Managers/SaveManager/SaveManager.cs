using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        LoadSave();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) SaveGame();
        if (Input.GetKeyDown(KeyCode.P)) LoadSave();
    }

    private void SaveGame() {
        List<ISaveable> saveables = new List<ISaveable>();
        saveables.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>());

        SaveDataManager.SaveDataToJson(saveables);
    }

    private void LoadSave() {
        List<ISaveable> saveables = new List<ISaveable>();
        saveables.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>());

        SaveDataManager.LoadFromJson(saveables);
    }
}
