using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public static string currentScene;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        playerController = FindObjectOfType<PlayerController>();

        var sceneFader = FindObjectOfType<SceneFader>();
        if (sceneFader != null)
        {
            _ = sceneFader.PerformSceneStartFadeIn(); 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ClearDontDestroyObjects();
            SceneManager.LoadScene("Title");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearDontDestroyObjects();
            SceneManager.LoadScene(currentScene);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null) return;
        }

        if (playerController.isClear)
        {
            if (Input.GetMouseButton(0))
            {
                ClearDontDestroyObjects();
                LoadNextScene();
            }
        }
    }

    private void LoadNextScene()
    {
        playerController.isClear = false;

        switch (currentScene)
        {
            case "Tutorial_Line":
                SceneManager.LoadScene("Tutorial_Long");
                break;
            case "Tutorial_Long":
                SceneManager.LoadScene("Tutorial_Wall");
                break;
            case "Tutorial_Wall":
                SceneManager.LoadScene("Tutorial_Weapon");
                break;
            case "Tutorial_Weapon":
                SceneManager.LoadScene("Tutorial_Enemy");
                break;
            case "Tutorial_Enemy":
                SceneManager.LoadScene("Tutorial_Key");
                break;
            case "Tutorial_Key":
                SceneManager.LoadScene("Title");
                break;
            case "Easy":
                SceneManager.LoadScene("Normal");
                break;
            case "Normal":
                SceneManager.LoadScene("Hard");
                break;
            case "Hard":
                SceneManager.LoadScene("Title");
                break;
        }
    }

    private void ClearDontDestroyObjects()
    {
        var temp = new GameObject("TempObjectForDestroy");
        DontDestroyOnLoad(temp);
        var rootObjects = temp.scene.GetRootGameObjects();

        foreach (var obj in rootObjects)
        {
            if (obj != temp) 
            {
                Destroy(obj);
            }
        }
        Destroy(temp);
    }
}
