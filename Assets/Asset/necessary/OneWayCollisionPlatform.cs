using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OneWayCollisionPlatform : MonoBehaviour
{
    private PlatformEffector2D effector2D;
    private float waitTime;


    // Start is called before the first frame update
    private void Start()
    {
        waitTime = 3f; 
        effector2D = GetComponent<PlatformEffector2D>();   
    }

    // Update is called once per frame
    private void Update()
    {
        OneWayCollisionController(); 
    }

    private void OneWayCollisionController()
    {
        if(PlayerController.Instance.pState.jumping == true)
        {
            effector2D.rotationalOffset = 0f;
        }

        waitTime -= Time.deltaTime;
        if (waitTime < 0f)
        {
            effector2D.rotationalOffset = 180f;
            waitTime = 3f;  
        }
    }
}
