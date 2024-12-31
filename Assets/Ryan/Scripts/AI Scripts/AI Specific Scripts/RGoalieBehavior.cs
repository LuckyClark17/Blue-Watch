using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class RGoalieBehavior : AIScript
{
    [SerializeField] [Range(0, 0.2f)] private float speed;
    
    public override void FixedUpdate()
    { 
        ballTransform = GameManager.Instance.ball.transform;
        ClosestUpperTeamMate(GameManager.Instance.RedDefenders);
        base.FixedUpdate();
        GoalieFollowingAim();
    }

    public override void Update()
    {
        Timer();
        DrawLines();
    }

    public override void Start()
    {
        intersectPoint = transform.position;
        base.Start();
    }
}