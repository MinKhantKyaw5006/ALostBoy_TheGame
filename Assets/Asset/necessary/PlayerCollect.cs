using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to use TextMeshPro

// Define an enum for collectible types
public enum CollectibleType
{
    Gem,
    Jade,
    Stone,
    Diamond
}

public class PlayerCollect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gemCountText;
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private TextMeshProUGUI stoneCountText;
    [SerializeField] private TextMeshProUGUI diamondCountText;
    [SerializeField] private AudioSource collectSound;


    private int gemCount = 0;
    private int jadeCount = 0;
    private int stoneCount = 0;
    private int diamondCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CollectibleItem"))
        {
            // Assume the collectible item has a component that specifies its type
            CollectibleItem collectibleItem = collision.gameObject.GetComponent<CollectibleItem>();
            if (collectibleItem != null)
            {
                switch (collectibleItem.itemType)
                {
                    case CollectibleType.Gem:
                        gemCount++;
                        gemCountText.text = gemCount.ToString();
                        break;
                    case CollectibleType.Jade:
                        jadeCount++;
                        keyCountText.text = jadeCount.ToString();
                        break;
                    case CollectibleType.Stone:
                        stoneCount++;
                        stoneCountText.text = stoneCount.ToString();
                        break;
                    case CollectibleType.Diamond:
                        diamondCount++;
                        diamondCountText.text = diamondCount.ToString();
                        break;
                }

                collectSound.Play();
                Destroy(collision.gameObject); // Destroy the collectible item after collecting
            }
        }
    }
    public bool HasItems(CollectibleType itemType, int count)
    {
        switch (itemType)
        {
            case CollectibleType.Gem:
                return gemCount >= count;
            case CollectibleType.Jade:
                return jadeCount >= count;
            case CollectibleType.Stone:
                return stoneCount >= count;
            case CollectibleType.Diamond:
                return diamondCount >= count;
            default:
                return false;   
        }
    }

    public void UseItems(CollectibleType itemType, int count)
    {
        switch (itemType)
        {
            case CollectibleType.Gem:
                gemCount = Mathf.Max(0, gemCount - count);
                gemCountText.text = gemCount.ToString();
                break;
            case CollectibleType.Jade:
                jadeCount = Mathf.Max(0, jadeCount - count);
                keyCountText.text = jadeCount.ToString();
                break;
            case CollectibleType.Stone:
                stoneCount = Mathf.Max(0, stoneCount - count);
                stoneCountText.text = stoneCount.ToString();
                break;
            case CollectibleType.Diamond:
                diamondCount = Mathf.Max(0, diamondCount - count);
                diamondCountText.text = diamondCount.ToString();
                break;
        }
    }

    public int GetItemCount(CollectibleType itemType)
    {
        switch (itemType)
        {
            case CollectibleType.Gem:
                return gemCount;
            case CollectibleType.Jade:
                return jadeCount;
            case CollectibleType.Stone:
                return stoneCount;
            case CollectibleType.Diamond:
                return diamondCount;
            default:
                Debug.LogError("Unknown item type.");
                return 0;
        }
    }


}

