using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class InfiniteCeiling : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private float bufferDistance = 50.0f;

    private PolygonCollider2D polyCollider;

    void Start()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    void LateUpdate()
    {
        if (player == null || polyCollider == null) return;

        Vector2[] points = polyCollider.points;

        float maxY = -Mathf.Infinity;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y > maxY)
            {
                maxY = points[i].y;
            }
        }

        if (player.position.y > maxY - bufferDistance)
        {
            ExtendCeiling(points, maxY);
        }
    }

    void ExtendCeiling(Vector2[] points, float currentMaxY)
    {
        float newHeight = player.position.y + bufferDistance + 100.0f;

        for (int i = 0; i < points.Length; i++)
        {
            if (Mathf.Abs(points[i].y - currentMaxY) < 1.0f) 
            {
                points[i].y = newHeight;
            }
        }

        polyCollider.points = points;
    }
}