/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public Vector2 playerPosition;
    public GameData()
    {
        playerPosition = Vector2.zero; // Default player position
    }
}

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector2 playerPosition;
    public long lastUpdated; //timestamp storage
    public bool shouldReset = false;

    public float health;
    public float mana = 0; // Default mana value
    public List<int> checkpointIDs = new List<int>();// IDs of checkpoints the player has activated
    



    public GameData()
    {
        playerPosition = Vector2.zero; // Default player position
        health = 0f;
        
    }
}



