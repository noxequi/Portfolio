using UnityEngine;
using TMPro;
using UnityEngine.Audio; // 【追加】これが必要です

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    //プレイヤー設定
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float smallJumpForce = 3.0f; //小ジャンプ

    public float moveInput { get; private set; } 
    public LayerMask groundLayer;

    // bool値
    private bool rightFacing = true;
    private bool isJumping = false;
    private bool isDoubleJumped = false;

    [SerializeField] private float floorHeight = 20.0f; // TowerGeneratorの設定と合わせてください
    [SerializeField] private int scorePerFloor = 100;   // 1階上がるごとのスコア
    [SerializeField] private int coinScore = 50; // コインをとった時のスコア
    [SerializeField] private int numberOfKey = 0; //持っている鍵の個数
    
    private int currentScore = 0;
    private int highestFloorReached = 0; // これまで到達した最高階層

    public int CurrentScore => currentScore; 

    // UI
    [SerializeField] private TextMeshProUGUI scoreText;

    // SE
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip coinSound;
    
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    AudioSource audiosourse; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audiosourse = GetComponent<AudioSource>();
        if (audiosourse == null)
        {
            audiosourse = gameObject.AddComponent<AudioSource>();
        }

        if (sfxMixerGroup != null)
        {
            audiosourse.outputAudioMixerGroup = sfxMixerGroup;
        }

        rightFacing = true;
        isJumping = false;
        isDoubleJumped = false;

        highestFloorReached = 0;
        UpdateScoreUI();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
        Move();
        Jump();
        UpdateAnimationState();
        CheckFloorProgress();
    }

    private void CheckFloorProgress()
    {
        int currentFloor = Mathf.FloorToInt(transform.position.y / floorHeight);

        // 今の階が過去の最高到達階より高い場合のみスコア加算
        if (currentFloor > highestFloorReached)
        {
            int gainedFloors = currentFloor - highestFloorReached;
            
            if (gainedFloors > 0)
            {
                currentScore += gainedFloors * scorePerFloor;
                highestFloorReached = currentFloor;
                UpdateScoreUI();
            }
        }
    }

    public void Move()
    {
        if (moveInput > 0)
        {
            if (!rightFacing)
            {
                Flip();
            }

            rb.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.linearVelocity.y);
        }
        else if (moveInput < 0)
        {
            if (rightFacing)
            {
                Flip();
            }

            rb.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.linearVelocity.y);
        }
        else 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    } 

    public void Jump()
    {
        if (isGround() && rb.linearVelocity.y <= 0)
        {
            isDoubleJumped = false;
            isJumping = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGround())
            {
                isJumping = true;
                float force = Input.GetKey(KeyCode.LeftShift) ? smallJumpForce : jumpForce;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
                audiosourse.PlayOneShot(jumpSound, 0.6f);
            }
            else if (isJumping && !isDoubleJumped)
            {
                isDoubleJumped = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                audiosourse.PlayOneShot(jumpSound, 0.6f);
            }
        }
    }

    private void Flip()
    {
        rightFacing = !rightFacing;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public bool isGround()
    {
        CapsuleCollider2D c = GetComponent<CapsuleCollider2D>();
        return Physics2D.CapsuleCast(
            c.bounds.center,
            c.bounds.size,
            CapsuleDirection2D.Vertical,
            0f,
            Vector2.down,
            0.1f,
            groundLayer
        );
    }

    private void UpdateAnimationState()
    {
        if (rb.linearVelocity.x != 0)
        {
            GetComponent<Animator>().SetInteger("state", 1);
        }
        else
        {
            GetComponent<Animator>().SetInteger("state", 0);
        }

        if(rb.linearVelocity.y > 0.1f)
        {
            GetComponent<Animator>().SetInteger("state", 2);
        }
        else if(rb.linearVelocity.y < -0.1f)
        {
            GetComponent<Animator>().SetInteger("state", 3);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Coin"))
        {
            currentScore += coinScore;
            UpdateScoreUI();
            Destroy(obj);
            audiosourse.PlayOneShot(coinSound, 0.6f);
        }
        else if (obj.CompareTag("Key"))
        {
            numberOfKey++;
            Destroy(obj);
        }
        else if (obj.CompareTag("Bonus"))
        {
            if (numberOfKey > 0)
            {
                numberOfKey--;
                Destroy(obj);
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"スコア： {currentScore}";
        }
    }
}