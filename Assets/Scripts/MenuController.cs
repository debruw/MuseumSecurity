using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject Scene_Game, Scene_Start;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("UseMenu").Equals(1) || !PlayerPrefs.HasKey("UseMenu"))
        {
            //Scene_Start.SetActive(true);
            Scene_Game.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("UseMenu").Equals(0))
        {
            Scene_Start.SetActive(false);
            Scene_Game.SetActive(true);
        }
    }

    public void StartButtonClick()
    {
        GetComponent<Animator>().SetTrigger("Start");
    }

    public void ChangeScene()
    {

        Scene_Start.SetActive(false);
        Scene_Game.SetActive(true);

    }

    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.SetInt("UseMenu", 1);
    }
    private void OnApplicationQuit()
    {
        Debug.Log("Pause");
        PlayerPrefs.SetInt("UseMenu", 1);
    }
}
