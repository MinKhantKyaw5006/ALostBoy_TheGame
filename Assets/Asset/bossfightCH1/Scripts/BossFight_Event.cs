using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFight_Event : MonoBehaviour
{

    private void SlashDamagePlayer()
    {
        if (PlayerController.Instance.transform.position.x - transform.position.x != 0)
        {
            Hit(BossFight.Instance.sideAttack, BossFight.Instance.sideAttackArea);
            Debug.Log("Side Attack!!!");
        }

        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            Hit(BossFight.Instance.upAttack, BossFight.Instance.upAttackArea);
            Debug.Log("Top Attack!!!");

        }

        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            Hit(BossFight.Instance.downAttack, BossFight.Instance.downAttackArea);
            Debug.Log("Down Attack!!!");
        }
    }
    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D _objectsToHit = Physics2D.OverlapBox(_attackTransform.position, _attackArea, 0);
        if (_objectsToHit.GetComponent<PlayerController>() != null)
        {
            _objectsToHit.GetComponent<PlayerController>().TakeDamage(BossFight.Instance.damage);
        }
    }

    void Parrying()
    {
        BossFight.Instance.parrying = true;
    }

    private void BendDownCheck()
    {
        if (BossFight.Instance.barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }

        if (BossFight.Instance.outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTransition());
        }

        if (BossFight.Instance.bounceAttack)
        {
            BossFight.Instance.anim.SetTrigger("Bounce1");
        }
    }

    private void BarrageorOutbreak()
    {
        if (BossFight.Instance.barrageAttack)
        {
            BossFight.Instance.StartCoroutine(BossFight.Instance.Barrage());
        }

        if (BossFight.Instance.outbreakAttack)
        {
            BossFight.Instance.StartCoroutine(BossFight.Instance.Outbreak());
        }
    }
    private IEnumerator BarrageAttackTransition()
    {

        yield return new WaitForSeconds(1f);
        BossFight.Instance.anim.SetBool("Cast", true);
    }

    private IEnumerator OutbreakAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        BossFight.Instance.anim.SetBool("Cast", true);
    }
    
    private void DestroyAfterDeath()
    {
        EnemySpawner.Instance.IsNotTrigger();
        BossFight.Instance.DestroyAfterDeath(); 
    }
}
