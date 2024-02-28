using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure to use TextMeshPro

// Define an enum for collectible types
public enum CollectibleType
{
    Gem,
    Key,
    Stone,
    Diamond
}

public class PlayerCollect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gemCountText;
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private TextMeshProUGUI stoneCountText;
    [SerializeField] private TextMeshProUGUI diamondCountText;

    private int gemCount = 0;
    private int keyCount = 0;
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
                    case CollectibleType.Key:
                        keyCount++;
                        keyCountText.text = keyCount.ToString();
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

                Destroy(collision.gameObject); // Destroy the collectible item after collecting
            }
        }
    }
}

