using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    public GameObject tutorialList;
    public GameObject leftButton;
    public GameObject rightButton;
    private int currentObject = 0;

    public void OnClickLeftButton()
    {
        // 버튼을 누른 후 마지막 팁이면
        currentObject--;
        if (currentObject == 0)
        {
            leftButton.SetActive(false);
            tutorialList.transform.GetChild(currentObject + 1).gameObject.SetActive(false);
            tutorialList.transform.GetChild(currentObject).gameObject.SetActive(true);
        }
        else
        {
            leftButton.SetActive(true);
            rightButton.SetActive(true);
            tutorialList.transform.GetChild(currentObject + 1).gameObject.SetActive(false);
            tutorialList.transform.GetChild(currentObject).gameObject.SetActive(true);
        }
    }

    public void OnClickRightButton()
    {
        // 버튼을 누른 후 마지막 팁이면
        currentObject ++;
        if (currentObject == tutorialList.transform.childCount - 1)
        {
            rightButton.SetActive(false);
            tutorialList.transform.GetChild(currentObject-1).gameObject.SetActive(false);
            tutorialList.transform.GetChild(currentObject).gameObject.SetActive(true);
        }
        else
        {
            leftButton.SetActive(true);
            rightButton.SetActive(true);
            tutorialList.transform.GetChild(currentObject - 1).gameObject.SetActive(false);
            tutorialList.transform.GetChild(currentObject).gameObject.SetActive(true);
        }
    }
}
