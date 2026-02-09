using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    [SerializeField] private GameObject operationPanel;
    [SerializeField] private GameObject settingPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            operationPanel.SetActive(false);
            settingPanel.SetActive(false);
        }
    }

    public void Onclick_Button_Tutorial()
    {
        SceneManager.LoadScene("Tutorial_Line");
    }

    public void Onclick_Button_Easy()
    {
        SceneManager.LoadScene("Easy");
    }

    public void Onclick_Button_Normal()
    {
        SceneManager.LoadScene("Normal");
    }

    public void Onclick_Button_Hard()
    {
        SceneManager.LoadScene("Hard");
    }

    public void Onclick_Button_Operation()
    {
        operationPanel.SetActive(true);
        if (Input.GetKeyDown(KeyCode.T))
        {
            operationPanel.SetActive(false);
        }
    }
    
    public void Onclick_Button_Setting()
    {
        settingPanel.SetActive(true);
        if (Input.GetKeyDown(KeyCode.T))
        {
            settingPanel.SetActive(false);
        }
    }
}
