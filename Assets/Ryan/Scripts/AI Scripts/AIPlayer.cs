// using System;
// using System.Collections;
// using System.Numerics;
// using Unity.VisualScripting;
// using UnityEngine;
// using Quaternion = UnityEngine.Quaternion;
// using Vector3 = UnityEngine.Vector3;
// using UnityEngine.AI;
// using UnityEngine.UIElements;

// public class AIPlayer : MonoBehaviour
// {
//     protected Transform ballTransform;
//     private Camera _camera1;
//     public bool enemyAvoidance;
//     public bool teammateAvoidance;
//     protected Vector3 intersectPoint;
//     private GameManager.BallBehavior BallBehavior;
//     protected float targetClampZ;
//     private float distanceToClamp;

//     public GameObject[] teammates;

//     private bool _timerOn;
//     private float _timer;
//     public bool interceptTimerOn;
//     public float interceptTimer;
//     public bool canIntercept = true; //true when timer
//     [Header("CLAMPED POSITIONS")] [Tooltip("FIRST ELEMENT IS FOR MIN, SECOND IS FOR MAX.")]
//     public float[] xClamp = new float[2];
//     public float[] zClamp = new float[2]; 
//     private float[][] _bounds;

//     [Header("Passing and Teammates")] protected GameObject closestUpperTeammate;
//     private GameObject _closestTeammate;
//     private float _angleToKick;
//     private Vector3 _targetDir;

//     [Header("AI FOV")] public float fovRadius;
//     public float fovAngle = 75;
//     private GameObject[] playersInRadius;
//     private GameObject[] playersInAIRadius;
//     private bool playerInFovRadius;
//     private bool playerInFov;
//     private Transform _target;
//     private bool movingTowardsClamp = true;

//     private bool playerInAIRadius;

//     [Header("Masks")] public LayerMask enemyMask;
//     public LayerMask teamMask;

//     protected bool tryingToIntercept;

//     private PassBehavior AIBehavior = PassBehavior.PassUp;
//     private bool reset;

//     //starting vectors
//     public Vector3 _startingPosition;
//     private Quaternion _startingRotation;

//     private bool startingCooldown;

//     //TODO: Before passing to teammate, try to get as close as possible to temamate, then check.
//     //TODO: 

//     private enum PassBehavior
//     {
//         PassUp,
//         PassToTeammate,
//         RunTheBallUp
//     }

//     private void ChecksPlayersReset()
//     {
//         if (Equals(gameObject.transform, _startingPosition))
//         {
//             //work on at home
//         }
//     }

//     public virtual void Start()
//     {
//         Time.timeScale = 0;
//         StartCoroutine(StartingCoroutine());
//         teammateAvoidance = true;
//         ballTransform = GameManager.Instance.ball.transform;
//         _camera1 = Camera.main;

//         _startingPosition = gameObject.transform.position;
//         _startingRotation = gameObject.transform.rotation; 
//     }

//     IEnumerator StartingCoroutine()
//     {
//         yield return new WaitForSecondsRealtime(7.5f);
//         Time.timeScale = 1;
//     }

//     public void resetPos()
//     {
//         reset = GameManager.Instance.scored;
//         if (reset)
//         {
//             print("RUNNING RESET");
//             gameObject.transform.position = _startingPosition;
//             gameObject.transform.rotation = _startingRotation;
//             reset = false;
//         }

//         if (GameManager.Instance.allHaveReset)
//         {
//             GameManager.Instance.scored = false;
//         }
        
//     }
//     /*
//  * Method sets the ball a child of the object it touches, and sets the ball directly in front of the player.
//  * ball is not effected by physics while held.
//      * When AI picks up the ball, the default behavior is to try and pass the ball up.
//  */
//     public virtual void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("Ball"))
//         {
//             interceptTimerOn = true;
//             canIntercept = false;
//             GameObject ballObject = collision.gameObject;
//             ballObject.transform.rotation = transform.rotation;
//             ballObject.transform.SetParent(transform);
//             GameManager.Instance._ballrb.isKinematic = true;        
//             ballObject.transform.position = transform.localPosition;
//             ballObject.transform.position = transform.localPosition + transform.TransformDirection(new Vector3(0, 0, 1.1f));
//             GameManager.Instance.ballHeldByAI = true;
//             AIBehavior = PassBehavior.PassUp;
//         }
//     }

