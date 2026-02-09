using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Audio;
using unityroom.Api;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private float _maxHP = 3.0f;
    [SerializeField] private float _hp = 5.0f;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private PlayerController playerController;
    
    // 無敵時間の設定
    [SerializeField] private float damageCooldown = 1.0f; 
    private bool isInvincible = false; // 無敵中かどうか
    [SerializeField] private float flashInterval = 0.05f;

    public float MaxHP => _maxHP;
    public float HP => _hp;
    public event Action<float, float> OnHPChanged;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    AudioSource audiosourse; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _hp = _maxHP;
        OnHPChanged?.Invoke(_hp, _maxHP);

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible) return;

        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Enemy"))
        {
            if (_hp > 0)
            {
                HandleDamageSequence(1).Forget();
            }
        }
    }

    private async UniTaskVoid HandleDamageSequence(float damage)
    {
        // ダメージ処理
        _hp -= damage;
        OnHPChanged?.Invoke(_hp, _maxHP);
        Debug.Log($"HP: {_hp}");

        audiosourse.PlayOneShot(damageSound);

        if (_hp <= 0)
        {
            Death();
            return;
        }

        isInvincible = true;
        var token = this.GetCancellationTokenOnDestroy();
        
        float elapsedTime = 0f;

        // ダメージを受けた時の点滅
        while (elapsedTime < damageCooldown)
        {
            if (spriteRenderer != null) spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            await UniTask.Delay(TimeSpan.FromSeconds(flashInterval), cancellationToken: token);
            elapsedTime += flashInterval;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;

            await UniTask.Delay(TimeSpan.FromSeconds(flashInterval), cancellationToken: token);
            elapsedTime += flashInterval;
        }

        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    private void Death()
    {
        UnityroomApiClient.Instance.SendScore(1, playerController.CurrentScore, ScoreboardWriteMode.Always);
        this.gameObject.SetActive(false); 
        gameOverPanel.SetActive(true);
    }
}