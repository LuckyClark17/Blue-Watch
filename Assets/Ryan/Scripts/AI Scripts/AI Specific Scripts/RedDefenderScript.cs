using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDefenderScript : AIScript
{ 
    public override void FixedUpdate()
    {  
        
        
        
        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.Middle)
            targetClampZ = -10f;

        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.BlueMidSpawn)
            targetClampZ = 2.5f;
        
        if(GameManager.Instance.ballBehavior == GameManager.BallBehavior.BlueDefSpawn)
            targetClampZ = 10f;
        
        MoveTowardsClamp();
        ClosestUpperTeamMate(GameManager.Instance.RedMidFielders);
        base.FixedUpdate();
        FindTeammates(GameManager.Instance.RedDefenders);

    }
}
