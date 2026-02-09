using UnityEngine;
using Cysharp.Threading.Tasks; 
using System;
using UnityEngine.Audio;

public class ShrapnelEnemyBehaviour : BaseEnemyBehaviour, IEnemyPausable
{
    [SerializeField] private GameObject shrapnelBullet;

    // 攻撃設定
    [SerializeField] private int bulletCount = 12; // 1回で飛ばす弾の数
    [SerializeField] private int shotCount = 3; // 1クールで攻撃する回数
    [SerializeField] private float startWaitTime = 15.0f; // 生成直後のインターバル
    [SerializeField] private float shotInterval = 1.0f; // 1クール中の攻撃の時間幅
    [SerializeField] private float attackInterval = 10.0f; // 1連の攻撃と攻撃の間のインターバル
    [SerializeField] private float bulletSpeed = 3.0f;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    private bool isActive = false;

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    AudioSource audiosourse; 
    void Start()
    {
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
        await UniTask.WaitUntil(() => isActive, cancellationToken: token);

        while (true)
        {
            await UniTask.WaitUntil(() => isActive, cancellationToken: token);

            var startCanceled = await UniTask.Delay(TimeSpan.FromSeconds(startWaitTime), cancellationToken: token)
                                            .SuppressCancellationThrow();
            if (startCanceled) return;

            for (int i = 0; i < shotCount; i++)
            {
                await UniTask.WaitUntil(() => isActive, cancellationToken: token);
                Fire();
                var canceled = await UniTask.Delay(TimeSpan.FromSeconds(shotInterval), cancellationToken: token).SuppressCancellationThrow();
                if (canceled) return;
            }

            var cycleCanceled = await UniTask.Delay(TimeSpan.FromSeconds(attackInterval), cancellationToken: token)
                                                .SuppressCancellationThrow();

            if (cycleCanceled) return;
        }
    }

    private void Fire()
    {
        float angleStep = 360f / bulletCount;
        float startAngle = transform.eulerAngles.z;

        audiosourse.PlayOneShot(attackSound);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            
            GameObject obj = Instantiate(shrapnelBullet, transform.position + spawnOffset, rotation);
            
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = obj.transform.right * bulletSpeed;
            }
        }
    }

    public void OnPause() { isActive = false; }
    public void OnResume() { isActive = true; }
}