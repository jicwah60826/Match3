using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{

    public string mainMenu;

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}
