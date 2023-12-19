using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;
    // Start is called before the first frame update

    private void Start()
    {
        if(transitionTo == GameManager.instance.transitionedFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;
            StartCoroutine(PlayerController.Instance.WalkintoNewScene(exitDirection, exitTime));
        }
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerController.Instance.pState.cutscene = true;
            SceneManager.LoadScene(transitionTo);
        }
    }
}
