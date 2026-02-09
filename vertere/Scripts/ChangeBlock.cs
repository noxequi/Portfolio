using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChangeBlock : MonoBehaviour
{
    public bool isActive;
    [SerializeField] GameObject block;
    [SerializeField] GameObject line;
    [SerializeField] Player player;

    void Start()
    {
        isActive = block.activeSelf;
        if(isActive == true)
        {
            line.SetActive(false);
        }
        else
        {
            line.SetActive(true);
        }
    }

    void Update()
    {
        Changeblock();
    }

    private async UniTask Changeblock()
    {
        if(Input.GetKeyDown(KeyCode.Space)&& player.isGrounded())
        {
            await UniTask.Delay(1);
            if(isActive == true)
            {
                block.SetActive(false);
                line.SetActive(true);
                isActive = false;
            }
            else
            {
                block.SetActive(true);
                line.SetActive(false);
                isActive = true;
            }
        }
    }
}
