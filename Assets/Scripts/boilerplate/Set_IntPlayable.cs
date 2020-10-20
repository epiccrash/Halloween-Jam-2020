using UnityEngine.Playables;

public class Set_IntPlayable : PlayableBehaviour
{
    public int val;
    public IntRef Reference;

    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        IntRef ref_in = playerData as IntRef;
        Reference = ref_in;
        Reference.val = val;   
    }
}