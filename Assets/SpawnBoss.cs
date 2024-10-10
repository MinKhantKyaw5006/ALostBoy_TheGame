using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public static SpawnBoss Instance;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject boss;
    [SerializeField] Vector2 exitDirection;
    bool callOnce;
    BoxCollider2D col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!callOnce)
            {
                StartCoroutine(WalkIntoRoom());
                callOnce = true;
            }
        }
    }

    public void IsNotTrigger()
    {
        col.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WalkIntoRoom()
    {
        // StartCoroutine(PlayerController.Instance.walkintonewscene(exitDirection, 1));
        yield return new WaitForSeconds(1f);
        col.isTrigger = false;
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }
}
