using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    [SerializeField] private GameObject ColorPicker;

    void Start()
    {
        ColorPicker.SetActive(false);
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            ColorPicker.SetActive(false);
        }
    }

    public void onClick_Color()
    {
        ColorPicker.SetActive(true);
    }
}
