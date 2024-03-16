using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAttackEnemy : Enemy
{
    [SerializeField] private float chaseDistance;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle);

    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (currentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                if(_dist> chaseDistance)
                {
                    ChangeState(EnemyStates.Bat_Chase);
                }
                break;

            case EnemyStates.Bat_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                FlipBat();
                break;

            case EnemyStates.Bat_Stunned:
                break;

            case EnemyStates.Bat_Death:
                break;
        }
    }

    void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
}
