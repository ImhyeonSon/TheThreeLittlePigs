using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TutorialChatting : MonoBehaviour
{
    [SerializeField] private List<GameObject> chats = new List<GameObject>();
    [SerializeField] private Transform chatTransform;
    public GameObject scrollbar;

    private float curTime;

    private void OnEnable()
    {
        foreach (Transform t in chatTransform)
        {
            chats.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
        scrollbar.GetComponent<Scrollbar>().value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        scrollbar.GetComponent<Scrollbar>().value = 0;
        if (curTime <= 1)
        {
            chats[0].SetActive(true);
        }
        else if (curTime <= 2)
        {
            chats[1].SetActive(true);
        }
        else if (curTime <= 3)
        {
            chats[2].SetActive(true);
        }
        else if (curTime <= 4)
        {
            chats[3].SetActive(true);
        }
        else if (curTime <= 5)
        {
            chats[4].SetActive(true);
        }
        else if (curTime <= 6)
        {
            chats[5].SetActive(true);
        }
        else if (curTime <= 7)
        {
            chats[6].SetActive(true);
        }
        else if (curTime <= 8)
        {
            chats[7].SetActive(true);
        }
        else if (curTime <= 9)
        {
            chats[8].SetActive(true);
        }
        else
        {
            curTime = 0;
            foreach (Transform t in chatTransform)
            {
                chats.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
            scrollbar.GetComponent<Scrollbar>().value = 1;
        }
    }
}
