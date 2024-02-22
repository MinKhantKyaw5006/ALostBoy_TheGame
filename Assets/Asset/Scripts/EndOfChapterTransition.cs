using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfChapterTransition : MonoBehaviour
{
    public string nextSceneName = "Chapter2"; // The name of the next scene

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Make sure the colliding object is the player
        {
            SceneManager.LoadScene(nextSceneName); // Load the next chapter
        }
    }
}
