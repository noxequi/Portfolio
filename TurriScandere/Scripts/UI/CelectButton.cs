using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using TMPro;

public class SelectButton : MonoBehaviour
{
    [SerializeField] private Button defaultSelectedButton;

    // 最後に選択されていたUIオブジェクトを記憶しておく変数
    private GameObject lastSelectedObject;

    void Start()
    {
        // 最初に選択状態にしたいボタンを設定
        if (defaultSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
        }
    }

    void Update()
    {
        // 現在フォーカスされているUIオブジェクトが存在する場合
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            // 最後に選択されていたオブジェクトとして記憶
            lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        }
        // フォーカスが外れてしまっている場合
        else
        {
            // ゲームパッドやキーボードの十字キー入力があった場合
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                // 最後に選択されていたオブジェクトにフォーカスを戻す
                if (lastSelectedObject != null)
                {
                    EventSystem.current.SetSelectedGameObject(lastSelectedObject);
                }
                // もし最後のオブジェクト情報がなければ、デフォルトのボタンを選択する
                else if (defaultSelectedButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton.gameObject);
                }
            }
        }
    }
}