using UnityEngine;
using System.Collections.Generic;
using EnemyState; 

public class MapChunk : MonoBehaviour
{
    // このマップに含まれる敵のリスト
    private List<IEnemyPausable> enemies = new List<IEnemyPausable>();

    void Awake()
    {
        var allEnemies = GetComponentsInChildren<IEnemyPausable>(true);
        enemies.AddRange(allEnemies);
    }

    void Start()
    {
        // SetChunkActive(false);
    }

    public void SetChunkActive(bool isActive)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                if (isActive)
                {
                    enemy.OnResume();
                }
                else
                {
                    enemy.OnPause(); 
                }
            }
        }
    }
}