//     // protected void ClosestUpperTeamMate(GameObject[] UpperTeammates)
//     // {
//     //     int lowestDistanceIndex = 0;
//     //     float smallest = float.MaxValue;
//     //     float[] distanceBetween = new float[UpperTeammates.Length];
//     //     for (int i = 0; i < UpperTeammates.Length; i++)
//     //     {
//     //         distanceBetween[i] = Vector3.Distance(transform.position, UpperTeammates[i].transform.position);

//     //         if (distanceBetween[i] < smallest)
//     //         {
//     //             smallest = distanceBetween[i];
//     //             lowestDistanceIndex = i;
//     //         }
//     //     }
//     //     closestUpperTeammate = UpperTeammates[lowestDistanceIndex];
//     // }


//     // /*
//     //  * Gets the position of said object and creates a new array of the objects teammates (minus the object itself) and then finds the distance between
//     //  * the teammates and the object itself.
//     //  */

//     // /*
//     //  * returns array of teammmates EXCLUDING the object its called on itself.
//     //  */
    
    
//     // //correctPos should ONLY GO UP if the object isnt talking about itself
    
//     // protected GameObject[] FindTeammates(GameObject[] position)
//     // {
//     //     teammates = new GameObject[position.Length-1];

//     //     int correctIndexPos = 0;
//     //     //loops through all of the players. If the player it is looking at is unique and is not the player the script is attached to, then the player is added to teammates.
//     //     for (int index = 0; index < position.Length; index++)
//     //     {
//     //         if (position[index] != gameObject)
//     //         {
//     //             teammates[correctIndexPos] = position[index];
//     //             correctIndexPos++;
//     //         }
//     //     }
//     //     return teammates;
//     // }

//     // private void ClosestTeammate()
//     // {
//     //     int lowestDistanceIndex = 0;
//     //     float smallest = float.MaxValue;
        

//     //     for (int i = 0; i < teammates.Length; i++)
//     //     {
//     //         if (teammates[i] != gameObject)
//     //         {
//     //             float distanceBetween = Vector3.Distance(transform.position, teammates[i].transform.position);

//     //             if (distanceBetween < smallest)
//     //             {
//     //                 smallest = distanceBetween;
//     //                 lowestDistanceIndex = i;
//     //             }

//     //             _closestTeammate = teammates[lowestDistanceIndex];
//     //         }
//     //     }
//     // }


//     private void PassBall()
//     {
//         if (!ObjectHasBall()) return;
//         if (AIBehavior == PassBehavior.PassUp)
//         {
//            RotateTowards(closestUpperTeammate.transform);
//         }

//         if (AIBehavior == PassBehavior.PassToTeammate)
//         {
//           RotateTowards(_closestTeammate.transform);
//         }

//         if (_angleToKick < 1 && !FieldOfViewCheck())
//         {
//             _timerOn = true;

//             if (_timer >= GameManager.Instance.delayBeforeKick)
//             {
//                 GameManager.Instance._ballrb.isKinematic = false;
//                 GameManager.Instance.ball.transform.SetParent(null);
//                 GameManager.Instance._ballrb.AddForce(
//                     transform.forward * GameManager.Instance.AIShotPower, ForceMode.Impulse);
//                 _timerOn = false;
//                 GameManager.Instance.ballHeldByAI = false;

//                 if (gameObject.layer == GameManager.Instance.objectThatHasBall.layer)
//                     GameManager.Instance.playerWhoKicked = gameObject;
//             }
//         }  
//     }




//     private void SetPassBehavior()
//     {
//         if (gameObject.name == "RedForward")
//         {
//             return;
//         }
        
//         switch (AIBehavior)
//         {
//             case PassBehavior.PassUp:
//                 _targetDir = closestUpperTeammate.transform.position - transform.position;
//                 break;
//             case PassBehavior.PassToTeammate:
//                 _targetDir = _closestTeammate.transform.position - transform.position;
//                 break;
//         }
//         _angleToKick = Vector3.Angle(transform.forward, _targetDir);


