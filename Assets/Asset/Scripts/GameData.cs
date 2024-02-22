using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class GameData
{
    // Your existing fields
    public Vector2 playerPosition;
    public string currentSceneName;
    public int lastCheckpointID = -1; // Default to -1 indicating no checkpoint has been reached
    public Vector2 lastCheckpointPos; // Position of the last checkpoint reached

    public GameData()
    {
        // Initialize default values if needed
        playerPosition = Vector2.zero; // Default player position
        currentSceneName = "Chapter1"; // Default to the first game scene
        lastCheckpointPos = Vector2.zero; // Default checkpoint position

    }
}

