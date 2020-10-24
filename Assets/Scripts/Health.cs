using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;

    public GameObject damageScreen;
	public AudioSource damageAudioSource;
	public AudioClip damageClip;
	public AudioClip damageClip2;

	Animator _damageScreenAnimator;
    Animator _playerAnimator;


    bool _hasDied = false;
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
        if (!_hasDied)
        {
            health -= amt;
            if (health > 0)
            {
				damageAudioSource.PlayOneShot(damageClip);
				damageAudioSource.PlayOneShot(damageClip2);
                // TODO play the damage animation
                _damageScreenAnimator.SetTrigger("Damage");
            } else 
            {
                Die();
            }
        }
    }

    void Die() {
		// For now, also trigger the damage visual overlay
		_damageScreenAnimator.SetTrigger("Damage");
		_hasDied = true;
        GameLogicController.Instance.Lose();
    }

}
