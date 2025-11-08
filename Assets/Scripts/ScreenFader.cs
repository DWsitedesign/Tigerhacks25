using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;

    private Image fadeImage;
    private Coroutine currentFade;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        Color c = fadeImage.color;
        c.a = 0f; // start transparent
        fadeImage.color = c;
    }

    public void FadeOut()
    {
        Debug.Log("FadeOut called");
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(1f));
    }

    public void FadeIn()
    {
        Debug.Log("FadeIn called");
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            Color c = fadeImage.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = c;
            yield return null;
        }

        Color final = fadeImage.color;
        final.a = targetAlpha;
        fadeImage.color = final;
    }
}
