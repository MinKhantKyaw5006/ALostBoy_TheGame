using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGroundSaver : MonoBehaviour
{
    [SerializeField] private float saveFrequency = 3f;

    public Vector2 SafeGroundLocation { get; private set; } = new Vector2(0f, 0f);

    private Coroutine safeGroundCoroutine;

    private void Start()
    {
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());

        //initialize starting a safe position
        SafeGroundLocation = transform.position;
    }
    private IEnumerator SaveGroundLocation()
    {
        float elapsedTime = 0f;

        while (true)
        {
            while (elapsedTime < saveFrequency)
            {
                elapsedTime += Time.deltaTime;
                yield return null; // Wait until the next frame
            }

            // Reset elapsedTime after reaching saveFrequency
            elapsedTime = 0f;

            //if playeris touching ground

            // Update SafeGroundLocation
            SafeGroundLocation = new Vector2(transform.position.x, transform.position.y);

            // No need to restart the coroutine due to the outer while loop
        }
    }


}
