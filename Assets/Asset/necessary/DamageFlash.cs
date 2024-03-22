using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true), SerializeField] private Color _color;
    [SerializeField] private float _flashTime = 0.25f;

    private SpriteRenderer[] _renderers;
    private Material[] _materials;
    private bool isFlashing = false;

    private float _timer;

    private int _flashamount = Shader.PropertyToID("_FlashAmount");
    private int _flashcolor = Shader.PropertyToID("_DamageColor");

    private void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new Material[_renderers.Length];   

        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i] = _renderers[i].material;
        }
    }

    private void Update()
    {
        if (isFlashing)
        {
            _timer += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(1f, 0f, (_timer / _flashTime));
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_flashamount, lerpedAmount);
            } 

            if(_timer > _flashTime)
            {
                isFlashing = false;
                _timer = 0f;
            }
        }
    }

    public void Flash()
    {
        isFlashing = true;
        _timer = 0;

        for(int i  = 0; i< _materials.Length; i++)
        {
            _materials[i].SetFloat(_flashamount, 1f);
            _materials[i].SetColor(_flashcolor, _color);
        }
    }

}
