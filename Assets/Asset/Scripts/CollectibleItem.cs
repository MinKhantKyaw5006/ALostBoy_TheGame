/*using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // Define the enum inside the CollectibleItem script
    public enum CollectibleType
    {
        Gem,
        Key,
        Stone,
        Diamond
    }

    public CollectibleType itemType; // Use this to differentiate between item types
}
*/
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public CollectibleType itemType; // Make sure this matches the enum name in your PlayerCollect script
}
