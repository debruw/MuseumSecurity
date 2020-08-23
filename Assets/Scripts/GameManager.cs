using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TapticPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    [HideInInspector]
    public int roomCount = 4, selectedRoom;

    public GameObject AlarmLight;
    public GameObject Thief;
    public Transform stolenPosition;
    public Transform placePosition;
    public GameObject soundManagerPrefab;
    public GameObject StolenObjectEffects;

    public int dayId = 0;

    #region Booleans
    bool isAlarmed = false;
    public bool isThiefFounded = false;
    public bool isThiefArrested = false;
    public bool isPlaced = false;
    #endregion

    #region Room Objets
    public RoomConfiguration[] RoomConfigurations;
    public GameObject SecurityRoomCamera;
    public GameObject PlacingRoomCamera;
    public GameObject InvestigationCamera;

    #endregion

    #region UIElements
    public GameObject transitionUI;
    public GameObject ArrestUI, ArrestUIPointer;
    public GameObject ArrestedTextUI;
    public GameObject InvestigationUI;
    public GameObject[] InvestImages;
    public GameObject Cross;
    public GameObject DayOverPanel, FailPanel;
    public GameObject VibrationButton;

    public TextMeshProUGUI dayIdText;
    #endregion

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
        Instantiate(soundManagerPrefab);

        if (!PlayerPrefs.HasKey("VIBRATION"))
        {
            PlayerPrefs.SetInt("VIBRATION", 1);
            VibrationButton.GetComponent<Image>().sprite = on;
        }

        if (!PlayerPrefs.HasKey("DayId"))
        {
            PlayerPrefs.SetInt("DayId", dayId);
        }
        else
        {
            dayId = PlayerPrefs.GetInt("DayId");
        }
        dayIdText.text = "Day " + (dayId + 1);

        selectedRoom = Random.Range(0, roomCount);
        if (PlayerPrefs.GetInt("LastRoomId", 0) == selectedRoom)
        {
            selectedRoom = Random.Range(0, roomCount);
        }
        PlayerPrefs.SetInt("LastRoomId", selectedRoom);

        //FeatureController.Instance.CheckFeatures(dayId);

        InitializeGame();
    }

    private void Start()
    {

    }

    void InitializeGame()
    {
        Debug.Log("Selected Room : " + selectedRoom);
        RoomConfigurations[selectedRoom].SecurityScreen.SetActive(true);
        Thief.GetComponent<NavMeshAgent>().transform.position = RoomConfigurations[selectedRoom].ThiefStart.position;
        Thief.GetComponent<NavMeshAgent>().SetDestination(RoomConfigurations[selectedRoom].ThiefEnd.position);
        StartCoroutine(WaitAndAlarm());
    }

    IEnumerator WaitAndAlarm()
    {
        yield return new WaitForSeconds(3f);
        SoundManager.Instance.playSound(SoundManager.GameSounds.Alarm);
        AlarmLight.GetComponent<Animator>().SetTrigger("StartAlarm");
        isAlarmed = true;
    }

    public void ChangeScreen(int roomId)
    {
        if (isAlarmed)
        {
            if (roomId == selectedRoom)
            {
                transitionUI.GetComponent<Animator>().SetTrigger("StartTransition");
                Debug.Log("Clicked Room : " + roomId);
                StartCoroutine(WaitAndChangeCamera(roomId));

                AlarmLight.GetComponent<Animator>().SetTrigger("StopAlarm");
                SoundManager.Instance.stopSound(SoundManager.GameSounds.Alarm);
                //AlarmLight.GetComponent<Animator>().enabled = false; 
            }
            else
            {
                Cross.GetComponent<Animator>().SetTrigger("Wrong");
            }
        }
    }

    IEnumerator WaitAndChangeCamera(int roomId)
    {
        yield return new WaitForSeconds(.5f);
        // Make thief crouch
        Thief.GetComponent<NavMeshAgent>().isStopped = true;
        Thief.GetComponent<NavMeshAgent>().transform.position = RoomConfigurations[selectedRoom].HidingSpots[Random.Range(0, RoomConfigurations[selectedRoom].HidingSpots.Length)].position;
        Thief.transform.LookAt(RoomConfigurations[selectedRoom].SearchCamera.transform);
        Thief.GetComponent<Animator>().SetTrigger("StopMove");
        SecurityRoomCamera.SetActive(false);
        RoomConfigurations[roomId].SearchCamera.SetActive(true);
    }

    public void ThiefFounded()
    {
        // Open the lights
        StartCoroutine(WaitForAnim());
        Thief.GetComponent<Animator>().SetTrigger("Surrender");
        isThiefFounded = true;
        RoomConfigurations[selectedRoom].SearchCamera.transform.LookAt(Thief.transform);
        RoomConfigurations[selectedRoom].SearchCamera.transform.eulerAngles =
            new Vector3
            (
                9,
                RoomConfigurations[selectedRoom].SearchCamera.transform.eulerAngles.y,
                RoomConfigurations[selectedRoom].SearchCamera.transform.eulerAngles.z
            );
        if (FeatureController.Instance.featureStatus[FeatureController.Features.ArrestThief])
        {
            // Start arresting sequence
            ArrestUI.SetActive(true);
            RoomConfigurations[selectedRoom].HandCuff.SetActive(true);
        }
        else
        {
            SoundManager.Instance.playSound(SoundManager.GameSounds.Win);
            ShowFireWorks();
            DayOverPanel.SetActive(true);
            FeatureController.Instance.AddFeaturePercentage(dayId);
            dayId++;
            PlayerPrefs.SetInt("DayId", dayId);
        }
    }

    IEnumerator WaitForAnim()
    {
        RoomConfigurations[selectedRoom].SearchCamera.GetComponent<Animator>().SetTrigger("LightUp");
        RoomConfigurations[selectedRoom].SearchCamera.transform.GetChild(1).gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

    int ArrestTryCount = 0;
    GameObject Stolen;
    public void CheckArrestSuccess()
    {
        Debug.Log("Checking Arrest...");
        Debug.Log(ArrestUIPointer.transform.eulerAngles.z);
        RoomConfigurations[selectedRoom].SearchCamera.GetComponent<Animator>().SetTrigger("TryArrest");
        if (ArrestUIPointer.transform.eulerAngles.z > 0 && ArrestUIPointer.transform.eulerAngles.z < 16
            || ArrestUIPointer.transform.eulerAngles.z < 360 && ArrestUIPointer.transform.eulerAngles.z > 344)
        {
            Debug.Log("Arrest succesfull");
            isThiefArrested = true;
            // Make a puff
            RoomConfigurations[selectedRoom].SearchCamera.transform.GetChild(0).gameObject.SetActive(true);
            RoomConfigurations[selectedRoom].HandCuff.SetActive(false);
            // Arrest Thief
            Thief.GetComponent<Animator>().SetTrigger("Arrested");
            ArrestUI.SetActive(false);
            if (!FeatureController.Instance.featureStatus[FeatureController.Features.SelectFile])
            {
                ArrestedTextUI.transform.GetChild(0).gameObject.SetActive(false);
            }
            ArrestedTextUI.SetActive(true);
            SoundManager.Instance.playSound(SoundManager.GameSounds.Arrested);
            Stolen = RoomConfigurations[selectedRoom].StolenObjects[Random.Range(0, RoomConfigurations[selectedRoom].StolenObjects.Count)];
            Stolen.transform.position = stolenPosition.position;
            Stolen.transform.LookAt(RoomConfigurations[selectedRoom].SearchCamera.transform);
            StolenObjectEffects.SetActive(true);
            if (Stolen.transform.localScale.x > 4)
            {
                Stolen.transform.localScale = Stolen.transform.localScale / 2;
            }
            SetInvestigateImages();

            if (!FeatureController.Instance.featureStatus[FeatureController.Features.SelectFile])
            {
                StartCoroutine(WaitAndNextDay());
            }
        }
        else
        {
            Debug.Log("Arrest unsuccessfull!");
            if (ArrestTryCount >= 2)
            {
                // FAİL
                FailPanel.SetActive(true);
                TapticManager.Impact(ImpactFeedback.Medium);
                SoundManager.Instance.playSound(SoundManager.GameSounds.Lose);
            }
            else
            {
                Debug.Log("Try Again");
                RoomConfigurations[selectedRoom].SearchCamera.GetComponent<Animator>().SetTrigger("ArrestFail");
                ArrestUI.GetComponent<Animator>().SetTrigger("Start");
                ArrestTryCount++;
            }
        }
    }

    IEnumerator WaitAndNextDay()
    {
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.playSound(SoundManager.GameSounds.Win);
        ShowFireWorks();
        DayOverPanel.SetActive(true);
        FeatureController.Instance.AddFeaturePercentage(dayId);
        dayId++;
        PlayerPrefs.SetInt("DayId", dayId);
    }

    int objectsFileNumber;
    void SetInvestigateImages()
    {
        objectsFileNumber = Random.Range(0, InvestImages.Length);
        InvestImages[objectsFileNumber].GetComponent<Image>().sprite = Stolen.GetComponent<Prop>().UISprite;
        RoomConfigurations[selectedRoom].StolenObjects.Remove(Stolen);
        foreach (GameObject item in InvestImages)
        {
            if (item.GetComponent<Image>().sprite == null)
            {
                item.GetComponent<Image>().sprite = RoomConfigurations[selectedRoom].StolenObjects[Random.Range(0, RoomConfigurations[selectedRoom].StolenObjects.Count)].GetComponent<Prop>().UISprite;
            }
        }
    }

    public void GoToInvestigate()
    {
        if (FeatureController.Instance.featureStatus[FeatureController.Features.SelectFile])
        {
            InvestigationCamera.SetActive(true);
            RoomConfigurations[selectedRoom].SearchCamera.SetActive(false);
            ArrestedTextUI.SetActive(false);
            InvestigationUI.SetActive(true);
        }
        else
        {
            SoundManager.Instance.playSound(SoundManager.GameSounds.Win);
            ShowFireWorks();
            DayOverPanel.SetActive(true);
            FeatureController.Instance.AddFeaturePercentage(dayId);
            dayId++;
            PlayerPrefs.SetInt("DayId", dayId);
        }
    }

    public Transform placingObjectPosition;
    public void SelectStolenObjectsFile(int fileID)
    {
        if (objectsFileNumber == fileID)
        {
            InvestImages[objectsFileNumber].transform.GetChild(0).gameObject.SetActive(true);
            if (FeatureController.Instance.featureStatus[FeatureController.Features.PlaceObject])
            {
                Debug.Log("True Object");
                transitionUI.GetComponent<Animator>().SetTrigger("StartTransition");
                InvestigationUI.SetActive(false);
                Stolen.transform.position = placingObjectPosition.position;
                Stolen.transform.rotation = Quaternion.identity;
                StartCoroutine(WaitAndGoPlacing());
            }
            else
            {
                SoundManager.Instance.playSound(SoundManager.GameSounds.Win);
                ShowFireWorks();
                InvestigationUI.SetActive(false);
                DayOverPanel.SetActive(true);
                FeatureController.Instance.AddFeaturePercentage(dayId);
                dayId++;
                PlayerPrefs.SetInt("DayId", dayId);
            }
        }
        else
        {
            Debug.Log("False Object");
            Cross.GetComponent<Animator>().SetTrigger("Wrong");
        }
    }

    IEnumerator WaitAndGoPlacing()
    {
        yield return new WaitForSeconds(1f);
        SecurityRoomCamera.SetActive(false);
        PlacingRoomCamera.SetActive(true);
    }

    public void ShowFireWorks()
    {
        TapticManager.Notification(NotificationFeedback.Success);
        RoomConfigurations[selectedRoom].SearchCamera.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void NextDayButtonClick()
    {
        SceneManager.LoadScene("Scene_Game");
    }

    public void RetryButtonClick()
    {
        SceneManager.LoadScene("Scene_Game");
    }

    public Sprite on, off;
    public void VibrateButtonClick()
    {
        if (PlayerPrefs.GetInt("VIBRATION").Equals(1))
        {//Vibration is on
            PlayerPrefs.SetInt("VIBRATION", 0);
            VibrationButton.GetComponent<Image>().sprite = off;
        }
        else
        {//Vibration is off
            PlayerPrefs.SetInt("VIBRATION", 1);
            VibrationButton.GetComponent<Image>().sprite = on;
        }

        TapticManager.Impact(ImpactFeedback.Light);
    }
}
