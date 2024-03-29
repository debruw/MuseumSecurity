﻿using System.Collections;
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
    public GameObject tutorialManagerPrefab;
    public GameObject StolenObjectEffects;
    public TutorialController tutoCont;
    public Light light1, light2;

    public int dayId = 0;

    #region Booleans
    bool isAlarmed = false;
    public bool isThiefFounded = false;
    public bool isThiefArrested = false;
    public bool isPlaced = false;
    public bool isAllowPlacing = false;
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
        tutoCont = Instantiate(tutorialManagerPrefab).GetComponent<TutorialController>();
        if (!PlayerPrefs.HasKey("tutorial" + TutorialController.Tutorials.ScreenClick))
        {
            selectedRoom = 0;
        }
        PlayerPrefs.SetInt("LastRoomId", selectedRoom);
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
        yield return new WaitForSeconds(2f);
        tutoCont.CheckTutorial(TutorialController.Tutorials.ScreenClick);
        SoundManager.Instance.playSound(SoundManager.GameSounds.Alarm);
        AlarmLight.GetComponent<Animator>().SetTrigger("StartAlarm");
        isAlarmed = true;
        yield return new WaitForSeconds(.5f);
        SecurityRoomCamera.GetComponent<Animator>().SetTrigger("Security1");
    }

    public void ChangeScreen(int roomId)
    {
        if (isAlarmed)
        {
            if (roomId == selectedRoom)
            {
                SecurityRoomCamera.GetComponent<Animator>().SetInteger("SecurityToRoom", roomId);

                transitionUI.GetComponent<Animator>().SetTrigger("StartTransition");
                Debug.Log("Clicked Room : " + roomId);
                StartCoroutine(WaitAndChangeCamera(roomId));

                AlarmLight.GetComponent<Animator>().SetTrigger("StopAlarm");
                SoundManager.Instance.stopSound(SoundManager.GameSounds.Alarm);
                //AlarmLight.GetComponent<Animator>().enabled = false; 
                tutoCont.CheckTutorial(TutorialController.Tutorials.FindThief);
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
        light1.intensity = 1.25f;
        light2.intensity = 1.25f;
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
        StartCoroutine(WaitAndGo());
    }

    IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(1f);
        if (FeatureController.Instance.featureStatus[FeatureController.Features.ArrestThief])
        {
            // Start arresting sequence
            ArrestUI.SetActive(true);
            RoomConfigurations[selectedRoom].HandCuff.SetActive(true);
            tutoCont.CheckTutorial(TutorialController.Tutorials.Arrest);
        }
        else
        {
            tutoCont.CloseTutorials();
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
            ArrestUI.GetComponent<Animator>().enabled = false;
            StartCoroutine(WaitAndArrest());                       
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

    IEnumerator WaitAndArrest()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Arrest succesfull");
        isThiefArrested = true;
        // Make a puff
        RoomConfigurations[selectedRoom].SearchCamera.transform.GetChild(0).gameObject.SetActive(true);
        RoomConfigurations[selectedRoom].HandCuff.SetActive(false);
        // Arrest Thief
        Thief.GetComponent<Animator>().SetTrigger("Arrested");
               
        if (!FeatureController.Instance.featureStatus[FeatureController.Features.SelectFile])
        {
            ArrestedTextUI.transform.GetChild(0).gameObject.SetActive(false);
        }
        ArrestedTextUI.SetActive(true);
        SoundManager.Instance.playSound(SoundManager.GameSounds.Arrested);

        if (!FeatureController.Instance.featureStatus[FeatureController.Features.SelectFile])
        {
            tutoCont.CloseTutorials();
            StartCoroutine(WaitAndNextDay());
        }
        if (FeatureController.Instance.featureStatus[FeatureController.Features.SelectFile])
        {
            Stolen = RoomConfigurations[selectedRoom].StolenObjects[Random.Range(0, RoomConfigurations[selectedRoom].StolenObjects.Count)];
            Stolen.transform.position = stolenPosition.position;
            Stolen.transform.LookAt(RoomConfigurations[selectedRoom].SearchCamera.transform);
            Stolen.GetComponent<Prop>().isTurning = true;
            StolenObjectEffects.SetActive(true);
            if (Stolen.transform.localScale.x > 4)
            {
                Stolen.transform.localScale = Stolen.transform.localScale / 2;
            }
            SetInvestigateImages();
        }
    }

    IEnumerator WaitAndNextDay()
    {
        yield return new WaitForSeconds(1f);
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
            ArrestUI.SetActive(false);
            Stolen.GetComponent<Prop>().isTurning = false;
            tutoCont.CheckTutorial(TutorialController.Tutorials.FileFind);
            InvestigationCamera.SetActive(true);
            RoomConfigurations[selectedRoom].SearchCamera.SetActive(false);
            ArrestedTextUI.SetActive(false);
            InvestigationUI.SetActive(true);
        }
        else
        {
            ArrestUI.SetActive(false);
            tutoCont.CloseTutorials();
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
                tutoCont.CloseTutorials();
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
        isAllowPlacing = true;
        tutoCont.CheckTutorial(TutorialController.Tutorials.PlaceObject);
    }

    public void ShowFireWorks()
    {
        TapticManager.Notification(NotificationFeedback.Success);
        RoomConfigurations[selectedRoom].SearchCamera.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void NextDayButtonClick()
    {
        PlayerPrefs.SetInt("UseMenu", 0);
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

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("Pause");
        PlayerPrefs.SetInt("UseMenu", 1);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Pause");
        PlayerPrefs.SetInt("UseMenu", 1);
    }
}
