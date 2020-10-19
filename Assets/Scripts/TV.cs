using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is attached to TV gameobjects 
public class TV : MonoBehaviour
{
    public KeyCode unplugKey;

    // if the TV is unplugged when this is set to true, then the enemy is alerted
    bool isEvil;


    // unplug the tv by going near to it and pressing the button
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(unplugKey))
        {
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

    void Unplug()
    {
        // play the unplug animation

        return;
    }

   
}
