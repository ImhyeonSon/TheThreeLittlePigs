using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    //public GameObject notificationUI;
    public TextMeshProUGUI notificationText;
    private float fadeInOutTime = 3f;
    private static NotificationUI instance = null;

    public static NotificationUI Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<NotificationUI>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        //text = this.gameObject.GetComponent<TextMeshProUGUI>();
        //text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ShowMessageCoroutine(string msg, float duration)
    {
        Debug.Log("쇼메시지");
        Color originalColor = notificationText.color;
        notificationText.text = msg;
        gameObject.SetActive(true);

        yield return FadeInOut(notificationText, fadeInOutTime, true);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return FadeInOut(notificationText, fadeInOutTime, false);

        gameObject.SetActive(false);
        notificationText.color = originalColor;
    }

    private IEnumerator FadeInOut(TextMeshProUGUI target, float duration, bool inOut)
    {
        float start, end;
        if (inOut)
        {
            start = 0f;
            end = 1f;
        }
        else
        {
            start = 1f;
            end = 0f;
        }

        Color current = Color.clear;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            target.color = new Color(current.r, current.g, current.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void ShowMessage(string msg, float duration)
    {
        StartCoroutine(ShowMessageCoroutine(msg, duration));
    }

}
