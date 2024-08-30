using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Slider progressBar;

    void Start()
    {
        StartCoroutine(UpdateSliderValueOverTime(1f, 1.5f));
    }

    private IEnumerator UpdateSliderValueOverTime(float targetValue, float duration)
    {
        float initialValue = progressBar.value;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            progressBar.value = Mathf.Lerp(initialValue, targetValue, timeElapsed / duration);
            yield return null;
        }

        progressBar.value = targetValue;

        CanvasGroup canvasGroup = loadingCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float initialAlpha = canvasGroup.alpha;
            float targetAlpha = 0f;
            float fadeDuration = 1f;
            float alphaTimeElapsed = 0f;

            while (alphaTimeElapsed < fadeDuration)
            {
                alphaTimeElapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, alphaTimeElapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            loadingCanvas.SetActive(false);
        }
    }
}
