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
        if (featurePercentage[Features.ArrestThief] > 98)
        {
            featureStatus[Features.ArrestThief] = true;
            featureObjects[(int)Features.ArrestThief].SetActive(true);
            if (PlayerPrefs.GetInt("feature" + Features.ArrestThief) == 1)
            {
                ShowParticle(Features.ArrestThief);
            }
        }
        featurePercentage[Features.SelectFile] = PlayerPrefs.GetInt("FeatureSelectFile");
        if (featurePercentage[Features.SelectFile] > 98)
        {
            featureStatus[Features.SelectFile] = true;
            featureObjects[(int)Features.SelectFile].SetActive(true);
            if (PlayerPrefs.GetInt("feature" + Features.SelectFile) == 1)
            {
                ShowParticle(Features.SelectFile);
            }
        }
        featurePercentage[Features.PlaceObject] = PlayerPrefs.GetInt("FeaturePlaceObject");
        if (featurePercentage[Features.PlaceObject] > 98)
        {
            featureStatus[Features.PlaceObject] = true;
            featureObjects[(int)Features.PlaceObject].SetActive(true);
            if (PlayerPrefs.GetInt("feature" + Features.PlaceObject) == 1)
            {
                ShowParticle(Features.PlaceObject);
            }
        }
        if (GameManager.Instance.dayId > 9)
        {
            featureImage.sprite = Congratulations;
            featurefade.gameObject.SetActive(false);
            percentageText.gameObject.SetActive(false);
            featureImage.fillAmount = 1;
        }
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
    public GameObject[] featureObjects;
    public GameObject FeatureUnlockedText;
    public Sprite Congratulations;
    public Text percentageText;

    public void AddFeaturePercentage(int dayId)
    {
        if (dayId < 2)
        {
            if (featurePercentage[Features.ArrestThief] >= 100)
            {
                return;
            }
            featureImage.sprite = FeatureSprites[(int)Features.ArrestThief];
            featurefade.sprite = FeatureSprites[(int)Features.ArrestThief];
            float perc = PlayerPrefs.GetInt("FeatureArrestThief");
            featureImage.fillAmount = perc / 100;
            percentageText.text = "%" + perc.ToString("#,#0.0");
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
            percentageText.text = "%" + perc.ToString("#,#0.0");
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
            percentageText.text = "%" + perc.ToString("#,#0.0");
            featurePercentage[Features.PlaceObject] += 25;
            PlayerPrefs.SetInt("FeaturePlaceObject", featurePercentage[Features.PlaceObject]);
            StartCoroutine(FillImage(Features.PlaceObject, 25));
        }        
    }

    IEnumerator FillImage(Features ft, int add)
    {
        while (add > 1)
        {
            add--;
            featureImage.fillAmount = (featureImage.fillAmount * 100 + 1) / 100;
            percentageText.text = "%" + (featureImage.fillAmount * 100 + 1).ToString("#,#0.0");
            yield return new WaitForSeconds(.01f);
        }
        if(featurePercentage[ft] > 100)
        {
            featurePercentage[ft] = 100;
            featureImage.fillAmount = 1;
            percentageText.text = "%" + featurePercentage[ft].ToString("#,#0.0");
        }
        if (featureImage.fillAmount > .95f)
        {
            featureStatus[ft] = true;
            FeatureUnlockedText.SetActive(true);
            SoundManager.Instance.playSound(SoundManager.GameSounds.FeatureUnlock);
            GameManager.Instance.ShowFireWorks();
            PlayerPrefs.SetInt("feature" + ft, 1);
        }
    }

    public void ShowParticle(Features featureId)
    {
        Debug.Log("play stars");
        featureObjects[(int)featureId].GetComponent<ParticleSystem>().Play();
        PlayerPrefs.SetInt("feature" + featureId, 0);
    }
}
