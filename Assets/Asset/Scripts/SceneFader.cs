using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private float fadeTime;

    private Image fadeoutUIImage;
    public enum FadeDirection 
    {
        In,
        Out
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeoutUIImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDirection == FadeDirection.Out ? 0 : 1;

        if(_fadeDirection == FadeDirection.Out)
        {
            while(_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }

            fadeoutUIImage.enabled = false;
        }
        else
        {
            fadeoutUIImage.enabled = true;

            while(_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);
                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _levelToLoad)
    {
        fadeoutUIImage.enabled = true;
        yield return Fade(_fadeDirection);
        SceneManager.LoadScene(_levelToLoad);
    }
    
    void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeoutUIImage.color = new Color(fadeoutUIImage.color.r, fadeoutUIImage.color.g, fadeoutUIImage.color.b, _alpha);

        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}
