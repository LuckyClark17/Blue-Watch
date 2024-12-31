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
    
    protected Transform ballTransform;
    private Camera _camera1;
    public bool enemyAvoidance;
    public bool teammateAvoidance;
    protected Vector3 intersectPoint;
    private GameManager.BallBehavior BallBehavior;
    protected float targetClampZ;
    private float distanceToClamp;

    public GameObject[] teammates;

    private bool _timerOn;
    private float _timer;
    public bool interceptTimerOn;
    public float interceptTimer;
    public bool canIntercept = true; //true when timer
    [Header("CLAMPED POSITIONS")] [Tooltip("FIRST ELEMENT IS FOR MIN, SECOND IS FOR MAX.")]
    public float[] xClamp = new float[2];
    public float[] zClamp = new float[2]; 
    private float[][] _bounds;

    [Header("Passing and Teammates")] 
    public GameObject closestUpperTeammate;
    public GameObject _closestTeammate;
    private float _angleToKick;
    private Vector3 _targetDir;

    [Header("AI FOV")] public float fovRadius;
    public float fovAngle = 75;
    private GameObject[] playersInRadius;
    private GameObject[] playersInAIRadius;
    private bool playerInFovRadius;
    private bool playerInFov;
    private Transform _target;
    private bool movingTowardsClamp = true;

    private bool playerInAIRadius;

    [Header("Masks")] public LayerMask enemyMask;
    public LayerMask teamMask;

    protected bool tryingToIntercept;

    private PassBehavior AIBehavior = PassBehavior.PassUp;
    private bool reset;

    //starting vectors
    public Vector3 _startingPosition;
    private Quaternion _startingRotation;

    private bool startingCooldown;

    //TODO: Before passing to teammate, try to get as close as possible to temamate, then check.
    //TODO: 

    private enum PassBehavior
    {
        PassUp,
        PassToTeammate,
        RunTheBallUp
    }

    public virtual void Start()
    {
        Time.timeScale = 0;
        StartCoroutine(StartingCoroutine());
        teammateAvoidance = true;
        ballTransform = GameManager.Instance.ball.transform;
        _camera1 = Camera.main;
    }

        private void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    
    public virtual void Update()
    {
        DrawLines();
        // ChecksPlayersReset();
        // resetPos();
        ObjectsInFovRadius();
        FieldOfViewCheck();
        SetPassBehavior();
        Timer();
        TeamHasBall();
        DrawLines();
    }

    public virtual void FixedUpdate()
    {
        if(gameObject.name != "RedForward")
        fovRadius = Vector3.Distance(transform.position, closestUpperTeammate.transform.position);
        ballTransform = GameManager.Instance.ball.transform;
        //ClampPos();
        EnemyAvoidance();
        InterceptBall();
        ClosestTeammate();
    

        if(gameObject.name != "RedForward")
        {
            ReflectBall();
           MoveTowardsBall();
            TeamAvoidance();
            PassBall();
        }
    }

        IEnumerator StartingCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
    }
}
