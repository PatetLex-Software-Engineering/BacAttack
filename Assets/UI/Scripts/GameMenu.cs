using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void NewGame() {
        Save.save = "game";
        SceneManager.LoadSceneAsync("Game");
    }

    public void Exit() {
        Application.Quit();
    }
}
