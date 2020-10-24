using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// control logic of memory game. the game flow is defined in GameLogicController
public class MemoryLogicController : UnitySingleton<MemoryLogicController>
{
    public List<TV> goodTVs;
    public List<TV> evilTVs;
    public List<TV> allTVs;

    public int TVsRemaining;

    private void Start()
    {
        goodTVs = new List<TV>();
        evilTVs = new List<TV>();
        // initialize goodTVs and evilTVs lists
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("TV"))
        {
            TV tv = g.GetComponent<TV>();
            if (tv.Evil())
            {
                evilTVs.Add(tv);
                allTVs.Add(tv);
            }
            else
            {
                goodTVs.Add(tv);
                allTVs.Add(tv);
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
    }

    // Returns true when the player has unplugged all TVs correctly, false otherwise
    public bool TVsUnpluggedCorrectly()
    {
        return TVsRemaining == 0;
    }
}

