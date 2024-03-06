using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;
    
    [Header("Menus")]
    public GameObject startingMenu;
    public GameObject selectModeMenu;
    public GameObject gameplayMenu;
    public GameObject mainMenuTower;
    public GameObject mainMenuP1;
    public GameObject mainMenuP2;
    public GameObject title;
    public GameObject tutorialUI;
    public GameObject creditsUI;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        startingMenu.SetActive(false);
        selectModeMenu.SetActive(false);
        gameplayMenu.SetActive(false);

        mainMenuP1.SetActive(true);
        mainMenuP2.SetActive(true);
        title.SetActive(true);

        tutorialUI.SetActive(false);
        creditsUI.SetActive(false);

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
        tutorialUI.SetActive(true);
    }

    public void hideTutorial()
    {
        tutorialUI.SetActive(false);
    }

    public void displayCredits()
    {
        creditsUI.SetActive(true);
    }

    public void hideCredits()
    {
        creditsUI.SetActive(false);
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
        mainMenuTower.SetActive(false);
        gameplayMenu.SetActive(true);
        gameManager.enterGameplay();

        mainMenuP1.SetActive(false);
        mainMenuP2.SetActive(false);
        title.SetActive(false);
    }

    public void returnMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
