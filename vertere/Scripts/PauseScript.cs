using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//一時停止ボタン、参考　https://www.youtube.com/watch?v=w10_AXiGYuY

public class PauseScript : MonoBehaviour{
    [System.NonSerialized] public bool IsOnPause;
    [Header("ポーズ中に表示するパネル等")]public GameObject [] pauseEffects;
    [Header("ポーズ中隠したいオブジェクト")] public GameObject [] HideObjects;
    [Header("ポーズ画面に入る効果音")]public AudioClip pauseOnSE;
    [Header("ポーズ画面から抜ける効果音")] public AudioClip pauseOffSE;
    SoundManager soundManager;
    //public GameContoroller gameContoroller;//ステージクリア後かどうか判定するためにgamecontorollerを登録。ゲーム内容によっては不要。

    private void Start() {
        GameObject gameObject = GameObject.FindGameObjectWithTag("SoundManager");
        soundManager = gameObject.GetComponent<SoundManager>();
    }

    //画面上に設定しているボタンをクリックした場合
    public void OnClick() {
        pauseTheGame();
    }

    //マウス、キーボード、コントローラー等のボタンを押した場合
    public void Update() {
        //////////////////※最重要※ポーズ画面になる条件///////////////////
       if (Input.GetKeyDown(KeyCode.X)/* &&!gameContoroller.isEnd*/) {         
            pauseTheGame();
        }
    }

    public void pauseTheGame(){
        if (IsOnPause) { 
            Time.timeScale = 1;
            IsOnPause = false;
            //soundManager.PlaySe(pauseOffSE);
            for (int i = 0; i < pauseEffects.Length; i++) {
                pauseEffects[i].SetActive(false);
            }
            for (int i = 0; i < HideObjects.Length; i++) {
                HideObjects[i].SetActive(true);
            }
        } else {
            //soundManager.PlaySe(pauseOnSE);
            Time.timeScale = 0;
            IsOnPause = true;
            for (int i = 0; i < pauseEffects.Length; i++) {
                pauseEffects[i].SetActive(true);
            }
            for (int i = 0; i < HideObjects.Length; i++) {
                HideObjects[i].SetActive(false);
            }
        }
    }
}