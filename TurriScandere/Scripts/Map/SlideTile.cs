using UnityEngine;

public class SlideTile : MonoBehaviour
{
    [SerializeField] private float slideRange = 3.0f; // 動く範囲
    [SerializeField] private float speed = 2.0f; // 往復する速さ
    [SerializeField] private bool isSlideHorizontal = true; // 動く方向。trueなら左右に動く。
    [SerializeField] private bool isSlideVertical = false; // 動く方向。trueなら上下に動く。
    
    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Slide();
    }

    private void Slide()
    {
        float offset = Mathf.Sin(Time.time * speed) * slideRange;

        if (isSlideHorizontal && !isSlideVertical)
        {
            transform.position = new Vector2(startPos.x + offset, startPos.y);
        }
        else if (!isSlideHorizontal && isSlideVertical)
        {
             transform.position = new Vector2(startPos.x, startPos.y + offset);
        }
    }
}