using UnityEngine;
using System.IO;
using System.Collections.Generic;

using System.Linq; // Add this line

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/savefile.json";

    public static void SaveGame()
    {
        var state = new Dictionary<string, object>();
        foreach (var saveable in Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        string json = JsonUtility.ToJson(state);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadGame()
    {
        if (!File.Exists(SavePath)) return;

        string json = File.ReadAllText(SavePath);
        var state = JsonUtility.FromJson<Dictionary<string, object>>(json);

        foreach (var saveable in Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();
            if (state.TryGetValue(typeName, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }
}
