using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    public Transform player;

    public GameObject[] mapPrefabs; // マップのプレハブ
    public float floorHeight = 20.0f; // 1マップの高さ
    public int viewRange = 3; //上下に保持するマップ数
    private Dictionary<int, MapChunk> generatedChunks = new Dictionary<int, MapChunk>();// 現在生成されているマップのリスト
    private int currentPlayerFloor = -999;

    void Start()
    {
        if (player != null)
        {
            currentPlayerFloor = Mathf.FloorToInt(player.position.y / floorHeight);
        }
        else
        {
            currentPlayerFloor = 0;
        }
        UpdateTower();
    }

    void Update()
    {
        if (player == null) return;

        // プレイヤーの現在の階層を計算
        int newFloor = Mathf.FloorToInt(player.position.y / floorHeight);

        if (newFloor != currentPlayerFloor)
        {
            currentPlayerFloor = newFloor;
            UpdateTower();
        }
    }

    void UpdateTower()
    {
        int minFloor = currentPlayerFloor - viewRange;
        int maxFloor = currentPlayerFloor + viewRange;

        // 範囲外の古いマップを削除
        List<int> toRemove = new List<int>();
        foreach (var floor in generatedChunks.Keys)
        {
            if (floor < minFloor || floor > maxFloor)
            {
                toRemove.Add(floor);
            }
        }
        foreach (var floor in toRemove)
        {
            if (generatedChunks[floor] != null)
            {
                Destroy(generatedChunks[floor].gameObject);
            }
            generatedChunks.Remove(floor);
        }

        // 範囲内の新しいマップを生成
        for (int i = minFloor; i <= maxFloor; i++)
        {
            if (i < 0) continue; 

            if (!generatedChunks.ContainsKey(i))
            {
                CreateFloor(i);
            }
        }

        // 敵のアクティブ状態を更新
        foreach (var kvp in generatedChunks)
        {
            int floorNum = kvp.Key;
            MapChunk chunk = kvp.Value;

            if (chunk != null)
            {
                // 今の階層だけ敵を動かす。それ以外は止める。
                bool isCurrent = (floorNum == currentPlayerFloor);
                chunk.SetChunkActive(isCurrent);
            }
        }
    }

    void CreateFloor(int floorIndex)
    {
        if (mapPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, mapPrefabs.Length);
        GameObject prefab = mapPrefabs[randomIndex];
        Vector3 spawnPos = new Vector3(0, floorIndex * floorHeight, 0);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        
        MapChunk chunkScript = obj.GetComponent<MapChunk>();
        
        if (chunkScript == null)
        {
            chunkScript = obj.AddComponent<MapChunk>();
        }

        generatedChunks.Add(floorIndex, chunkScript);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        int centerFloor = (player != null) ? currentPlayerFloor : 0;

        for (int i = centerFloor - 5; i <= centerFloor + 5; i++)
        {
            float y = i * floorHeight;
            
            Gizmos.DrawLine(new Vector3(-10, y, 0), new Vector3(10, y, 0));

#if UNITY_EDITOR
            UnityEditor.Handles.Label(new Vector3(12, y, 0), $"Floor {i}");
#endif
        }
    }
}