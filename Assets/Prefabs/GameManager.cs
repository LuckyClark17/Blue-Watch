using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject ball;
    public Rigidbody _ballrb;
    public float AIShotPower;
    [SerializeField] [Range(0,100)]
    public int aiRadius;
    [SerializeField] [Range(0,100)] 
    public int targetRadius;
    public float radiusFactor;
    [SerializeField] [Range(0, 0.2f)] public float goalieSpeed = .12f;
    [SerializeField] [Range(0, 1)] public float runSpeed;
    public GameObject objectThatHasBall;
    public GameObject playerWhoKicked;

    public Vector3 AIintersectPoint;
    
    //booleans for all players being reset
    public bool allPlayersReset; 
    

    public int redScore;
    public int blueScore;
    public bool scored;
    public bool allHaveReset;
    
    public bool ballHeldByAI;
    public bool teamHasBall;
    public bool ballHeldByPlayer;
    public bool playerChargingBall;
    
    [Header("AI Passing Variables")]
    [SerializeField] [Range(0, 2)] public float delayBeforeKick;
    [SerializeField] [Range(10, 500)] public float AITurnSpeed;

    [SerializeField] [Range(0, 10)] public float interceptCooldownTimer;
    [SerializeField] [Range(0, 10)] public float reflectSpeed;

    [Header("Red Players")]
    public GameObject RedGoalie;
    public GameObject[] RedDefenders;
    public GameObject[] RedMidFielders;
    public GameObject RedStriker;

    public float levelIndex;

    [Header("Blue Players")]
    public GameObject BlueGoalie;
    public GameObject[] BlueDefender;
    public GameObject[] BlueMidFielders;
    public GameObject BlueStriker;
    
    [SerializeField] public BallBehavior ballBehavior;

    //[HideInInspector]
    public bool shouldJump;
    [Header("Audience")]

    public int AudijumpForce;


    private void Start()
    {
        Cursor.lockState = !Input.GetKey(KeyCode.R) ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true; 
    }

    public enum BallBehavior
    {
        RedDefSpawn,
        BlueDefSpawn,
        RedMidSpawn,
        BlueMidSpawn,
        Middle
    }

    private void Update()
    {
       
        if (shouldJump)
            shouldJump = false;
        switch (ball.transform.position.z)
        {
            case > 2.5f and < 37.5f:
                ballBehavior = BallBehavior.Middle;
                break;
            case < 2.5f and > -10f:
                ballBehavior = BallBehavior.RedMidSpawn;
                break;
            case < 10f:
                ballBehavior = BallBehavior.RedDefSpawn;
                break;
            case > 37.5f and < 50f:
                ballBehavior = BallBehavior.BlueMidSpawn;
                break;
            case > 50f:
                ballBehavior = BallBehavior.BlueDefSpawn;
                break;
        }
    }

    
    private void Awake()
    {
        //Handles the first time run case
        if (Instance == null)
        {
            //Unity will not destroy the gameObject
            //that this script is attached to.
            //Since the gameObject will live then
            //this instance of the script lives as well.
            DontDestroyOnLoad(gameObject);
            
            //Sets Instance to this (the current running copy 
            //of the GameManger script)
            Instance = this;
        }
        else if (Instance != this)
        {
            //Handles every other time the script
            //is run. If Instance is not the first
            //occurrence of the GameMange script
            //it destroys that duplicate object.
            Destroy(gameObject);
        }
    }
    
}
