using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryLogicController : UnitySingleton<MemoryLogicController>
{
    public List<TV> goodTVs;
    public List<TV> evilTVs;

    int TVsRemaining;

    private void Start()
    {
        TVsRemaining = goodTVs.Count;     
    }

    public void UnplugEvil(TV tv)
    {
        // alert the enemy? spawn new enemy?
        evilTVs.Remove(tv);
    }

    public void UnplugGood(TV tv)
    {
        goodTVs.Remove(tv);
        TVsRemaining--;

        // if there are no good tvs remaining, then the win condition has been met
        GameLogicController.Instance.Win();
    }
}

