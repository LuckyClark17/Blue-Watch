using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueMidfielderScript : AIScript
{
    public override void FixedUpdate()
    {  
        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.Middle)
            targetClampZ = 37.5f;

        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.RedMidSpawn)
             targetClampZ = 20f;
        
            if(GameManager.Instance.ballBehavior == GameManager.BallBehavior.RedDefSpawn)
                  targetClampZ = 10.5f;
            
        closestUpperTeammate = GameManager.Instance.BlueStriker;
        base.FixedUpdate();
        MoveTowardsClamp();

        FindTeammates(GameManager.Instance.BlueMidFielders);

    }
}
