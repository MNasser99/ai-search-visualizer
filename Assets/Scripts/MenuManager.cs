using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject escMenu;

    string scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene().name;

        Screen.SetResolution(1920, 1080, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && scene == "main")
        {
            if (escMenu.activeInHierarchy)
                ViewEscMenu(false);
            else
                ViewEscMenu(true);
        } 
    }

    public void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public void ViewEscMenu(bool toView)
    {
        escMenu.SetActive(toView);
    }
}
