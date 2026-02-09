using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_Start()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnClick_Title()
    {
        SceneManager.LoadScene("Title");
    }
}
