using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;

public class Player : MonoBehaviour
{
	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
    public float Speed;
    public float JumpForce;
    public float LittleJumpForce;
    public LayerMask GroundLayer;
    public int Coin = 0;
    public int hdCoin = 0;
    public int Lives = 5;
    private bool menuFlag;
    private Vector2 point;
    public bool istaken; 
    private bool Saved; 
    private bool canMove;
    private bool death;
    public int Score;
    public AudioClip jumpSound;
    public AudioClip coinsound;
    public AudioClip deathsound;
    AudioSource audiosourse;

	[HideInInspector] public bool rightFacing; 
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI livestext;
    [SerializeField] TextMeshProUGUI cointext;
    [SerializeField] TextMeshProUGUI timetext;
    [SerializeField] TextMeshProUGUI lifetext;
    [SerializeField] TextMeshProUGUI scoretext;
    [SerializeField] GameObject operation;
    [SerializeField] GameObject clearpanel;
    [SerializeField] GameObject GameOver;
    [SerializeField] GameObject[] nomalcoins;
    [SerializeField] GameObject[] hardcoins;
    [SerializeField] TimeCount timecount;
 
	async UniTask Start()
	{
        await UniTask.Delay(500);
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
        audiosourse = GetComponent<AudioSource>();
		
		rightFacing = true; 
        menuFlag = false;
        point = this.transform.position; 
        istaken = false;
        Saved = false;
        canMove = true;
        death = false;
	}
 
    void Update()
	{
		Move();
        Jump();
        UpdateAnimationState();
        CanMove();

        if(Time.timeScale == 0)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                operation.SetActive(false);
                Time.timeScale = 1;
            }
        }

        if(!istaken && !Saved)
        {
            foreach(GameObject coin in nomalcoins)
            {
                coin.SetActive(true);
            }
        }

        if(!istaken && Saved)
        {
            foreach(GameObject coin in hardcoins)
            {
                coin.SetActive(true);
            }
        }        


	}
	
	private void Move()
	{
        Speed = 5f;
		if(Input.GetKey (KeyCode.RightArrow) && canMove == true)
		{
			rightFacing = true;
 
			spriteRenderer.flipX = false;
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, rb.velocity.y);
		}
		else if(Input.GetKey (KeyCode.LeftArrow) && canMove == true)
		{
            Speed = -5f;
			rightFacing = false;
 
			spriteRenderer.flipX = true;
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, rb.velocity.y);
		}
        else
        {
            Speed = 0f;
        }
	}

    private void Jump()
	{
        if(Input.GetKey(KeyCode.LeftShift) && canMove == true)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, LittleJumpForce);
                audiosourse.PlayOneShot(jumpSound);
            }
        }
		else if(Input.GetKeyDown(KeyCode.Space) && isGrounded() && canMove == true)
		{
			rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            audiosourse.PlayOneShot(jumpSound);
        }
	}
 
	private void FixedUpdate()
	{
		Vector2 velocity = rb.velocity;
		velocity.x = Speed;
 
		rb.velocity = velocity;
	}

    public bool isGrounded()
    {
        BoxCollider2D c = GetComponent<BoxCollider2D>();
        return Physics2D.BoxCast(c.bounds.center, c.bounds.size, 0f, Vector2.down, .1f, GroundLayer);
    }

    private void UpdateAnimationState()
    {
        if (rb.velocity.x != 0)
        {
            GetComponent<Animator>().SetInteger("state", 1);
        }
        else
        {
            GetComponent<Animator>().SetInteger("state", 0);
        }

        if(rb.velocity.y > 0.1f)
        {
            GetComponent<Animator>().SetInteger("state", 2);
        }
        else if(rb.velocity.y < -0.1f)
        {
            GetComponent<Animator>().SetInteger("state", 3);
        }
    }

    private async UniTask OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        if (collision.gameObject.CompareTag("Death"))
        {
            GetComponent<Animator>().SetInteger("state", 4);
            Death();   
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.GetComponent<Collectable>() != null)
        {
            if(obj.GetComponent<Collectable>().ID == "coin_nomal")
            {
                Coin++;
                istaken = true;
                audiosourse.PlayOneShot(coinsound);
                if(istaken == true)
                {
                    obj.SetActive(false);
                }
                if(Coin < 10)
                {
                    text.SetText("0" + Coin.ToString());
                }
                else
                {
                    text.SetText(Coin.ToString());
                }
            }

            if(obj.GetComponent<Collectable>().ID == "coin_hard")
            {
                Coin++;
                hdCoin++;
                istaken = true;
                if(istaken == true)
                {
                    obj.SetActive(false);
                }
                if(Coin < 10)
                {
                    text.SetText("0" + Coin.ToString());
                }
                else
                {
                    text.SetText(Coin.ToString());
                }
            }

            if(obj.GetComponent<Collectable>().ID == "saveflag")
            {
                point = transform.position; 
                Saved = true;
                Debug.Log(point);
                GameObject.Destroy(obj);
            }

            if (obj.GetComponent<Collectable>().ID == "goal")
            {
                Score = 20000 * Coin + 150 * (int)timecount.countdown + 100000 * Lives;
                clearpanel.SetActive(true);
                canMove = false;
                timecount.Stopcowntdown();
                cointext.SetText("かくとくコイン：" + Coin.ToString());
                timetext.SetText("のこりじかん：" + ((int)timecount.countdown).ToString());
                lifetext.SetText("のこりざんき：" + Lives.ToString());
                scoretext.SetText("ごうけいスコア：" + Score.ToString());

            }
        }
    }

    public async UniTask Death()
    {
            audiosourse.PlayOneShot(deathsound);
            death = true;
            Lives -= 1;
            istaken = false;
            if(Saved == false)
            {
                Coin = 0;
                text.SetText("0" + Coin.ToString());
            }
            else if(Saved)
            {
                Coin = Coin - hdCoin;
                hdCoin = 0;
                text.SetText("0" + Coin.ToString());
            }
            if(Lives >= 0)
            {
                livestext.SetText("×0" + Lives.ToString());
            }
            if(Lives < 0)
            {
                Destroy(this.gameObject);
                GameOver.SetActive(true);
            }

            await UniTask.Delay(300);

            if(death)
            {
                transform.position = point; 
                timecount.countdown = 500.0f;
                canMove = false;
                death = false;
            }
    }

    private async UniTask CanMove()
    {
        if(!canMove)
        {
            await UniTask.Delay(800);
            canMove = true;
        }
    }

    private async UniTask LittleWait()
    {
    
        await UniTask.Delay(100);
    }
}
