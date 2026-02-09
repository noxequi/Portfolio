using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operation : MonoBehaviour
{
    [SerializeField] GameObject operation;
    public bool menuFlag;
    
    void Start()
    {
        menuFlag = false;
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

            if(obj.GetComponent<Collectable>().ID == "Player")
            {
                if(!menuFlag)
                {
                    operation.SetActive(true);
                    menuFlag = true;
                    Time.timeScale = 0;
                }  

                if(Time.timeScale == 0 && menuFlag == true)
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        operation.SetActive(false);
                        menuFlag = false;
                        Time.timeScale = 1;
                    }
                }
            }
    }
}
