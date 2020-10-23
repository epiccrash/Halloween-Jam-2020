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

    // TODO: if the TV is unplugged when this is set to true, then the enemy is alerted
    public bool isEvil;

    [Header("Appearance")]
    // when isOn is set to true, the TV will activate the onState GameObject and
    // deactivate the offState GameObject. And the reverse happens when isOn
    // set to false.
    public GameObject onState;
    public GameObject offState;

    bool _hasUnplugged = false;

    private void Start()
    {
        DisplayState();
    }

    // unplug the tv by going near to it and pressing the button
    private void OnTriggerStay(Collider other)
    {
        // can only interact with EVIl and GOOD tvs, DECOY and OFF are just decoration
        if (variant == TVVariant.EVIL || variant == TVVariant.GOOD)
        {
            // cannot interact with TVs during phase 1
            if (GameLogicController.Instance.phase != GameLogicController.GamePhase.PHASE_ONE)
            {
                if (Input.GetKeyDown(unplugKey))
                {
                    if (!_hasUnplugged)
                    {
                        Unplug();
                    }
                }
            }
        }
    }

    void Unplug()
    {
        _hasUnplugged = true;

        if (isEvil)
        {
            MemoryLogicController.Instance.UnplugEvil(this);
            Debug.Log("Unplug a an evil TV");
        }
        else
        {
            MemoryLogicController.Instance.UnplugGood(this);
            Debug.Log("Unplug a good TV");
        }

        variant = TVVariant.OFF;

        // TODO: play the unplug animation
        return;
    }

    // inInteractible must be set to true for On to have any effect.
    // the player will never actually turn on any TVs, but this method is
    // used to automatically display that the TV is turned on when
    // the isOn boolean is set to true
    void TurnOn()
    {
        if (variant != TVVariant.OFF)
        {
            if ((GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_ONE && !isEvil) ||
                GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_TWO)
            {
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
