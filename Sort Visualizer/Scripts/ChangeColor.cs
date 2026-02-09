using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    public Image image; 
    public FlexibleColorPicker fcp;

    void Start()
    {
        
    }

    void Update()
    {
        image.color = fcp.color;
    }
}
