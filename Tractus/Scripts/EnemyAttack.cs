using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private GameObject straightBulletPrefab;
    [SerializeField] private GameObject bounceBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int cooldownTime = 3000;         
    [SerializeField] private int intervalBetweenShots = 1000; 
    [SerializeField] private int firstShotDelay = 1000;       

    private string enemyName;

    void Start()
    {
        enemyName = gameObject.name;

        if (enemyName.Contains("StraightEnemy"))
        {
            FireStraight().Forget();
        }
        else if (enemyName.Contains("BounceEnemy"))
        {
            FireBounce().Forget();
        }
    }

    private async UniTaskVoid FireStraight()
    {
        await UniTask.Delay(firstShotDelay); 

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(straightBulletPrefab, firePoint.position, Quaternion.identity);
                await UniTask.Delay(intervalBetweenShots);
            }
            await UniTask.Delay(cooldownTime);
        }
    }

    private async UniTaskVoid FireBounce()
    {
        await UniTask.Delay(firstShotDelay); 

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(bounceBulletPrefab, firePoint.position, Quaternion.identity);
                await UniTask.Delay(intervalBetweenShots);
            }
            await UniTask.Delay(cooldownTime);
        }
    }
}
