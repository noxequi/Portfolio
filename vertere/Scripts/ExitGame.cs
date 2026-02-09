using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Endgame();
    }

    void Endgame()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Title")
        {
            Application.Quit();
        }
        else if(Input.GetKeyDown(KeyCode.Q) && SceneManager.GetActiveScene().name == "Main")
        {
            Application.Quit();
        }
    }
}
