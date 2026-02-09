using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StraightBullet : MonoBehaviour
{
    public float horizontalSpeed = 5f;

    private Rigidbody2D rb;
    private int groundLayer;
    private int lineLayer;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        groundLayer = LayerMask.NameToLayer("Ground");
        lineLayer = LayerMask.NameToLayer("Line");
    }

    void Start()
    {
        float direction = spriteRenderer.flipX ? 1f : -1f;
        rb.linearVelocity = new Vector2(horizontalSpeed * direction, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layerOfCollision = collision.gameObject.layer;

        if (layerOfCollision == groundLayer || layerOfCollision == lineLayer)
        {
            Destroy(gameObject);
        }
    }
}
