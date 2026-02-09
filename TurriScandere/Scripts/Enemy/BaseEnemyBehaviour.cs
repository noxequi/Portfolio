using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour, IEnemyPausable
{
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRangeX = 10.0f;
    [SerializeField] private float detectionRangeY = 2.0f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);

        if (distanceX <= detectionRangeX && distanceY <= detectionRangeY)
        {
            FaceToPlayer();
        }
    }

    void FaceToPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false; 
        }
        else if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void OnPause()
    {
        this.enabled = false;
    }

    public void OnResume()
    {
        this.enabled = true;
    }
}