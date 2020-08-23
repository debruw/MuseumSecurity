using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnMouseUpAsButton()
    {
        if(!GameManager.Instance.isPlaced)
        {
            return;
        }
        if (transform.localEulerAngles.z < 120)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - 30);
        }

        if (transform.localEulerAngles.y <= 10)
        {
            Debug.Log("bitti");
            GameManager.Instance.DayOverPanel.SetActive(true);
            FeatureController.Instance.AddFeaturePercentage(GameManager.Instance.dayId);
            GameManager.Instance.dayId++;
            PlayerPrefs.SetInt("DayId", GameManager.Instance.dayId);
        }
    }
}
