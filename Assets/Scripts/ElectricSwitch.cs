using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the electrical switch where the user turns off the power
public class ElectricSwitch : MonoBehaviour
{

    public KeyCode trigger;

    bool _hasSwitched = false;

    private void OnTriggerStay(Collider other)
    {
        if (!_hasSwitched)
        {
            if (other.gameObject.tag == "Player")
            {

                if (Input.GetKey(trigger))
                {
                    _hasSwitched = true;
                    GameLogicController.Instance.BeginPhaseTwo();
                }
            }
        }
    }

}
