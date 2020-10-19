using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the electrical switch where the user turns off the power
[RequireComponent(typeof(Animator))]
public class ElectricSwitch : MonoBehaviour
{

    public KeyCode trigger;

    bool _hasSwitched = false;

    Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_hasSwitched)
        {
            if (other.gameObject.tag == "Player")
            {

                if (Input.GetKey(trigger))
                {
                    _hasSwitched = true;
                    anim.SetTrigger("turn off");
                    GameLogicController.Instance.BeginPhaseTwo();
                }
            }
        }
    }

}
