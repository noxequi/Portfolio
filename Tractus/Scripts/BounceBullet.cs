using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BounceBullet : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public int maxBounces = 5;
    public float bounciness = 0.95f; 

    private Rigidbody2D rb;
    private int bounceCount = 0;
    private int lineLayer;
    private int groundLayer;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastVelocity; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineLayer = LayerMask.NameToLayer("Line");
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    void Start()
    {
        float direction = spriteRenderer.flipX ? 1f : -1f;
        rb.linearVelocity = new Vector2(horizontalSpeed * direction, 0f);
    }

    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        if (collisionLayer == lineLayer || collisionLayer == groundLayer)
        {
            if (Mathf.Abs(collision.contacts[0].normal.y) > 0.5f)
            {
                bounceCount++;
            }
            
            if (bounceCount > maxBounces)
            {
                Destroy(gameObject);
                return;
            }

            Vector2 reflectedVelocity = Vector2.Reflect(lastVelocity, collision.contacts[0].normal);
            rb.linearVelocity = reflectedVelocity * bounciness;
        }
    }
}