using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Image = UnityEngine.UI.Image;


namespace PlayerScripts.Helpers
{
    public class PlayerMovementScript : MonoBehaviour
    {
        private Camera _mainCamera;
        private Rigidbody _playerRb;
        private float horizontal_input;
        private float vertical_input;

        //fields for movement of player
        private Transform _cameraTransform;
        private Vector3 _cameraRelativeMovement;

        PlayerSprintScript SprintScript;
        

        [SerializeField][Range(0, 500)] private float defaultSpeed = 100;
        [SerializeField][Range(0, 5)] public float sprintMultiplier = 2;

        private float _turnSmoothVelocity;
        private readonly float _turnSmoothTime = 0.1f;

        [HideInInspector] public bool isSprinting;


        // Start is called before the first frame update
        public void Start()
        {
            SprintScript = GetComponent<PlayerSprintScript>();
            _playerRb = GetComponent<Rigidbody>();
            _mainCamera = Camera.main;
        }

        // Update is called once per frame
        public void Update()
        {
            horizontal_input = Input.GetAxisRaw("Horizontal");
            vertical_input = Input.GetAxisRaw("Vertical");
            _cameraTransform = _mainCamera.transform;
        }

        public void FixedUpdate()
        {
            FreezeRotations();
            // if player is moving 
            if (horizontal_input != 0 || vertical_input != 0)
            {
                CameraBasedRotation();
                if (!GameManager.Instance.playerChargingBall)
                {
                    CameraBasedMovement();
                }
                else

                {
                    _playerRb.velocity /= 1.2f;
                }

            }
        }

        void CameraBasedRotation()
        {
            var angle = Mathf.Atan2(_cameraRelativeMovement.x, _cameraRelativeMovement.z) *
                        Mathf.Rad2Deg; // uses inverse tangent to get the characters angle from its forward position
            var smoothAngle =
                Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref _turnSmoothVelocity,
                    _turnSmoothTime); // uses smooth damp angle to smooth out the rotation. 
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        public void CameraBasedMovement()
        {
            //gets the vertical and horizontal components of the camera transform
            var cameraForwardTransform = _cameraTransform.forward.normalized;
            var cameraRightTransform = _cameraTransform.right.normalized;
            //if its not zero, when you look up, the character will slow down. Y component has to be zero for the camera based movement to work.
            cameraForwardTransform.y = 0;
            cameraRightTransform.y = 0;

            var forwardRelativeVerticalInput = cameraForwardTransform * (vertical_input * defaultSpeed);
            var rightRelativeVerticalInput = cameraRightTransform * (horizontal_input * defaultSpeed);
            _cameraRelativeMovement = rightRelativeVerticalInput.normalized + forwardRelativeVerticalInput.normalized;

            //if you have stamina and checks if you have no delay before next sprint (see line 119)
            if (SprintScript.stamina > 0 && SprintScript.canSprint)
            {
                _playerRb.velocity = _cameraRelativeMovement * (defaultSpeed * sprintMultiplier * Time.deltaTime);
                isSprinting = true;
            }
            else
            {
                _playerRb.velocity = _cameraRelativeMovement * (defaultSpeed * Time.deltaTime);
                isSprinting = false;
            }
        }

        void FreezeRotations()
        {
            _playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

//             void FireRay()
//     {
//         Ray goalieRay = new Ray(transform.position, new Vector3(transform.forward.x, 0, transform.forward.z));
//         Physics.Raycast(goalieRay, out hitData);
// //        print(hitData.collider.tag);
//     }
    }
}