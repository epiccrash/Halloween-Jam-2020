﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger_event_on_input : MonoBehaviour
{

    [SerializeField] private UnitEvent hook = null;
    [SerializeField] private KeyCode k = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(k))
            hook.Invoke();
    }
}
