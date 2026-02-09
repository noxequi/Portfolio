using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace EnemyState
{
    public enum EnemyState
    {
        UNDEFINED,
        IDLE,      
        READY,     
        ATTACK,     
        RETURN,     
    };

    public interface IEnemyState
    {
        EnemyState GetCurrentState { get; }
        void OnStateBegin();
        void OnStateEnd();
        void Update(float deltaTime);
    }

    public abstract class BaseEnemyState : IEnemyState
    {
        protected TackleEnemyBehaviour controller;
        protected EnemyState nextState = EnemyState.UNDEFINED;
        public abstract EnemyState GetCurrentState { get; }

        public BaseEnemyState(TackleEnemyBehaviour controller)
        {
            this.controller = controller;
        }

        public virtual void OnStateBegin() { nextState = EnemyState.UNDEFINED; }
        public virtual void OnStateEnd() { }
        public virtual void Update(float deltaTime) { }

        protected void SetNextState(EnemyState next)
        {
            nextState = next;
            controller.RequestStateChange(next);
        }
    }

    public class IdleState : BaseEnemyState
    {
        private float timer;
        private float currentAngle;

        public override EnemyState GetCurrentState => EnemyState.IDLE;
        
        public IdleState(TackleEnemyBehaviour controller) : base(controller) { }

        public override void OnStateBegin()
        {
            base.OnStateBegin();
            timer = 0f;

            var player = controller.Player;
            if (player != null)
            {
                Vector3 direction = controller.transform.position - player.position;
                currentAngle = Mathf.Atan2(direction.y, direction.x);
            }
            else
            {
                currentAngle = Random.Range(0f, 360f); 
            }
        }

        public override void Update(float deltaTime)
        {
            var player = controller.Player;
            if (player == null) return;

            timer += deltaTime;

            // プレイヤーの周りを円運動
            currentAngle += 1.5f * deltaTime;
            float radius = 5.0f;
            Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0) * radius;
            Vector3 targetPos = player.position + offset;

            controller.transform.position = Vector3.Lerp(
                controller.transform.position, 
                targetPos, 
                2.0f * deltaTime
            );

            float dirX = player.position.x - controller.transform.position.x;
            controller.FaceTo(dirX);

            if (timer >= controller.AttackInterval)
            {
                SetNextState(EnemyState.READY);
            }
        }
    }

    public class ReadyState : BaseEnemyState
    {
        private float timer;
        private float readyDuration = 1.0f; 
        private float lockOnTime = 0.1f; 

        public override EnemyState GetCurrentState => EnemyState.READY;
        public ReadyState(TackleEnemyBehaviour controller) : base(controller) { }

        public override void OnStateBegin()
        {
            base.OnStateBegin();
            timer = 0f;
            controller.SetColor(Color.red);
        }

        public override void Update(float deltaTime)
        {
            timer += deltaTime;

            var player = controller.Player;
            if (player != null)
            {
                if (timer < readyDuration - lockOnTime)
                {
                    float dirX = player.position.x - controller.transform.position.x;
                    controller.FaceTo(dirX);
                }
            }

            if (timer >= readyDuration)
            {
                SetNextState(EnemyState.ATTACK);
            }
        }

        public override void OnStateEnd()
        {
            controller.SetColor(Color.white);
            base.OnStateEnd();
        }
    }

    public class AttackState : BaseEnemyState
    {
        private float timer;
        private Vector3 attackDirection;
        private bool isAttacking;

        public override EnemyState GetCurrentState => EnemyState.ATTACK;
        public AttackState(TackleEnemyBehaviour controller) : base(controller) { }

        public override void OnStateBegin()
        {
            base.OnStateBegin();
            timer = 0f;
            isAttacking = true;

            var player = controller.Player;
            if (player != null)
            {
                Vector3 offset = new Vector3(0, -0.5f, 0);
                Vector3 targetPos = player.position + offset;
                attackDirection = (targetPos - controller.transform.position).normalized;
                controller.Attack(); 
            }
            else
            {
                SetNextState(EnemyState.RETURN); 
            }
        }

        public override void Update(float deltaTime)
        {
            if (!isAttacking) return;

            timer += deltaTime;

            float dashSpeed = controller.chaseSpeed * 1.5f;
            controller.transform.position += attackDirection * dashSpeed * deltaTime;

            if (timer >= 1.0f)
            {
                SetNextState(EnemyState.RETURN);
            }
        }
    }

    public class ReturnState : BaseEnemyState
    {
        public override EnemyState GetCurrentState => EnemyState.RETURN;
        public ReturnState(TackleEnemyBehaviour controller) : base(controller) { }

        public override void OnStateBegin()
        {
            base.OnStateBegin();
        }

        public override void Update(float deltaTime)
        {
            var player = controller.Player;
            if (player == null)
            {
                SetNextState(EnemyState.IDLE);
                return;
            }

            Vector3 targetPos = player.position + new Vector3(0, 4.0f, 0);
            float dist = Vector3.Distance(controller.transform.position, targetPos);
            float returnSpeed = controller.chaseSpeed * 0.75f;
            
            controller.transform.position = Vector3.MoveTowards(
                controller.transform.position, 
                targetPos, 
                returnSpeed * deltaTime
            );

            float dirX = player.position.x - controller.transform.position.x;
            controller.FaceTo(dirX);

            if (dist < 1.0f)
            {
                SetNextState(EnemyState.IDLE);
            }
        }
        
        public override void OnStateEnd()
        {
            base.OnStateEnd();
        }
    }

    public class TackleEnemyBehaviour : BaseEnemyBehaviour, IEnemyPausable
    {
        private Dictionary<EnemyState, IEnemyState> states = new Dictionary<EnemyState, IEnemyState>();
        private IEnemyState currentState;
        private Rigidbody2D rb;

        [SerializeField] private Transform _player;
        [SerializeField] private float _chaseSpeed = 5.0f; 
        [SerializeField] private float _attackInterval = 3.0f; 

        public Transform Player => _player;
        public float chaseSpeed => _chaseSpeed;
        public float AttackInterval => _attackInterval;

        public UnityEvent OnAttack;

        [SerializeField] private AudioClip attackSound;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        AudioSource audiosourse; 

        public void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (_player == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p) _player = p.transform;
            }

            states.Add(EnemyState.IDLE, new IdleState(this));
            states.Add(EnemyState.READY, new ReadyState(this)); 
            states.Add(EnemyState.ATTACK, new AttackState(this));
            states.Add(EnemyState.RETURN, new ReturnState(this)); // 追加

            ChangeState(EnemyState.IDLE);

            audiosourse = GetComponent<AudioSource>();
            if (audiosourse == null)
            {
                audiosourse = gameObject.AddComponent<AudioSource>();
            }

            if (sfxMixerGroup != null)
            {
                audiosourse.outputAudioMixerGroup = sfxMixerGroup;
            }
        }

        public void Update()
        {
            if (!this.enabled) return;
            currentState?.Update(Time.deltaTime);
        }

        public void RequestStateChange(EnemyState next)
        {
            ChangeState(next);
        }

        private void ChangeState(EnemyState next)
        {
            currentState?.OnStateEnd();
            if (states.ContainsKey(next))
            {
                currentState = states[next];
                currentState.OnStateBegin();
            }
        }

        public void FaceTo(float directionX)
        {
            if (Mathf.Abs(directionX) < 0.01f) return;
            Vector3 scale = transform.localScale;
            scale.x = directionX > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        public void Attack()
        {
            OnAttack?.Invoke();
            audiosourse.PlayOneShot(attackSound);
        }
        
        public void SetColor(Color color)
        {
            GetComponent<SpriteRenderer>().color = color;
        }

        public void OnPause()
        {
            this.enabled = false; 
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.simulated = false;       
            }
        }

        public void OnResume()
        {
            this.enabled = true; 
            if (rb != null)
            {
                rb.simulated = true; 
            }
        }
    }
}