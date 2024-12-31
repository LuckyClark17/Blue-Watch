using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlueDefenderScript : AIScript
{
    
    public override void FixedUpdate()
    {

        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.Middle)
            targetClampZ = 50f;

        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.RedMidSpawn)
            targetClampZ = 37.5f;
        
        if(GameManager.Instance.ballBehavior == GameManager.BallBehavior.RedDefSpawn)
            targetClampZ = 30f;
        
        
        MoveTowardsClamp();
       MoveTowardsBall();
        ClosestUpperTeamMate(GameManager.Instance.BlueMidFielders);
         base.FixedUpdate();
        FindTeammates(GameManager.Instance.BlueDefender);

    
    }
}
