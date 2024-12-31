using System;
using System.Numerics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.XR;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class BallScript : MonoBehaviour
{ 
    [SerializeField] [Range(0,100)] private float power = 50;
    
    private float _timer;
    private bool _timerOn;
    
    private Camera _mainCamera;
    private Transform _cameraTransform;
    public Transform _playerTransform;
    private Rigidbody _ballrb;
    private Vector3 shootingDirection;
    
    
    private bool _cancelShot;
     
     
    [SerializeField] GameObject releasePosition;
    
    [SerializeField] private LineRenderer lineRenderer;
    private Transform _releasePosition;

    [Header("Display Controls")]
    [SerializeField] [Range(10, 100)]
    private int linePoints = 25;
    [SerializeField] [Range(0.01f, 0.25f)] private float timeBetweenPoints = 0.1f;
    

    //TODO:ADD DIFFERENT TYPES OF SHOOTING (ANGLED, STRAIGHT, PARABOLA) different power / speed depending on type of shooting 

    // Update is called once per frame
    private void Start()
    {
        _mainCamera = Camera.main;
        _ballrb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _cameraTransform = _mainCamera.transform;
        ClampedShootingDirection();
        Timer();
        KickBall();
        DrawProjection();
    }
    
/*
 * Method for actualphysics of kicking.
 */
    private void KickBall()
    {        // maybe add different types of kicking?
        
        if ((Input.GetMouseButton(0)) &&  Application.isFocused && transform. root != transform && !Input.GetMouseButton(1))
        {
            
            GameManager.Instance.playerChargingBall = true;   
            lineRenderer.enabled = true;
            _timerOn = true;
        }

        if (Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.playerChargingBall = false;
            _cancelShot = true;
            lineRenderer.enabled = false;
            _timerOn = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _cancelShot = false;
        }

        else if (Input.GetMouseButtonUp(0) && Application.isFocused && transform.root != transform && !_cancelShot)
        {
            GameManager.Instance.playerChargingBall = false;
            _ballrb.isKinematic = false;
            gameObject.transform.SetParent(null); // unchilds ball from player
            print(_timer);
            _ballrb.AddForce(shootingDirection * (power * _timer), ForceMode.Impulse);  // projects the ball forward relative to the camera's forward by x power and x time held down.
            _timerOn = false;
            _cancelShot = false;
            lineRenderer.enabled = false;
            GameManager.Instance.ballHeldByPlayer = false;
            GameManager.Instance.playerWhoKicked = GameManager.Instance.player;
        }
    }
    
    
    
    private void DrawProjection()
    {
        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
        Vector3 startPosition = transform.position;
        Vector3 startVelocity = shootingDirection * (power * _timer);
        int i = 0;
        lineRenderer.SetPosition(i, startPosition);
        
       // time equals time from when it is released. point.y = the displacement from start to finish on the y axis
         for (float time = 0; time < linePoints; time += timeBetweenPoints)
         {
             i++;
             Vector3 point = startPosition + time * startVelocity;
             point.y = startPosition.y + startVelocity.y * time +   (Physics.gravity.y/2f  * Mathf.Pow(time, 2));
             
             lineRenderer.SetPosition(i, point);
         }
    }
    Vector3 GetDirectionWithinLimit(Vector3 initialDirection, Vector3 targetDirection, float maxAngle)
    {
        
        //gets angle between the player and the camera. 
        float angle = Vector3.Angle(initialDirection, targetDirection);
        // If angle is within the limit, 
        if (angle <= maxAngle)
        {
            return targetDirection;
        }

        // Factor will be 0 - 1 if angle > maxAngle
        // Otherwise factor will be > 1 but Slerp clamps it to 1 anyway
        float factor = maxAngle / angle;
        return Vector3.Slerp(_playerTransform.forward, targetDirection, factor);
    }
    
    void ClampedShootingDirection()
    {
        var playerAngle = _playerTransform.forward;
        //var camAngle = _cameraTransform.forward;
        var raisedCamAngle = new Vector3(_cameraTransform.forward.x, _cameraTransform.forward.y + .2f, _cameraTransform.forward.z);

        shootingDirection = GetDirectionWithinLimit(playerAngle, raisedCamAngle, 30);
    }
    
    void Timer()
    {
        if (_timerOn && _timer <= 1 )
        {
            _timer += Time.deltaTime;
        }
        if(!_timerOn)
        {
            _timer = .4f;
            
        }
    }
}
