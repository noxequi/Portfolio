using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Blinker : MonoBehaviour
{
    public float speed = 1.0f;
    private TextMeshProUGUI text;
    private float time;

    void Start()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.color = GetAlphaColor(text.color);
    }

    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time);

        return color;
    }
}
