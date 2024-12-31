using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    
    public void StartGameFunction()
    {
        SceneManager.LoadScene(1);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
