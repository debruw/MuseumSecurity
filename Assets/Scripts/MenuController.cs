using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartButtonClick()
    {
        GetComponent<Animator>().SetTrigger("Start");        
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Scene_Game");
    }
}
