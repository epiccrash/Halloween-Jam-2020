using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is attached to TV gameobjects 
public class TV : MonoBehaviour
{
    public KeyCode unplugKey;

    // TVs that are not interactible are simply decoys and not interacted with
    public bool isInteractible;

    public bool On;

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
        // interactible tvs are in-play, non-interactible tvs are decoys
        if (isInteractible)
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
        On = false;
        // TODO: play the unplug animation

        onState.SetActive(false);
        offState.SetActive(true);

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
        return;
    }

    // inInteractible must be set to true for On to have any effect.
    // the player will never actually turn on any TVs, but this method is
    // used to automatically display that the TV is turned on when
    // the isOn boolean is set to true
    void TurnOn()
    {
        if (isInteractible)
        {
            On = true;
            if ((GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_ONE && !isEvil) ||
                GameLogicController.Instance.phase == GameLogicController.GamePhase.PHASE_TWO)
            {
                onState.SetActive(true);
                offState.SetActive(false);
            }
        }
    }

    // displays the current state of the tv depending on GameLogicController.Instance.phase
    // and whether the tv is evil. Evil tvs do not turn on during phase 1 but
    // they do during phase 2. Good tvs are always on
    public void DisplayState()
    {
        if (On)
            TurnOn();
        else
            Unplug();
    }
   
}
