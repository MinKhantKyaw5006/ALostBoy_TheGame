using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class SaveSlot : MonoBehaviour
{
    [Header("ProfileId")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    [SerializeField] private TextMeshProUGUI percentageCompleteText;
    [SerializeField] private TextMeshProUGUI levelDisplay;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    //original
    /*
    public void SetData(GameData data)
    {
        //there's no data for this profileId
        if(data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }

        //there is data for this profileId
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            percentageCompleteText.text = "game progess percent";
            levelDisplay.text = "Level 1";
        }
    }
    */

    //original working code
    
    public void SetData(GameData data, string profileId)
    {
        // Check if there's no data for this profileId
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            // Load the save slot data for this profile
            SaveSlotData saveSlotData = DataPersistenceManager.instance.LoadSaveSlotData(profileId);
            if (saveSlotData != null && !string.IsNullOrEmpty(saveSlotData.lastPlayedScene))
            {
                // Directly use the scene name as the level display
                levelDisplay.text = saveSlotData.lastPlayedScene;
            }
            else
            {
                // Use a default text or placeholder if no specific scene data found
                levelDisplay.text = "Start Your Adventure!"; // Or "Level 1" if you prefer
            }

            // Update the game progress percentage text
            percentageCompleteText.text = "game progress percent"; // Update this based on your game's logic
        }
    }
    







    public string GetProfileId()
    {
        return this.profileId;
    }

    public void Setinteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
}
