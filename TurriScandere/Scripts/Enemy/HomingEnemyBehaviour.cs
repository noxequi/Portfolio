using UnityEngine;
using Cysharp.Threading.Tasks; 
using System;
using UnityEngine.Audio;

public class HomingEnemyBehaviour : BaseEnemyBehaviour, IEnemyPausable
{
    private Transform player;
    [SerializeField] private GameObject HomingBullet;

    // 攻撃設定
    [SerializeField] private float startWaitTime = 15.0f; // 生成直後のインターバル
    [SerializeField] private float attackInterval = 5.0f; // 1連の攻撃と攻撃の間のインターバル
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    private bool isActive = false;

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    AudioSource audiosourse; 

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        Attack().Forget();

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

    private async UniTaskVoid Attack()
    {
        var token = this.GetCancellationTokenOnDestroy();

        var startCanceled = await UniTask.Delay(TimeSpan.FromSeconds(startWaitTime), cancellationToken: token)
                                .SuppressCancellationThrow();
        if (startCanceled) return;

        while (true)
        {
            await UniTask.WaitUntil(() => isActive, cancellationToken: token);

            Fire();
            var canceled = await UniTask.Delay(TimeSpan.FromSeconds(attackInterval), cancellationToken: token)
                                    .SuppressCancellationThrow();
            if (canceled) return;
        }
    }

    private void Fire()
    {
        if (player == null) return;
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        GameObject obj = Instantiate(HomingBullet, transform.position + spawnOffset, rotation);

        audiosourse.PlayOneShot(attackSound);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = obj.transform.right * bulletSpeed;
        }
    }

    public void OnPause() { isActive = false; }
    public void OnResume() { isActive = true; }
}
