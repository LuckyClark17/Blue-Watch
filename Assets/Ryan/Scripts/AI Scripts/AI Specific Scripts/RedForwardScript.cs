using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Cinemachine;
using Cinemachine.Utility;
using Unity.Rendering.HybridV2;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class RedForwardScript : AIScript
{

   
   //arrays for level one through five 
   [Header("Level Transforms")]
   public Transform[] levelOne;
   public Transform[] levelTwo;
   public Transform[] levelThree;
   public Transform[] levelFour;
   public Transform[] levelFive;

   private Transform[][] _levels;

   private bool firstInstance = true;
   private Vector3 targetDir;
   public float angle;

   private Transform closestTransform;

   private GameObject blueGoalie; 
   public Transform leftGoalPost;
   public Transform rightGoalPost;
   public Transform targetRotation;
   public Transform centerGoal;

   public float distanceFromGoalie;

   public bool timerOn;
   public float timer;
   public float rotateTime;
   private bool canKick;

   public bool calculate = false;
   private int counter = 0;
   private int correctIndexPos = 0;

   [Tooltip("Index for which Level you're at")]
   private int levelIndex = 0;

   [Tooltip("Array of current levels transforms")]
   private Transform[] currentLvlTransforms;

   [Tooltip("Array of closet transforms relative to the player")]
   private Transform[] closeLvlTransforms;
   
   private List<Transform> clearedCloseLvls;

   [Tooltip("How far the player will look for levels near itself")]
   public float range;
   [Tooltip("Index for currentLvlTransforms that is randomly chosen.")]
   private int randomIndex;

   public Transform spawnTransform;
   private Vector3 spawnPos;
   private Transform targetTransform;

   [HideInInspector]
   public GameObject[] enemiesAroundTarget;

   public shootBehavior ShootBehavior;
   protected RaycastHit hitData;
   public float power = 50;


   public override void Start()
   {
      blueGoalie = GameManager.Instance.BlueGoalie;  
      base.Start();
      _levels = new[] { levelOne, levelTwo, levelThree, levelFour, levelFive };
      spawnTransform.position = gameObject.transform.position;
      spawnPos = spawnTransform.position;
   }

   public enum shootBehavior
   {
      shootLeft,
      shootRight,
      shootCenter
   }

   public override void Update()
   {
      GameManager.Instance.levelIndex = levelIndex;
      base.Timer();
      Timer();
      
      if (calculate)
      {  
         ClosestLvlTransforms();
         CheckEnemiesAroundAllPossibleTargets();
      }

      if (calculate && ObjectHasBall())
      {
         CalculatePossibleMoves();
      }

      CheckIfPlayerReachedTarget();
   }

   public override void FixedUpdate()
   {
      base.FixedUpdate();
      FireRay();
      if (!tryingToIntercept && !ObjectHasBall())
      {
         transform.position = Vector3.MoveTowards(transform.position, spawnPos, GameManager.Instance.runSpeed * 1.5f); 
//         RotateTowards(spawnTransform);
      }
      else if (targetTransform != null && ObjectHasBall())
      {
         transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, GameManager.Instance.runSpeed * 1.5f);
   RotateTowards(targetTransform);
}
      checkIfCanShoot();
   }
   
   private void ClosestLvlTransforms()
   {
      for (int i = 0; i < _levels[levelIndex].Length; i++)
      {
         currentLvlTransforms = _levels[levelIndex];
      }
      counter = 0;
      correctIndexPos = 0;

      if (levelIndex == 0) //makes it so that when he first tries, he can try to go to any first position
      {
         closeLvlTransforms = new Transform[currentLvlTransforms.Length];
         closeLvlTransforms = currentLvlTransforms;
         return;
      }

      for (int i = 0; i < currentLvlTransforms.Length; i++)
      {
         if (Vector3.Distance(transform.position, currentLvlTransforms[i].position) < range)
            counter++;
      }

      closeLvlTransforms = new Transform[counter];

      for (int i = 0; i < currentLvlTransforms.Length; i++)
      {
         if (Vector3.Distance(transform.position, currentLvlTransforms[i].position) < range)
         {
            closeLvlTransforms[correctIndexPos] = currentLvlTransforms[i];
            correctIndexPos++;
         }
      }
   }
   
   void CheckEnemiesAroundAllPossibleTargets()
   {
      
      clearedCloseLvls = new List<Transform>();
      for (int i = 0; i < closeLvlTransforms.Length; i++)
      {
         Collider[] rangeChecks = Physics.OverlapSphere(closeLvlTransforms[i].position, GameManager.Instance.targetRadius, enemyMask);
            if (rangeChecks.Length == 0)
            {
               clearedCloseLvls.Add(closeLvlTransforms[i]);
            }
      }
   }
   
   //checks if enemy is near target its going towards
   void CalculatePossibleMoves()
   {
      if (clearedCloseLvls.Count == 0)
      {  
         targetTransform = null;
         calculate = false;
         return;
      }

      int lowestDistanceIndex = 0;
      float smallest = float.MaxValue;
        

      for (int i = 0; i < clearedCloseLvls.Count; i++)
      {
         float distanceBetween = Vector3.Distance(transform.position, clearedCloseLvls[i].transform.position);

            if (distanceBetween < smallest)
            {
               smallest = distanceBetween;
               lowestDistanceIndex = i;
            }

            targetTransform = clearedCloseLvls[lowestDistanceIndex];
      }
    //  targetTransform = clearedCloseLvls[randomIndex];
      
      calculate = false;
   }

   void CheckIfPlayerReachedTarget()
   {
      if (targetTransform != null && levelIndex != 4)
      {
         if (Vector3.Distance(transform.position, targetTransform.position) < .1f && targetTransform != null)
         {
            levelIndex++;
            calculate = true;
         }
      }
      else if (targetTransform != null) 
      {
         targetTransform = null;
         calculate = false;
      }
   }
   
   public override void OnCollisionEnter(Collision other)
   {
      base.OnCollisionEnter(other);
      if (other.gameObject == ballTransform.gameObject)
      {
         calculate = true;
         levelIndex = 0;
      }
   }


   void checkWhereGoalieIs()
   {
      if (Vector3.Distance(blueGoalie.transform.position,leftGoalPost.position) > Vector3.Distance(blueGoalie.transform.position, rightGoalPost.position)) // if goalie is farther from left goal post then right.
      {
         ShootBehavior = shootBehavior.shootLeft;
      }
      else
      {
         ShootBehavior = shootBehavior.shootRight;
      }
   }
   //if looking at goalie, move left or right for x seconds, check again if looking at goalie, if not, shoot forward. shoots to the side the goalie is farther from.
   void checkIfCanShoot()
   {
      if (levelIndex == 4)
      {
         if (firstInstance)
         {
            timerOn = true;
            targetRotation = centerGoal;
            targetDir = targetRotation.transform.position - transform.position;

            angle = Vector3.Angle(transform.forward, targetDir);

            if (angle > 4)
            {
               RotateTowards(targetRotation);
            }
            else
               firstInstance = false;

            return;
         }


         //sets shootBehavior based on goalie positioning.
         checkWhereGoalieIs();

         if (lookingAtGoalie())
         {
            timerOn = false;
            if (ShootBehavior == shootBehavior.shootLeft)
               targetRotation = leftGoalPost;
            if (ShootBehavior == shootBehavior.shootRight)
               targetRotation = rightGoalPost;
         }

         targetDir = targetRotation.transform.position - transform.position;
         angle = Vector3.Angle(transform.forward, targetDir);

         print(Vector3.Distance(blueGoalie.transform.position, GameManager.Instance.AIintersectPoint));
         if (Vector3.Distance(blueGoalie.transform.position, GameManager.Instance.AIintersectPoint) > distanceFromGoalie && ObjectHasBall() && angle < 10)
         {
            KickBall();
         }
         
         if (angle > 4)
         {
            RotateTowards(targetRotation);
         }
         else
         {
            if (ShootBehavior == shootBehavior.shootLeft)
            {
               ShootBehavior = shootBehavior.shootRight;
               return;
            }
         
            if (ShootBehavior == shootBehavior.shootRight)
            {
               ShootBehavior = shootBehavior.shootLeft;
            }
         }
      }
   }

   void KickBall()
   {
      GameManager.Instance._ballrb.isKinematic = false;
      GameManager.Instance.ball.transform.SetParent(null); // unchilds ball from player
      GameManager.Instance._ballrb.AddForce(gameObject.transform.forward * (power), ForceMode.Impulse);
   }
   
   void FireRay()
   {
      Ray goalieRay = new Ray(transform.position, new Vector3(transform.forward.x, transform.forward.y, transform.forward.z));
      Physics.Raycast(goalieRay, out hitData);
   }
   
   bool lookingAtGoalie()
   {
      FireRay();
      
      if (hitData.collider == null)
         return false;
      
      if (hitData.collider.gameObject.CompareTag("BlueGoalie"))
      {
         return true;
      }
      return false;
   }
   new protected void Timer()
   {
      if (timerOn && timer <= rotateTime)
      { 
         timer += Time.deltaTime;
      }

      if (!timerOn)
      {
         timer = 0;
      }
   }
}