//         if (_angleToKick < 2 &&
//             FieldOfViewCheck()) // if the AI is looking at its teammate and someone is in his field of view
//         {
//             if (AIBehavior == PassBehavior.PassUp)
//             {
//                 AIBehavior = PassBehavior.PassToTeammate;
//             }
//             else if (AIBehavior == PassBehavior.PassToTeammate)
//             {
//                 AIBehavior = PassBehavior.PassUp;
//             }
//         }
//     }


//     private GameObject[] ObjectsInFovRadius()
//     {
//         Collider[] rangeChecks = Physics.OverlapSphere(transform.position, fovRadius, enemyMask);
//         playersInRadius = new GameObject[rangeChecks.Length];
//         if (rangeChecks.Length != 0)
//         {
//             playerInFovRadius = true;
//             for (int i = 0; i < rangeChecks.Length; i++)
//             {
//                 playerInFov = true;
//                 if (rangeChecks[i].gameObject.layer != gameObject.layer)
//                     playersInRadius[i] = rangeChecks[i].gameObject;
//                 _target = playersInRadius[i].transform;
//             }
//         }
//         return playersInRadius;
//     }

//     private bool FieldOfViewCheck()
//     {
        
//         if(ObjectsInFovRadius().Length !=0)
//             for (int i = 0; i < ObjectsInFovRadius().Length; i++)
//             {
//                 _target = ObjectsInFovRadius()[i].transform;

//                 Vector3 directionToTarget = (_target.position - transform.position).normalized;

//                 if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 2)
//                 {
//                     var distanceToTarget = Vector3.Distance(transform.position, _target.position);
//                     if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, teamMask))
//                     {
//                         playerInFov = true;
//                         return true;
//                     }
//                     playerInFov = false;
//                 }
//                 else
//                 {
//                     playerInFov = false;
//                 }
//             }
//         playerInFov = false;
//         return false;
//     }


//      void ClampPos()
//     {
//         _bounds = new[] { xClamp, zClamp };
        
//         Vector3 clampedPos = new Vector3();
//         clampedPos.x = Mathf.Clamp(transform.position.x, _bounds[0][0], _bounds[0][1]);
//         clampedPos.y = transform.position.y;
//         clampedPos.z = Mathf.Clamp(transform.position.z, _bounds[1][0], _bounds[1][1]);
//         transform.position = clampedPos;
//     }

//     protected void MoveUp(Vector3 targetPos, float speed)
//     {
//         var followOnZ = new Vector3(transform.position.x, transform.position.y, targetPos.z);
//         transform.position = Vector3.MoveTowards(transform.position, followOnZ, GameManager.Instance.runSpeed);
//     }

//     protected void GoalieFollowingAim()
//     {
//         if (GameManager.Instance.ballHeldByPlayer)
//         {
//             intersectPoint = GameManager.Instance.player.transform.position;

//             if (gameObject.CompareTag("RedGoalie"))
//                 while (intersectPoint.z > transform.position.z && _camera1.transform.forward.z <= 0)
//                 {
//                     intersectPoint.x += _camera1.transform.forward.x;
//                     intersectPoint.z += _camera1.transform.forward.z;
//                 }

//             if (gameObject.CompareTag("BlueGoalie"))
//                 while (intersectPoint.z < transform.position.z && _camera1.transform.forward.z > 0)
//                 {
//                     intersectPoint.x += _camera1.transform.forward.x;
//                     intersectPoint.z += _camera1.transform.forward.z;
//                 }

//             transform.LookAt(GameManager.Instance.ball.transform);
//         }

//         if (GameManager.Instance.ballHeldByAI && GameManager.Instance.objectThatHasBall.name == "RedForward" && GameManager.Instance.levelIndex == 4)
//         {
//             Transform aiShooter = GameManager.Instance.objectThatHasBall.transform;
//             intersectPoint = aiShooter.position;
            
//             if (gameObject.CompareTag("RedGoalie"))
//                 while (intersectPoint.z > aiShooter.position.z)
//                 {
//                     intersectPoint.x += aiShooter.forward.x;
//                     intersectPoint.z += aiShooter.forward.z;
//                 }
        
//             if (gameObject.CompareTag("BlueGoalie"))
//                 while (intersectPoint.z < transform.position.z)
//                 {
//                     intersectPoint.x += aiShooter.forward.x;
//                     intersectPoint.z += aiShooter.forward.z;
//                 }

