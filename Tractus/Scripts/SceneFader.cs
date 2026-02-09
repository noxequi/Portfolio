using UnityEngine;
using Cysharp.Threading.Tasks;

public class SceneFader : MonoBehaviour
{
    public GameManager gameManager;
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1.5f;
    public float initialDelay = 2f;

    async void Start()
    {
        await PerformSceneStartFadeIn();
    }

    public async UniTask PerformSceneStartFadeIn()
    {
        fadeCanvas.alpha = 1f;
        Time.timeScale = 0f;

        if (gameManager != null)
        {
            gameManager.enabled = false;
        }

        await UniTask.Delay((int)(initialDelay * 1000), ignoreTimeScale: true);
        await Fade(0f, fadeDuration, true);
        await UniTask.Yield();

        Time.timeScale = 1f;

        if (gameManager != null)
        {
            gameManager.enabled = true;
        }
    }

    public async UniTask FadeIn(float duration, bool ignoreTimeScale = false)
    {
        await Fade(0f, duration, ignoreTimeScale);
    }

    public async UniTask FadeOut(float duration, bool ignoreTimeScale = false)
    {
        await Fade(1f, duration, ignoreTimeScale);
    }

    private async UniTask Fade(float targetAlpha, float duration, bool ignoreTimeScale)
    {
        float startAlpha = fadeCanvas.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            await UniTask.Yield();
        }

        fadeCanvas.alpha = targetAlpha;
    }
}