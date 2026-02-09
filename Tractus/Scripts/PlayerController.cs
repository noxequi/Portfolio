using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    public Vector2 playerPosition;
    public float speed = 5f;
    public float stepHeight = 0.5f;
    public float footOffsetY = -0.6f;
    public float torsoOffsetY = 0.0f;
    public LayerMask GroundLayer;
    public LayerMask LineLayer;
    public int requiredKeys = 0;
    public AudioClip clearsound;
    public AudioClip deathsound;
    [HideInInspector] public bool isClear;
    [HideInInspector] public bool rightFacing;
    [SerializeField] private GameObject clearpanel;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private TMP_Text scoreText;
    private int weaponQuantity = 0;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audiosource;
    private bool menuFlag;
    private bool canMove;
    private bool death;
    private int getCoin = 0;
    private int getKey = 0;
    private int Score = 0;
    private HashSet<GameObject> processedObjects = new HashSet<GameObject>();

    async UniTask Start()
    {
        Initialize();
    }

    void Update()
    {
        if (death)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("Title");
            }
            return;
        }

        Move();
        UpdateAnimationState();
        CanMove();
    }

    private void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audiosource = GetComponent<AudioSource>();
        transform.position = playerPosition;
        rightFacing = true;
        menuFlag = false;
        canMove = true;
        death = false;
        isClear = false;
        getCoin = 0;
        getKey = 0;
        Score = 0;
        // GameOver.SetActive(false);
        // clearpanel.SetActive(false);
        UpdateUI();
    }
    private void FixedUpdate()
    {
        if (death) return;
        CheckWallBumping();
    }

    private void Move()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float moveDirection = rightFacing ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * speed, rb.linearVelocity.y);
        spriteRenderer.flipX = !rightFacing;
    }

    private void CheckWallBumping()
    {
        float checkDistance = 0.5f;
        Vector2 direction = rightFacing ? Vector2.right : Vector2.left;
        
        Vector2 torsoRayOrigin = new Vector2(transform.position.x, transform.position.y + torsoOffsetY);
        RaycastHit2D torsoHit = Physics2D.Raycast(torsoRayOrigin, direction, checkDistance, GroundLayer | LineLayer);

        if (torsoHit.collider != null)
        {
            rightFacing = !rightFacing;
            return;
        }

        Vector2 footRayOrigin = new Vector2(transform.position.x, transform.position.y + footOffsetY);
        RaycastHit2D footHit = Physics2D.Raycast(footRayOrigin, direction, checkDistance, GroundLayer | LineLayer);

        if (footHit.collider != null)
        {
            Vector2 topCheckOrigin = new Vector2(footHit.point.x + (direction.x * 0.05f), footRayOrigin.y + stepHeight);
            RaycastHit2D topHit = Physics2D.Raycast(topCheckOrigin, Vector2.down, stepHeight + 0.1f, GroundLayer | LineLayer);

            if (topHit.collider != null)
            {
                float heightDifference = topHit.point.y - footRayOrigin.y;
                if (heightDifference > 0.01f && heightDifference < stepHeight)
                {
                    float verticalVelocity = Mathf.Sqrt(2.0f * heightDifference * Mathf.Abs(Physics2D.gravity.y));
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
                }
            }
        }
    }

    private void UpdateAnimationState()
    {
        Animator animator = GetComponent<Animator>();
        if (rb.linearVelocity.x != 0)
        {
            animator.SetInteger("state", 1);
        }
        else
        {
            animator.SetInteger("state", 0);
        }

        if (rb.linearVelocity.y > 0.1f && !IsGrounded())
        {
            animator.SetInteger("state", 2);
        }
        else if (rb.linearVelocity.y < -0.1f && !IsGrounded())
        {
            animator.SetInteger("state", 3);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (death) return;
        GameObject obj = collision.gameObject;

        if (!obj.CompareTag("Goal"))
        {
            if (processedObjects.Contains(obj)) return;
            processedObjects.Add(obj);
        }

        if (obj.CompareTag("Weapon"))
        {
            weaponQuantity += 2;
            Destroy(obj);
            Score += 500;
            UpdateUI();
        }
        else if (obj.CompareTag("Enemy"))
        {
            if (weaponQuantity == 0)
            {
                Death();
            }
            else
            {
                weaponQuantity--;

                if (obj.name.Contains("StraightEnemy"))
                {
                    Score += 750;
                }
                else if (obj.name.Contains("BounceEnemy"))
                {
                    Score += 1000;
                }
                else
                {
                    Score += 500;
                }

                Destroy(obj);
                UpdateUI();
            }
        }
        else if (obj.CompareTag("EnemyAttack") || obj.CompareTag("Death"))
        {
            Death();
        }
        else if (obj.CompareTag("Coin"))
        {
            getCoin += 1;
            Destroy(obj);
            Score += 3000;
            UpdateUI();
        }
        else if (obj.CompareTag("Key"))
        {
            getKey += 1;
            Destroy(obj);
            Score += 500;
            UpdateUI();
        }
        else if (obj.CompareTag("Goal"))
        {
            Clear();
            UpdateUI();
        }
    }

    public bool IsGrounded()
    {
        CapsuleCollider2D c = GetComponent<CapsuleCollider2D>();
        return Physics2D.CapsuleCast(
            c.bounds.center,
            c.bounds.size,
            CapsuleDirection2D.Vertical,
            0f,
            Vector2.down,
            0.1f,
            GroundLayer | LineLayer
        );
    }

    private void Clear()
    {
        if (getKey >= requiredKeys)
        {
            isClear = true;
            //audiosource.PlayOneShot(clearsound);
            clearpanel.SetActive(true);
            Time.timeScale = 0f;
            if (SceneChange.currentScene == "Easy")
            {
                Score += 10000;
            }
            else if (SceneChange.currentScene == "Normal")
            {
                Score += 50000;
            }
            else if (SceneChange.currentScene == "Hard")
            {
                Score += 100000;
            }
            else
            {
                Score += 5000;
            }
        }
    }

    private void Death()
    {
        if (death) return;

        death = true;
        canMove = false;

        GetComponent<Animator>().SetInteger("state", 4);
        //audiosource.PlayOneShot(deathsound);
        GameOver.SetActive(true);

        Time.timeScale = 0f;
    }

    private void UpdateUI()
    {
        if (coinText != null) coinText.text = $"× {getCoin}";
        if (keyText != null) keyText.text = $"× {getKey}/{requiredKeys}";
        if (weaponText != null) weaponText.text = $"× {weaponQuantity}";
        if (scoreText != null) scoreText.text = $"スコア: {Score}";
    }

    private async UniTask CanMove()
    {
        if (!canMove && !death)
        {
            await UniTask.Delay(800);
            if (!death)
            {
                canMove = true;
            }
        }
    }
}