using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class SceneDataHandler 
{
    private string dataDirPath;
    private string sceneDataFileName;

    public SceneDataHandler(string dataDirPath, string sceneDataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.sceneDataFileName = sceneDataFileName;
    }

    public void SaveSceneData(SceneData sceneData)
    {
        string fullPath = Path.Combine(dataDirPath, sceneDataFileName);
        try
        {
            string json = JsonUtility.ToJson(sceneData, true);
            File.WriteAllText(fullPath, json);
            Debug.Log($"Scene data saved to {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save scene data to {fullPath}: {e.Message}");
        }
    }

    public SceneData LoadSceneData()
    {
        string fullPath = Path.Combine(dataDirPath, sceneDataFileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string json = File.ReadAllText(fullPath);
                SceneData sceneData = JsonUtility.FromJson<SceneData>(json);
                return sceneData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load scene data from {fullPath}: {e.Message}");
                return new SceneData(); // Return new instance if loading fails
            }
        }
        else
        {
            Debug.LogWarning($"No scene file found at {fullPath}, returning new instance.");
            return new SceneData();
        }
    }

    public bool SceneDataExists()
    {
        string fullPath = Path.Combine(dataDirPath, sceneDataFileName);
        Debug.Log("SceneData already exist");
        return File.Exists(fullPath);
        
    }

}
