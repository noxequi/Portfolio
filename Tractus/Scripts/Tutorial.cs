using UnityEngine;
using TMPro;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText; 
    [SerializeField] private string[] tutorialMessages;
    [SerializeField] private TMP_Text nextText;
    [SerializeField] private MakeLine lineDrawer;
    [SerializeField] private Collider2D destinationCollider;

    private bool isTutorialActive = false;
    private int currentMessageIndex = 0;
    private bool isWaitingForClick = false;

    private void Awake()
    {
        tutorialPanel.SetActive(false);
        if (lineDrawer == null)
        {
            return;
        }

        if (nextText != null)
        {
            nextText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTutorialActive && other.CompareTag("Player"))
        {
            StartTutorial();
        }
    }

    private void StartTutorial()
    {
        isTutorialActive = true;
        Time.timeScale = 0f; 
        if (tutorialMessages != null)
        {
            tutorialPanel.SetActive(true);
        }
        
        currentMessageIndex = 0;
        ShowMessage(currentMessageIndex);
        lineDrawer?.EnableDrawing(false);
        isWaitingForClick = true;
    }

    private void ShowMessage(int index)
    {
        if (index >= 0 && index < tutorialMessages.Length)
        {
            tutorialText.text = tutorialMessages[index];
            if (nextText != null)
            {
                nextText.gameObject.SetActive(index < tutorialMessages.Length - 1);
            }
        }
    }

    private void Update()
    {
        if (isTutorialActive && isWaitingForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentMessageIndex++;

                if (currentMessageIndex < tutorialMessages.Length)
                {
                    ShowMessage(currentMessageIndex);
                }
                else
                {
                    if (destinationCollider == null)
                    {
                        isWaitingForClick = false;
                        if (Input.GetMouseButton(0))
                        {
                            EndTutorial();
                        }
                        return;
                    }

                    isWaitingForClick = false;

                    lineDrawer?.EnableDrawing(true);
                    StartCoroutine(CheckGoal());
                }
            }
        }
    }

    private IEnumerator CheckGoal()
    {
        while (isTutorialActive)
        {
            if (lineDrawer != null && lineDrawer.HasAnyPoints())
            {
                Vector2 lastPoint = lineDrawer.GetLastPoint();
                if (destinationCollider.OverlapPoint(lastPoint))
                {
                    Debug.Log("チュートリアルクリア！");
                    EndTutorial();
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void EndTutorial()
    {
        isTutorialActive = false;
        Time.timeScale = 1f;
        tutorialPanel.SetActive(false);

        lineDrawer?.EnableDrawing(false);

        gameObject.SetActive(false);
    }
}
