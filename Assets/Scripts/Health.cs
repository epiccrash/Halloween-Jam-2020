using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;

    public GameObject damageScreen;
    Animator _damageScreenAnimator;
    Animator _playerAnimator;

    private void Start()
    {
        _playerAnimator = gameObject.GetComponent<Animator>();
        _damageScreenAnimator = damageScreen.GetComponent<Animator>();

        StartCoroutine("HealPeriodically");
    }


    IEnumerator HealPeriodically()
    {
        int healAmt = 25;
        int secondsInBetweenHealing = 5;
        while (health > 0)
        {
            health = health + healAmt > 100 ? 100 : health + healAmt;
            yield return new WaitForSeconds(secondsInBetweenHealing);
        }
    }

    public void Damage(int amt)
    {
        health -= amt;
        if (health > 0)
        {
            // TODO play the damage sound
            // TODO play the damage animation
            _damageScreenAnimator.SetTrigger("Damage");
        } else 
        {
            Die();
        }
    }

    void Die()
    {
        // TODO play the death animation
        // TODO show the death screen
    }

}
