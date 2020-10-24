using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNote : MonoBehaviour
{
    public KeyCode close;

    private void Update()
    {
        if (Input.GetKey(close))
        {
            GameLogicController.Instance.player.GetComponent<FPController>().GiveControl();
            gameObject.SetActive(false);
        }
    }
}
