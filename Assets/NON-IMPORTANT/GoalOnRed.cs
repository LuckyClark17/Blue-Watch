using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class GoalOnRed : MonoBehaviour
{
    // Start is called before the first frame update

    private float _timer;
    private bool _timerOn;

    public TextMeshProUGUI blueWinDisplay; 
    public TextMeshProUGUI blueScoreboard1;
    public TextMeshProUGUI blueScoreboard2;
    //score vars used from game manager
    
    void Start()
    {
        //setting display inactive at first


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
    
    
    public void BlueWinsGame()
    {
       
        
        //all players reset to original positions
        
        
        //scores are reset to zero, along with scoreboards
        GameManager.Instance.redScore = 0;
        GameManager.Instance.blueScore = 0; 
        blueScoreboard1.SetText(GameManager.Instance.blueScore + "");
        blueScoreboard2.SetText(GameManager.Instance.blueScore + "");
        //the scores in the red score are also rest
        
        //10 seconds to take in victory 
        StartCoroutine(Courentine());
        
    } 
    
    //checks if ball collides (triggers) with blue goal 
    private void OnTriggerEnter(Collider col)
    {
        //ONCE BALL COLLIDES
        if (col.CompareTag("Ball"))
        {
            //once blue has reached 10 goals, they win
            if (GameManager.Instance.blueScore == 10)
            {
                //blue wins
                BlueWinsGame();
            }
            else
            {
                //USA SCORES
                //display "score goal" text for RED TEAM
                GameManager.Instance.blueScore++;
                blueScoreboard1.SetText(GameManager.Instance.blueScore + "");
                blueScoreboard2.SetText(GameManager.Instance.blueScore + "");
           
                StartCoroutine(Courentine());

            }
            
            
            
            //game resets after five second delay
            //
            

        }
        
    }

    IEnumerator Courentine()
    {
        yield return new WaitForSeconds(5f);
        GameManager.Instance.scored = true;
    }

    // Update is called once per frame
    void Update()
    {
        Timer();


    }
}
