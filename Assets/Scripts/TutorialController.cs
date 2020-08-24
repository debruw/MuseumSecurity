using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    // Tutorial1
    // Tutorial2
    // Tutorial3
    // Tutorial4
    // Tutorial5
    public enum Tutorials
    {
        ScreenClick,
        FindThief,
        Arrest,
        FileFind,
        PlaceObject
    }

    public GameObject[] tutorials;

    public void CheckTutorial(Tutorials id)
    {
        CloseTutorials();
        if (!PlayerPrefs.HasKey("tutorial" + id))
        {
            tutorials[(int)id].SetActive(true);
            PlayerPrefs.SetInt(("tutorial" + id), 1);
            if(id == Tutorials.ScreenClick)
            {
                GameManager.Instance.selectedRoom = 0;
            }
        }
    }

    public void CloseTutorials()
    {
        tutorials[0].SetActive(false);
        tutorials[1].SetActive(false);
        tutorials[2].SetActive(false);
        tutorials[3].SetActive(false);
        tutorials[4].SetActive(false);
    }
}
