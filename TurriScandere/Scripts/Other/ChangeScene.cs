using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static string currentScene;
    private PlayerController playerController;
    [SerializeField] private GameObject OperationPanel;
    [SerializeField] private GameObject SettingPanel;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // Tキー: タイトルへ戻る
        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadTitle();
        }

        // Qキー: ゲーム終了
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (SettingPanel != null)
            {
                SettingPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Time.timeScale = 1;
            SettingPanel.SetActive(false);
            OperationPanel.SetActive(false);
        }
    } 

    /// <summary>
    /// タイトル画面のStart
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Exitボタン
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void Operation()
    {
        OperationPanel.SetActive(true);
    }

    public void Setting()
    {
        SettingPanel.SetActive(true);
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            LoadTitle();
        }
    }

    private void LoadTitle()
    {
        SceneManager.LoadScene("Title"); 
    }
}