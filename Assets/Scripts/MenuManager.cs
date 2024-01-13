using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;
    
    [Header("Menus")]
    public GameObject startingMenu;
    public GameObject selectModeMenu;
    public GameObject gameplayMenu;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        startingMenu.SetActive(false);
        selectModeMenu.SetActive(false);
        gameplayMenu.SetActive(false);

        displayStartingMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayStartingMenu()
    {
        startingMenu.SetActive(true);
        gameplayMenu.SetActive(false);
    }

    public void displayTutorial()
    {
        //TODO
    }

    public void displayOptions()
    {
        //TODO
    }

    public void quit()
    {
        Application.Quit();
    }

    public void displaySelectModeMenu()
    {
        startingMenu.SetActive(false);
        selectModeMenu.SetActive(true);
    }

    public void startGame()
    {
        selectModeMenu.SetActive(false);
        gameplayMenu.SetActive(true);
        gameManager.enterGameplay();
    }

}
