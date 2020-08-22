using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureController : MonoBehaviour
{
    private static FeatureController _instance;

    public static FeatureController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        featureStatus[Features.SelectScreen] = false;
        featureStatus[Features.FindThief] = false;
        featureStatus[Features.ArrestThief] = false;
        featureStatus[Features.SelectFile] = false;
        featureStatus[Features.PlaceObject] = false;

        featurePercentage[Features.ArrestThief] = PlayerPrefs.GetInt("FeatureArrestThief");
        Debug.Log(featurePercentage[Features.ArrestThief]);
        if (featurePercentage[Features.ArrestThief] > 98)
        {
            featureStatus[Features.ArrestThief] = true;            
        }
        featurePercentage[Features.SelectFile] = PlayerPrefs.GetInt("FeatureSelectFile");
        if (featurePercentage[Features.SelectFile] > 98)
        {
            featureStatus[Features.SelectFile] = true;
        }
        featurePercentage[Features.PlaceObject] = PlayerPrefs.GetInt("FeaturePlaceObject");
        if (featurePercentage[Features.PlaceObject] > 98)
        {
            featureStatus[Features.PlaceObject] = true;
        }
    }

    private void Start()
    {
        //if (GameManager.Instance.dayId < 2)
        //{
        //    featureImage.sprite = FeatureSprites[(int)Features.ArrestThief];
        //    featurefade.sprite = FeatureSprites[(int)Features.ArrestThief];
        //    Debug.Log("FeatureArrestThief: " + PlayerPrefs.GetInt("FeatureArrestThief"));
        //    featureImage.fillAmount = PlayerPrefs.GetInt("FeatureArrestThief") / 100;
        //    Debug.Log(featureImage.fillAmount);
        //}
        //if (GameManager.Instance.dayId > 1 && GameManager.Instance.dayId < 5)
        //{
        //    featureImage.sprite = FeatureSprites[(int)Features.PlaceObject];
        //    featurefade.sprite = FeatureSprites[(int)Features.PlaceObject];
        //    featureImage.fillAmount = PlayerPrefs.GetInt("FeaturePlaceObject") / 100;
        //}
        //if (GameManager.Instance.dayId > 4 && GameManager.Instance.dayId < 9)
        //{
        //    featureImage.sprite = FeatureSprites[(int)Features.PlaceObject];
        //    featurefade.sprite = FeatureSprites[(int)Features.PlaceObject];
        //    featureImage.fillAmount = PlayerPrefs.GetInt("FeaturePlaceObject") / 100;
        //}
    }

    public enum Features
    {
        SelectScreen,
        FindThief,
        ArrestThief,
        SelectFile,
        PlaceObject
    }

    public Dictionary<Features, bool> featureStatus = new Dictionary<Features, bool>();
    public Dictionary<Features, int> featurePercentage = new Dictionary<Features, int>();
    public Sprite[] FeatureSprites;
    public Image featureImage, featurefade;

    public void CheckFeatures(int dayId)
    {
        if (dayId < 2)
        {
            featureStatus[Features.SelectScreen] = true;
            featureStatus[Features.FindThief] = true;
        }
        if (dayId > 1 && dayId < 5)
        {
            featureStatus[Features.ArrestThief] = true;
        }
        if (dayId > 4 && dayId < 9)
        {
            featureStatus[Features.SelectFile] = true;
        }
        if (dayId > 8 && dayId < 15)
        {
            featureStatus[Features.PlaceObject] = true;
        }
    }

    public void AddFeaturePercentage(int dayId)
    {
        if (dayId < 2)
        {
            if(featurePercentage[Features.ArrestThief] >= 100)
            {
                return;
            }
            featureImage.sprite = FeatureSprites[(int)Features.ArrestThief];
            featurefade.sprite = FeatureSprites[(int)Features.ArrestThief];
            float perc = PlayerPrefs.GetInt("FeatureArrestThief");
            featureImage.fillAmount = perc / 100;
            featurePercentage[Features.ArrestThief] += 50;
            PlayerPrefs.SetInt("FeatureArrestThief", featurePercentage[Features.ArrestThief]);
            StartCoroutine(FillImage(Features.ArrestThief, 50));
        }
        if (dayId > 1 && dayId < 5)
        {
            if (featurePercentage[Features.SelectFile] >= 100)
            {
                return;
            }
            featureImage.sprite = FeatureSprites[(int)Features.SelectFile];
            featurefade.sprite = FeatureSprites[(int)Features.SelectFile];
            float perc = PlayerPrefs.GetInt("FeatureSelectFile");
            featureImage.fillAmount = perc / 100;            
            featurePercentage[Features.SelectFile] += 34;
            PlayerPrefs.SetInt("FeatureSelectFile", featurePercentage[Features.SelectFile]);
            StartCoroutine(FillImage(Features.SelectFile, 34));
        }
        if (dayId > 4 && dayId < 9)
        {
            if (featurePercentage[Features.PlaceObject] >= 100)
            {
                return;
            }
            featureImage.sprite = FeatureSprites[(int)Features.PlaceObject];
            featurefade.sprite = FeatureSprites[(int)Features.PlaceObject];
            float perc = PlayerPrefs.GetInt("FeaturePlaceObject");
            featureImage.fillAmount = perc / 100;
            featurePercentage[Features.PlaceObject] += 25;
            PlayerPrefs.SetInt("FeaturePlaceObject", featurePercentage[Features.PlaceObject]);
            StartCoroutine(FillImage(Features.PlaceObject, 25));
        }
    }

    IEnumerator FillImage(Features ft, int add)
    {
        while (add > 0)
        {
            add--;
            featureImage.fillAmount = (featureImage.fillAmount * 100 + 1) / 100;
            yield return new WaitForSeconds(.05f);
        }
        if(featureImage.fillAmount > .98)
        {
            featureStatus[ft] = true;
            GameManager.Instance.ShowFireWorks();
        }
    }
}
