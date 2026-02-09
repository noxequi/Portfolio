using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gaming : MonoBehaviour
{

    public GameObject gameobject;

    void Start()
    {
        
    }

    void Update()
    {
        float transitionSpeed = 0.3f;
        gameobject.GetComponent<TextMeshProUGUI>().color = Color.HSVToRGB(Time.time * transitionSpeed % 1, 1, 1);
    }
}
