using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is attached to TV gameobjects 
public class TV : MonoBehaviour
{
    public KeyCode unplugKey;

    // Evil TVS are off during the day and on at night
    // Good TVs are on during the day and on at night
    // Decoy TVs are on during the day and off at night
    // Off TVs are always off
    public enum TVVariant
    {
        EVIL,
        GOOD,
        DECOY,
        OFF
    }

    public TVVariant variant;

    // TODO: if the TV is unplugged the enemy is alerted

    [Header("Appearance")]
    // when isOn is set to true, the TV will activate the onState GameObject and
    // deactivate the offState GameObject. And the reverse happens when isOn
    // set to false.
    public GameObject onState;
    public GameObject offState;
    public GameObject evilState; // activated when evil tv unplugged

	[Header("Sound")]
	public AudioSource audioSource;
	public AudioClip turnOffClip;

    bool _hasUnplugged = false;



    private void Start() {
		DisplayState();
    }

	// unplug the tv by going near to it and pressing the button
	private void OnTriggerStay(Collider other)
    {
		// Only valid to interact with if you're colliding with the PLAYER (not Monster or other collider)
		if (!other.CompareTag("Player")) {
			return;
		}

		// can only interact with EVIl and GOOD tvs, DECOY and OFF are just decoration
		if (variant == TVVariant.EVIL || variant == TVVariant.GOOD)
        {
            // cannot interact with TVs during phase 1
            if (GameLogicController.Instance.phase != GameLogicController.GamePhase.PHASE_ONE) {
				if (Input.GetKey(unplugKey)) {
					if (!_hasUnplugged) {
						Unplug();
                    }
                }
            }
        }
    }

    void Unplug() {
		_hasUnplugged = true;
		// Play sound effect
		audioSource.PlayOneShot(turnOffClip);

        if (GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_ONE)
        {
            if (Evil())
            {
                MemoryLogicController.Instance.UnplugEvil(this);
            }
            else
            {
                MemoryLogicController.Instance.UnplugGood(this);
            }
            offState.SetActive(true);
            onState.SetActive(false);
        } else
        {
            if (Evil())
            {
                //Debug.Log("Unplug during phase 2");
                evilState.SetActive(true);
                MemoryLogicController.Instance.UnplugEvil(this);
            }
            else
            {
                MemoryLogicController.Instance.UnplugGood(this);
                offState.SetActive(true);
            }
            onState.SetActive(false);

            variant = TVVariant.OFF;
        }

        // TODO: play the unplug animation
        return;
    }

    // the player will never actually turn on any TVs, but this method is
    // used to automatically display that the TV is turned on when
    // the isOn boolean is set to true
    void TurnOn()
    {
        if (variant != TVVariant.OFF)
        {
            if ((GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_ONE && !Evil()) ||
                GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_TWO)
            {
				_hasUnplugged = false;
                onState.SetActive(true);
                offState.SetActive(false);
            }
        }
    }

    public bool Evil()
    {
        return variant == TVVariant.EVIL;
    }


    // displays the current state of the tv depending on GameLogicController.Instance.phase
    // and whether the tv is evil. Evil tvs do not turn on during phase 1 but
    // they do during phase 2. Good tvs are always on
    public void DisplayState()
    {
        if (GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_ONE)
        {
            if (variant == TVVariant.GOOD || variant == TVVariant.DECOY)
                TurnOn();
            else
                Unplug();
        } else
        {
            if (variant == TVVariant.GOOD || variant == TVVariant.EVIL)
            {
                TurnOn();
            } else
            {
                Unplug();
            }
        }
    }
   
}
