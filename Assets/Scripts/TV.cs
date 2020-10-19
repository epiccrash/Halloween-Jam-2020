using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is attached to TV gameobjects 
public class TV : MonoBehaviour
{
    public KeyCode unplugKey;

    // TVs that are not interactible cannot be turned on or off. used for Phase 1
    public bool isInteractible;

    // if the TV is unplugged when this is set to true, then the enemy is alerted
    public bool isEvil;

    // TV Material is set to offMaterial when unplugged
    public Material offMaterial;

    bool _hasUnplugged = false;

    // unplug the tv by going near to it and pressing the button
    private void OnTriggerStay(Collider other)
    {
        if (isInteractible)
        {
            if (Input.GetKeyDown(unplugKey))
            {
                if (!_hasUnplugged)
                {
                    Unplug();
                    if (isEvil)
                    {
                        MemoryLogicController.Instance.UnplugEvil(this);
                        Debug.Log("Unplug a an evil TV");
                    } else
                    {
                        MemoryLogicController.Instance.UnplugGood(this);
                        Debug.Log("Unplug a good TV");
                    }
                }
            }
        }
    }

    void Unplug()
    {
        _hasUnplugged = true;

        // play the unplug animation

        gameObject.GetComponent<Renderer>().material = offMaterial;
        return;
    }

   
}
