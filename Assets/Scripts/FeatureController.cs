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
    }

    public enum Features
    {
        SelectScreen,
        FindThief,
        ArrestThief,
        SelectFile,
        PlaceObject
    }

    public Dictionary<Features, bool> featureStatus;
    public Sprite[] FeatureSprites;
    public Image featureImage;

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

    public void UnlockFeature(Features id)
    {
        featureImage.sprite = FeatureSprites[(int)id];

    }
}
