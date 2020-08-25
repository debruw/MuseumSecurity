using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapControl : MonoBehaviour
{
    //inside class
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    private void Update()
    {
        if (!GameManager.Instance.isPlaced)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            ////swipe upwards
            //if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            //{
            //    Debug.Log("up swipe");
            //}
            ////swipe down
            //if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            //{
            //    Debug.Log("down swipe");
            //}
            ////swipe left
            //if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            //{
            //    Debug.Log("left swipe");
            //}
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                Debug.Log("right swipe");
                GameManager.Instance.tutoCont.CloseTutorials();
                // Close the glass door
                GetComponent<Animator>().SetTrigger("CloseDoor");
                
            }
        }
    }

    public void Finish()
    {
        Debug.Log("bitti");
        SoundManager.Instance.playSound(SoundManager.GameSounds.Win);
        GameManager.Instance.ShowFireWorks();
        GameManager.Instance.DayOverPanel.SetActive(true);
        FeatureController.Instance.AddFeaturePercentage(GameManager.Instance.dayId);
        GameManager.Instance.dayId++;
        PlayerPrefs.SetInt("DayId", GameManager.Instance.dayId);
    }
}