//             GameManager.Instance.AIintersectPoint = intersectPoint;
//             transform.LookAt(GameManager.Instance.ball.transform);
//             Debug.DrawLine(aiShooter.position, intersectPoint);
//         }
//         if ((ballTransform.position.z < -32 && Math.Abs(ballTransform.position.x) < 10 && ballTransform.position.y is < 7 and > 0) || (ballTransform.position.z > 72 && Math.Abs(ballTransform.position.x) < 10 && 
//                 ballTransform.position.y is < 7 and > 0))
//             intersectPoint.y = GameManager.Instance.ball.transform.position.y;

//         transform.position = Vector3.MoveTowards(transform.position, intersectPoint, GameManager.Instance.runSpeed);
//     }

//     protected void DrawLines()
//     {
// //        Debug.DrawLine(transform.position, closestUpperTeammate.transform.position, Color.red);
//         if (_closestTeammate != null)
//             Debug.DrawLine(transform.position, _closestTeammate.transform.position, Color.yellow);
//     }

//     protected bool ObjectHasBall()
//     {
//         if (ballTransform.parent == transform)
//         {
//             GameManager.Instance.objectThatHasBall = gameObject;
//             return true;
//         }

//         return false;
//     }

//     private bool TeamHasBall()
//     {
//         if (ObjectHasBall()) // the player that has the ball = the object that has the ball
//             GameManager.Instance.objectThatHasBall = gameObject;

//         if (GameManager.Instance.objectThatHasBall!= null &&gameObject.layer == GameManager.Instance.objectThatHasBall.layer)
//                 return true;

//         return false;
//     }

//     private void InterceptBall()
//     {
//         if (!canIntercept) return;
//         tryingToIntercept = false;
//         if (Vector3.Distance(transform.position, ballTransform.position) < GameManager.Instance.aiRadius && !TeamHasBall())
//         {
//             {
//                RotateTowards(ballTransform);
//                 transform.position = Vector3.MoveTowards(transform.position, new Vector3(ballTransform.position.x, transform.position.y,ballTransform.position.z),
//                     GameManager.Instance.runSpeed);

//                 tryingToIntercept = true;
//             }
//         }
//     }

//     protected void MoveTowardsClamp()
//     {
//         if (!BallInFrontOfPlayer())
//             return;
//         distanceToClamp = Mathf.Abs(transform.position.z - targetClampZ);

//         switch (distanceToClamp < 1)
//         {
//             case true:
//             {
//                 movingTowardsClamp = false;
//                 return;
//             }
//             case false:
//             {
//                 movingTowardsClamp = true;
//                 break;
//             }
//         }

//         transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, targetClampZ), GameManager.Instance.runSpeed);

//         if (gameObject.layer == 8) // if layer is blue 
//         {
//             if (targetClampZ > transform.position.z)
//             {
//                 if (distanceToClamp < 1)
//                 {
//                  //   teammateAvoidance = true;
//                     zClamp[0] = targetClampZ;
//                 }
//             }
//             else if (targetClampZ < transform.position.z)
//                 zClamp[0] = targetClampZ;
//         }

//         else if (gameObject.layer == 7) //if layer red
//         {
//             if (targetClampZ < transform.position.z)
//             {
//                 if (distanceToClamp < 1)
//                 {
//                 //    teammateAvoidance = true;
//                     zClamp[1] = targetClampZ;
//                 }
//             }
//             else if (targetClampZ > transform.position.z)
//                 zClamp[1] = targetClampZ;
//         }
//     }

//     protected void ReflectBall()
//     {
//         RotateTowards(ballTransform);
//         if(BallInFrontOfPlayer())
//         transform.position = Vector3.MoveTowards(transform.position, new Vector3(ballTransform.position.x, transform.position.y, transform.position.z),
//             GameManager.Instance.reflectSpeed);
//     }
//     protected void MoveTowardsBall()
//     {
//         if (!TeamHasBall())
//         {
//             RotateTowards(ballTransform);
//             if (!BallInFrontOfPlayer() && !tryingToIntercept)
//                 transform.position = Vector3.MoveTowards(transform.position,
//                     new Vector3(ballTransform.position.x, transform.position.y, ballTransform.position.z),
//                     GameManager.Instance.reflectSpeed);
//         }
//     }

