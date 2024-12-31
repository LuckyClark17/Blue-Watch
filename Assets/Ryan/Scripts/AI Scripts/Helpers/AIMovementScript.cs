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
public void RotateTowards(Transform target)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(target.position - transform.position),
            Time.deltaTime * GameManager.Instance.AITurnSpeed);
    }
    private void TeamAvoidance()
    {
        if (tryingToIntercept) teammateAvoidance = false;
        else teammateAvoidance = true;
        
        if (!teammateAvoidance) return;
        foreach (var teammate in teammates)
        {
            if (Vector3.Distance(transform.position, teammate.transform.position) < GameManager.Instance.aiRadius)
            {
                Vector3 oppDir = transform.position - teammate.transform.position;
                oppDir.y = transform.position.y;
                oppDir.z = 0;
                Debug.DrawLine(transform.position, transform.position + oppDir);
                transform.position = Vector3.MoveTowards(transform.position, transform.position + oppDir,
                    GameManager.Instance.runSpeed);
            }
        }
    }protected void MoveTowardsClamp()
    {
        if (!BallInFrontOfPlayer())
            return;
        distanceToClamp = Mathf.Abs(transform.position.z - targetClampZ);

        switch (distanceToClamp < 1)
        {
            case true:
            {
                movingTowardsClamp = false;
                return;
            }
            case false:
            {
                movingTowardsClamp = true;
                break;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, targetClampZ), GameManager.Instance.runSpeed);

        if (gameObject.layer == 8) // if layer is blue 
        {
            if (targetClampZ > transform.position.z)
            {
                if (distanceToClamp < 1)
                {
                 //   teammateAvoidance = true;
                    zClamp[0] = targetClampZ;
                }
            }
            else if (targetClampZ < transform.position.z)
                zClamp[0] = targetClampZ;
        }

        else if (gameObject.layer == 7) //if layer red
        {
            if (targetClampZ < transform.position.z)
            {
                if (distanceToClamp < 1)
                {
                //    teammateAvoidance = true;
                    zClamp[1] = targetClampZ;
                }
            }
            else if (targetClampZ > transform.position.z)
                zClamp[1] = targetClampZ;
        }
    }
     void ClampPos()
    {
        _bounds = new[] { xClamp, zClamp };
        
        Vector3 clampedPos = new Vector3();
        clampedPos.x = Mathf.Clamp(transform.position.x, _bounds[0][0], _bounds[0][1]);
        clampedPos.y = transform.position.y;
        clampedPos.z = Mathf.Clamp(transform.position.z, _bounds[1][0], _bounds[1][1]);
        transform.position = clampedPos;
    }
        protected void MoveUp(Vector3 targetPos, float speed)
    {
        var followOnZ = new Vector3(transform.position.x, transform.position.y, targetPos.z);
        transform.position = Vector3.MoveTowards(transform.position, followOnZ, GameManager.Instance.runSpeed);
    }

    private void EnemyAvoidance()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, GameManager.Instance.aiRadius, enemyMask);
        playersInAIRadius = new GameObject[rangeChecks.Length];
        for (var i = 0; i < rangeChecks.Length; i++)
        {
            playersInAIRadius[i] = rangeChecks[i].gameObject;
        }
        if (enemyAvoidance)
        {
            if (rangeChecks.Length != 0)
                foreach (var col in rangeChecks)
                {
                    Vector3 oppDir = transform.position - col.transform.position;
                    oppDir.y = transform.position.y;
                    Debug.DrawLine(transform.position, transform.position + oppDir);
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + oppDir,
                        GameManager.Instance.runSpeed);
                }
            else if (ObjectHasBall())
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y,50f), GameManager.Instance.runSpeed);
        }
    }
}