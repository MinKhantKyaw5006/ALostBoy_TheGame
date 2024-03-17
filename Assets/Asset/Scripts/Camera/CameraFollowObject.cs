using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransforms;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private PlayerController _player;

    private Coroutine _turnCoroutine;

    private bool _isFacingRight;

    private void Awake()
    {
        _player = _playerTransforms.gameObject.GetComponent<PlayerController>();

    }

    private void Update()
    {
        //make the camerafollowobject follow the player's position
        transform.position = _playerTransforms.position;


    }

    public void CallTurn()
    {


        //LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
        //_turnCoroutine = StartCoroutine(FlipYLerp());


    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;

        while(elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;
            //lerp the y rotation
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }

       
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
