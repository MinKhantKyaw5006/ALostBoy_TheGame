using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private float _shakeForce = 1f;
    private CinemachineImpulseSource _impulseSource;
    // Start is called before the first frame update
    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(Vector2 direction)
    {
        _impulseSource.GenerateImpulseWithVelocity(-direction * _shakeForce);
    }
}
