using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// control overall flow of memory game
public class MemoryLogicController : UnitySingleton<MemoryLogicController>
{
    List<TV> goodTVs;
    List<TV> evilTVs;

    int TVsRemaining;

    private void Start()
    {
        // initialize goodTVs and evilTVs lists
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("TV"))
        {
            goodTVs = new List<TV>();
            evilTVs = new List<TV>();
            TV tv = g.GetComponent<TV>();
            if (tv.isEvil)
            {
                evilTVs.Add(tv);
            }
            else
            {
                goodTVs.Add(tv);
            }
        }

        TVsRemaining = goodTVs.Count;
    }

    // Executed when the player unplugs an evil TV
    public void UnplugEvil(TV tv)
    {
        // alert the enemy? spawn new enemy?
        evilTVs.Remove(tv);
    }

    // Executed when the player unplugs a good TV
    public void UnplugGood(TV tv)
    {
        goodTVs.Remove(tv);
        TVsRemaining--;

        // if there are no good tvs remaining, then the win condition has been met
        GameLogicController.Instance.Win();
    }
}

