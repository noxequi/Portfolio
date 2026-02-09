using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCount : MonoBehaviour
{
    public float countdown = 500.0f; 
    public TextMeshProUGUI timetext; 
    public float cleartime = 0.0f; 
    public bool isrunning;

    [SerializeField] GameObject player;
    private Player chara;

    void Start()
    {
        isrunning = true;
        chara = player.GetComponent<Player>();
    }

    void Update()
    {
        if(isrunning)
        {
            countdown -= Time.deltaTime;
            timetext.text = countdown.ToString("f0");

            if(countdown <= 0)
            {
                chara.Death();
            }

            cleartime += Time.deltaTime;
        }
        
    }

    public void Stopcowntdown()
    {
        isrunning = false;
    }
    
}
