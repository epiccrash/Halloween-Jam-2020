using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the electrical switch where the user turns off the power
[RequireComponent(typeof(Animator))]
public class ElectricSwitch : MonoBehaviour
{

    public KeyCode trigger;
	public AudioSource audioSource;

    [Header("Lights")]
    [SerializeField]
    private Transform lightsParentObject;
    [SerializeField]
    private Material lightsOffMaterial;

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
					audioSource.Play();
                    StartCoroutine(TurnOffLights());
					GameLogicController.Instance.BeginPhaseTwo();
                }
            }
        }
    }

    private IEnumerator TurnOffLights()
    {
        foreach (Transform lightObj in lightsParentObject)
        {
            lightObj.GetComponent<MeshRenderer>().material = lightsOffMaterial;
            lightObj.GetChild(0).gameObject.SetActive(false);
            yield return null;
        }
        yield return null;
    }
}
