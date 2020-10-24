using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public KeyCode openDoor;

    bool _hasOpened = false;

    // call Interact() when the player pressed the openDoor key while within
    // range of door collider
    private void OnTriggerStay(Collider other)
    {
        if (!_hasOpened)
        {
            if (other.gameObject.tag == "Player")
            {
            
                if (Input.GetKeyDown(openDoor))
                {
                    Interact();
                }
            }
        }
    }

    // if TVs have been turned off correcltly, proceed to win screen.
    // otherwise, alert the enemy
    void Interact()
    {
        if (MemoryLogicController.Instance.TVsUnpluggedCorrectly())
        {
            GameLogicController.Instance.Win();
        }
    }
}
