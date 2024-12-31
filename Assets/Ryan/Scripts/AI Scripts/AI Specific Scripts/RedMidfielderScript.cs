using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RedMidfielderScript : AIScript
{
    public override void FixedUpdate()
    { 
        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.Middle)
            targetClampZ = 2.5f;

        if (GameManager.Instance.ballBehavior == GameManager.BallBehavior.BlueMidSpawn)
           targetClampZ = 20;
        
        if(GameManager.Instance.ballBehavior == GameManager.BallBehavior.BlueDefSpawn)
            targetClampZ = 37.5f;
        
        
        MoveTowardsClamp();
        closestUpperTeammate = GameManager.Instance.RedStriker;
        base.FixedUpdate();
             FindTeammates(GameManager.Instance.RedMidFielders);
       

    }
}