// /*
// * timer used for kicking and the delay between when the player has a good shot and when he actually kicks. Timer starts when _timerOn is true.
// */
//     protected void Timer()
//     {
//         if (_timerOn && _timer <=  GameManager.Instance.delayBeforeKick)
//         {
//             _timer += Time.deltaTime;
//         }
//         if (!_timerOn)
//         {
//             _timer = 0;
//         }

//         if (interceptTimerOn && interceptTimer <= GameManager.Instance.interceptCooldownTimer)
//         {
//             interceptTimer += Time.deltaTime;
//         }

//         if (interceptTimer >= GameManager.Instance.interceptCooldownTimer)
//         {
//             canIntercept = true;
//             interceptTimerOn = false;
//         }

//         if (!interceptTimerOn)
//         {
//             interceptTimer = 0;
//         }
//     }

//     private void EnemyAvoidance()
//     {
//         Collider[] rangeChecks = Physics.OverlapSphere(transform.position, GameManager.Instance.aiRadius, enemyMask);
//         playersInAIRadius = new GameObject[rangeChecks.Length];
//         for (var i = 0; i < rangeChecks.Length; i++)
//         {
//             playersInAIRadius[i] = rangeChecks[i].gameObject;
//         }
//         if (enemyAvoidance)
//         {
//             if (rangeChecks.Length != 0)
//                 foreach (var col in rangeChecks)
//                 {
//                     Vector3 oppDir = transform.position - col.transform.position;
//                     oppDir.y = transform.position.y;
//                     Debug.DrawLine(transform.position, transform.position + oppDir);
//                     transform.position = Vector3.MoveTowards(transform.position, transform.position + oppDir,
//                         GameManager.Instance.runSpeed);
//                 }
//             else if (ObjectHasBall())
//                 transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y,50f), GameManager.Instance.runSpeed);
//         }
//     }

//     private void TeamAvoidance()
//     {
//         if (tryingToIntercept) teammateAvoidance = false;
//         else teammateAvoidance = true;
        
//         if (!teammateAvoidance) return;
//         foreach (var teammate in teammates)
//         {
//             if (Vector3.Distance(transform.position, teammate.transform.position) < GameManager.Instance.aiRadius)
//             {
//                 Vector3 oppDir = transform.position - teammate.transform.position;
//                 oppDir.y = transform.position.y;
//                 oppDir.z = 0;
//                 Debug.DrawLine(transform.position, transform.position + oppDir);
//                 transform.position = Vector3.MoveTowards(transform.position, transform.position + oppDir,
//                     GameManager.Instance.runSpeed);
//             }
//         }
//     }

//     public void RotateTowards(Transform target)
//     {
//         transform.rotation = Quaternion.RotateTowards(transform.rotation,
//             Quaternion.LookRotation(target.position - transform.position),
//             Time.deltaTime * GameManager.Instance.AITurnSpeed);
//     }

//     bool BallInFrontOfPlayer()
//     {
//         if (gameObject.layer == 8) //blue players
//         {
//             if (ballTransform.position.z < transform.position.z)
//             {
//                 return true;
//             }
//         }

//         if (gameObject.layer == 7) // red players
//         {
//             if (ballTransform.position.z > transform.position.z)
//             {
//                 return true;
//             }
//         }

//         return false;
//     }
        
    
// /*
//  * stops players from flipping over.
//  */
//     private void LateUpdate()
//     {
//         transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
//     }
    
//     public virtual void Update()
//     {
//         ChecksPlayersReset();
//         resetPos();
//         ObjectsInFovRadius();
//         FieldOfViewCheck();
//         SetPassBehavior();
//         Timer();
//         TeamHasBall();
//         DrawLines();
//     }

//     public virtual void FixedUpdate()
//     {
//         if(gameObject.name != "RedForward")
//         fovRadius = Vector3.Distance(transform.position, closestUpperTeammate.transform.position);
//         ballTransform = GameManager.Instance.ball.transform;
//         ClampPos();
//         EnemyAvoidance();
//         InterceptBall();
//         ClosestTeammate();
    

//         if(gameObject.name != "RedForward")
//         {
//             ReflectBall();
//            MoveTowardsBall();
//             TeamAvoidance();
//             PassBall();
//         }
//     }
// }
