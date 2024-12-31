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
    bool BallInFrontOfPlayer()
    {
        if (gameObject.layer == 8) //blue players
        {
            if (ballTransform.position.z < transform.position.z)
            {
                return true;
            }
        }

        if (gameObject.layer == 7) // red players
        {
            if (ballTransform.position.z > transform.position.z)
            {
                return true;
            }
        }

        return false;
    }
    protected void ReflectBall()
    {
        RotateTowards(ballTransform);
        if(BallInFrontOfPlayer())
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(ballTransform.position.x, transform.position.y, transform.position.z),
            GameManager.Instance.reflectSpeed);
    }
    protected void MoveTowardsBall()
    {
        if (!TeamHasBall())
        {
            RotateTowards(ballTransform);
            if (!BallInFrontOfPlayer() && !tryingToIntercept)
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(ballTransform.position.x, transform.position.y, ballTransform.position.z),
                    GameManager.Instance.reflectSpeed);
        }
    }
        protected bool ObjectHasBall()
    {
        if (ballTransform.parent == transform)
        {
            return true;
        }

        return false;
    }
        private bool TeamHasBall()
    {
        if (ObjectHasBall()) // the player that has the ball = the object that has the ball
            GameManager.Instance.objectThatHasBall = gameObject;

        if (GameManager.Instance.objectThatHasBall!= null &&gameObject.layer == GameManager.Instance.objectThatHasBall.layer)
                return true;

        return false;
    }
    private void InterceptBall()
    {
        if (!canIntercept) return;
        tryingToIntercept = false;
        if (Vector3.Distance(transform.position, ballTransform.position) < GameManager.Instance.aiRadius && !TeamHasBall())
        {
            {
               RotateTowards(ballTransform);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(ballTransform.position.x, transform.position.y,ballTransform.position.z),
                    GameManager.Instance.runSpeed);

                tryingToIntercept = true;
            }
        }
    }
    private void PassBall()
    {
        if (!ObjectHasBall()) return;
        if (AIBehavior == PassBehavior.PassUp)
        {
           RotateTowards(closestUpperTeammate.transform);
        }

        if (AIBehavior == PassBehavior.PassToTeammate)
        {
          RotateTowards(_closestTeammate.transform);
        }

        if (_angleToKick < 1 && !FieldOfViewCheck())
        {
            _timerOn = true;

            if (_timer >= GameManager.Instance.delayBeforeKick)
            {
                GameManager.Instance._ballrb.isKinematic = false;
                GameManager.Instance.ball.transform.SetParent(null);
                GameManager.Instance._ballrb.AddForce(
                    transform.forward * GameManager.Instance.AIShotPower, ForceMode.Impulse);
                _timerOn = false;
                GameManager.Instance.ballHeldByAI = false;

                if (gameObject.layer == GameManager.Instance.objectThatHasBall.layer)
                    GameManager.Instance.playerWhoKicked = gameObject;
            }
        }  
    }
    public void SetPassBehavior()
    {
        if (gameObject.name == "RedForward")
        {
            return;
        }
        
        switch (AIBehavior)
        {
            case PassBehavior.PassUp:
                _targetDir = closestUpperTeammate.transform.position - transform.position;
                break;
            case PassBehavior.PassToTeammate:
                _targetDir = _closestTeammate.transform.position - transform.position;
                break;
        }
        _angleToKick = Vector3.Angle(transform.forward, _targetDir);


        if (_angleToKick < 2 &&
            FieldOfViewCheck()) // if the AI is looking at its teammate and someone is in his field of view
        {
            if (AIBehavior == PassBehavior.PassUp)
            {
                AIBehavior = PassBehavior.PassToTeammate;
            }
            else if (AIBehavior == PassBehavior.PassToTeammate)
            {
                AIBehavior = PassBehavior.PassUp;
            }
        }
    }
        public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            GameManager.Instance.objectThatHasBall = gameObject;
            interceptTimerOn = true;
            canIntercept = false;
            GameObject ballObject = collision.gameObject;
            ballObject.transform.rotation = transform.rotation;
            ballObject.transform.SetParent(transform);
            GameManager.Instance._ballrb.isKinematic = true;        
            ballObject.transform.position = transform.localPosition;
            ballObject.transform.position = transform.localPosition + transform.TransformDirection(new Vector3(0, 0, 1.1f));
            GameManager.Instance.ballHeldByAI = true;
            AIBehavior = PassBehavior.PassUp;
        }
    }

    private bool FieldOfViewCheck()
    {
        
        if(ObjectsInFovRadius().Length !=0)
            for (int i = 0; i < ObjectsInFovRadius().Length; i++)
            {
                _target = ObjectsInFovRadius()[i].transform;

                Vector3 directionToTarget = (_target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 2)
                {
                    var distanceToTarget = Vector3.Distance(transform.position, _target.position);
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, teamMask))
                    {
                        playerInFov = true;
                        return true;
                    }
                    playerInFov = false;
                }
                else
                {
                    playerInFov = false;
                }
            }
        playerInFov = false;
        return false;
    }
        private GameObject[] ObjectsInFovRadius()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, fovRadius, enemyMask);
        playersInRadius = new GameObject[rangeChecks.Length];
        if (rangeChecks.Length != 0)
        {
            playerInFovRadius = true;
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                playerInFov = true;
                if (rangeChecks[i].gameObject.layer != gameObject.layer)
                    playersInRadius[i] = rangeChecks[i].gameObject;
                _target = playersInRadius[i].transform;
            }
        }
        return playersInRadius;
    }
    //* timer used for kicking and the delay between when the player has a good shot and when he actually kicks. Timer starts when _timerOn is true.
    protected void Timer()
    {
        if (_timerOn && _timer <=  GameManager.Instance.delayBeforeKick)
        {
            _timer += Time.deltaTime;
        }
        if (!_timerOn)
        {
            _timer = 0;
        }

        if (interceptTimerOn && interceptTimer <= GameManager.Instance.interceptCooldownTimer)
        {
            interceptTimer += Time.deltaTime;
        }

        if (interceptTimer >= GameManager.Instance.interceptCooldownTimer)
        {
            canIntercept = true;
            interceptTimerOn = false;
        }

        if (!interceptTimerOn)
        {
            interceptTimer = 0;
        }
    }
}