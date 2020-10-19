using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the electrical switch where the user turns off the power
public class ElectricSwitch : MonoBehaviour
{

    public KeyCode trigger;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKey(trigger))
            {
                GameLogicController.Instance.BeginPhaseTwo();
            }
        }
    }

}
