using System;
using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;
using UnityEngine.UIElements;

public partial class AIScript : MonoBehaviour
{
protected void GoalieFollowingAim()
    {
        if (GameManager.Instance.ballHeldByPlayer)
        {
            intersectPoint = GameManager.Instance.player.transform.position;

            if (gameObject.CompareTag("RedGoalie"))
                while (intersectPoint.z > transform.position.z && _camera1.transform.forward.z <= 0)
                {
                    intersectPoint.x += _camera1.transform.forward.x;
                    intersectPoint.z += _camera1.transform.forward.z;
                }

            if (gameObject.CompareTag("BlueGoalie"))
                while (intersectPoint.z < transform.position.z && _camera1.transform.forward.z > 0)
                {
                    intersectPoint.x += _camera1.transform.forward.x;
                    intersectPoint.z += _camera1.transform.forward.z;
                }

            transform.LookAt(GameManager.Instance.ball.transform);
        }

        if (GameManager.Instance.ballHeldByAI && GameManager.Instance.objectThatHasBall.name == "RedForward" && GameManager.Instance.levelIndex == 4)
        {
            Transform aiShooter = GameManager.Instance.objectThatHasBall.transform;
            intersectPoint = aiShooter.position;
            
            if (gameObject.CompareTag("RedGoalie"))
                while (intersectPoint.z > aiShooter.position.z)
                {
                    intersectPoint.x += aiShooter.forward.x;
                    intersectPoint.z += aiShooter.forward.z;
                }
        
            if (gameObject.CompareTag("BlueGoalie"))
                while (intersectPoint.z < transform.position.z)
                {
                    intersectPoint.x += aiShooter.forward.x;
                    intersectPoint.z += aiShooter.forward.z;
                }

            GameManager.Instance.AIintersectPoint = intersectPoint;
            transform.LookAt(GameManager.Instance.ball.transform);
            Debug.DrawLine(aiShooter.position, intersectPoint);
        }
        if ((ballTransform.position.z < -32 && Math.Abs(ballTransform.position.x) < 10 && ballTransform.position.y is < 7 and > 0) || (ballTransform.position.z > 72 && Math.Abs(ballTransform.position.x) < 10 && 
                ballTransform.position.y is < 7 and > 0))
            intersectPoint.y = GameManager.Instance.ball.transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, intersectPoint, GameManager.Instance.runSpeed);
    }
}
