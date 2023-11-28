using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    private CanvasGroup CG;
    public TextMeshProUGUI msg;
    public float fadeTime = 0.3f;  // 페이드 타임
    float accumTime = 0f;
    private Coroutine fadeColor;

    // Start is called before the first frame update
    void Awake()
    {
        CG = gameObject.GetComponent<CanvasGroup>();
    }

    public void StartFadeIn(string text)
    {
        msg.text = text;
        if (fadeColor != null)
        {
            StopAllCoroutines();
            fadeColor = null;
        }
        fadeColor = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.2f);
        accumTime = 0f;
        while (accumTime < fadeTime)
        {
            CG.alpha = Mathf.Lerp(0f, 1f, accumTime / fadeTime);
            yield return 0;
            accumTime += Time.deltaTime;
        }
        CG.alpha = 1f;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(3f);
        accumTime = 0f;
        while (accumTime < fadeTime)
        {
            CG.alpha = Mathf.Lerp(1f, 0f, accumTime / fadeTime);
            yield return 0;
            accumTime += Time.deltaTime;
        }
        CG.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
