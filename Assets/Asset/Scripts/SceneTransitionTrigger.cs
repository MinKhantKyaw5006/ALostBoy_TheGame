using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // The name of the scene you want to load
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Make sure your player GameObject has the "Player" tag
        {

            Debug.Log("player detected");
           
            StartCoroutine(WaitAndLoadScene(sceneToLoad));
        }
    }

    private IEnumerator WaitAndLoadScene(string sceneName)
    {
    

        // Wait for 3 seconds
        yield return new WaitForSeconds(3);

        // Proceed to load the new scene, expecting DataPersistenceManager to handle the reset
        SceneManager.LoadScene(sceneName);
    }



}
