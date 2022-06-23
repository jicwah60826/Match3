using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{

    public string mainMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)){
            PlayerPrefs.DeleteAll();
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}
