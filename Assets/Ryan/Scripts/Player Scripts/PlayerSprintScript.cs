using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Image = UnityEngine.UI.Image;


namespace PlayerScripts.Helpers
{
    public class PlayerSprintScript : MonoBehaviour
    {
        [Header("Sprint")]

        [HideInInspector] public bool canSprint;
        private float timeUntilSprint = 2;
        private bool sprintKeyPressed;
        private bool sprintTimerOn;
        [SerializeField][Range(0, 5)] public float sprintMultiplier = 2;

        //fields for timer
        private bool _timerOn;
        [SerializeField][Range(0, 5)] private float sprintDelay = 2;
        private float _timer;

        [Header("Stamina")]
        [HideInInspector] public float stamina;
        [SerializeField][Range(0, 15)] private float maxStamina = 10;
        [SerializeField][Range(0, 5)] private float staminaRegenMultiplier = 1.25f;
        private float _staminaRegenRate;

        [SerializeField] private Sprite[] chargeSprites;
        [SerializeField] private Image chargeBarSpriteRenderer;
        private int staminaBarLevel;
        private Rigidbody _playerRb;

        public void Start()
        {
            stamina = maxStamina;
            _playerRb = GetComponent<Rigidbody>();
            staminaBarLevel = chargeSprites.Length - 1;
        }

        public void Update()
        {
            sprintKeyPressed = Input.GetKey(KeyCode.LeftShift);

            Timer();
            Sprinting();
            StaminaBar();
        }

        void Sprinting()
        {
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);
            timeUntilSprint = Mathf.Clamp(timeUntilSprint, 0f, sprintDelay);
            _timer = Mathf.Clamp(_timer, 0, 1);

            _staminaRegenRate = staminaRegenMultiplier * Time.deltaTime;

            //if you click sprint key for even a second it starts the timer before you can activate another sprint.
            if (_playerRb.velocity.magnitude > .1f && sprintKeyPressed)
                sprintTimerOn = true;

            // when you first click sprint key, if you have no delay before next sprint returns true.
            if (Input.GetKeyDown(KeyCode.LeftShift) && timeUntilSprint == 0f)
            {
                canSprint = true;
            }
            //when you let go of sprint, and you have a delay before you can sprint again, you can no longer sprint until delay is done.
            if (Input.GetKeyUp(KeyCode.LeftShift) && timeUntilSprint != 0)
            {
                canSprint = false;
            }

            //STAMINA SYSTEM
            {
                //if you have stamina, and you are sprinting, stamina bar goes down.
                if (stamina > 0 && canSprint && _playerRb.velocity.magnitude > .1f)
                {
                    stamina -= Time.deltaTime;
                    _timerOn = true;
                }

                //stamina regenerates when delay is over, you're not running, and when you haven't reached stamina threshold
                if (!sprintKeyPressed && stamina < maxStamina && timeUntilSprint == 0)
                {
                    stamina += _staminaRegenRate;
                    _timerOn = false;
                }
            }
        }

        public void Timer()
        {
            if (_timerOn && _timer <= 1)
            {
                _timer += Time.deltaTime;
            }

            if (!_timerOn && _timer != 0)
            {
                _timer -= Time.deltaTime;
            }

            //TIMER TURNS ON WHEN YOU HOLD SHIFT, TURNS OFF WHEN TIME REACHES DELAY LIMIT

            //if you try and sprint while the time has not reset, will reset timer to zero. (since timer is still on, will keep adding time unlike last if statement)
            if (sprintKeyPressed && timeUntilSprint != 0)
            {
                timeUntilSprint = 0;
            }

            //if you've waited until sprint delay and you're not sprinting, resets timeUntilSprint
            if (timeUntilSprint == sprintDelay && !sprintKeyPressed)
            {
                sprintTimerOn = false;
            }

            switch (sprintTimerOn)
            {
                // adds to time when timer is on, which is when you click sprint button.
                //when timer is on, and when time is less than two, add to time
                case true when timeUntilSprint < sprintDelay:
                    timeUntilSprint += Time.deltaTime; break;
                //runs when timer turns off (when time = max delay), resets time to zero.
                case false:
                    timeUntilSprint = 0f;
                    break;
            }
        }

        public void StaminaBar()
        {
            //staminaToChanged is so the stamina is in the same ratio to the amount of sprites (5). 
            float staminaToChargeRatio = (chargeSprites.Length - 1) / maxStamina;
            float staminaToCharged = stamina * staminaToChargeRatio;

            if (Mathf.Floor(staminaToCharged) > staminaBarLevel && staminaBarLevel != 5)
            {
                staminaBarLevel++;
            }
            else if (Mathf.Ceil(staminaToCharged) < staminaBarLevel && staminaBarLevel != 0)
            {
                staminaBarLevel--;
            }
            chargeBarSpriteRenderer.sprite = chargeSprites[staminaBarLevel];
        }
    }
}