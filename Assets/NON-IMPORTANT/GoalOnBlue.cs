using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalOnBlue : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI redWinDisplay;
    public TextMeshProUGUI redScoreboard1;
    public TextMeshProUGUI redScoreboard2;

    //ints for red and score, will be used from game manager
    
    void Start()
    {
        //setting win displau oimactive for the first part of threg am e
    }


    public void RedWinsGame()
    {
        //10 seconds to take in victory 
        
        //all players reset to original positions and scores are reset to zero, along with scoreboards
        GameManager.Instance.redScore = 0;
        GameManager.Instance.blueScore = 0; 
        redScoreboard1.SetText(GameManager.Instance.redScore + "");
        redScoreboard2.SetText(GameManager.Instance.redScore + "");
        
    }
    
    
    //checks if ball collides (triggers) with blue goal 
    private void OnTriggerEnter(Collider col)
    {
        //MEXICO SCORES
        if (col.CompareTag("Ball"))
        {
            print("Red scores wey");
           
            //display "score goal" text for RED TEAM
            GameManager.Instance.redScore++;
            redScoreboard1.SetText(GameManager.Instance.redScore + "");
            redScoreboard2.SetText(GameManager.Instance.redScore + "");
            
            StartCoroutine(Courentine());
            //game resets after 5 second delay
            GameManager.Instance.shouldJump = true;
             
            
            //checks if they have reached 5 goals 
            if (GameManager.Instance.redScore == 10)
            {
                //red WINS
            }
            

        }
        
    }
    
    //red score text appears
    public void DisplayRedScore()
    {
    }


    IEnumerator Courentine()
    {
        yield return new WaitForSeconds(5f);
        print("SCORED");
        GameManager.Instance.scored = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
