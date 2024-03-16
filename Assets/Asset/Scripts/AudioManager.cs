using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource themeSound;
    public AudioSource backgroundSound;

    void Start()
    {
        // Play theme sound loop
        themeSound.loop = true;
        themeSound.Play();

        // Play background sound loop
        backgroundSound.loop = true;
        backgroundSound.Play();
    }
}
