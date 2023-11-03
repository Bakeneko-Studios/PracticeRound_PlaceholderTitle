using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variable Declaration
    [Header("GameObjects")]
    public GameObject player1;
    public GameObject player2;
    public GameObject background1;
    public GameObject background2;
    public GameObject background3;

    [Header("Camera")]
    [HideInInspector] public Camera mainCamera;
    [SerializeField] private float cameraZoomSpeed;
    [SerializeField] private float maxZoom;
    private bool isMaxZoom;

    [Header("Background")]
    public float initialScrollSpeed;
    [SerializeField] private float scrollSpeedIncreaseRate; //one more unit in the player's speed = this much increase in scroll speed

    [Header("Player Horizontal Speeds")]
    [SerializeField] private float relativeSpeedMultiplier; //player horizontal speed = relative speed * multiplier

    [HideInInspector] public float initialCameraSize;
    private PlayerController player1Controller;
    private PlayerController player2Controller;
    [HideInInspector] public BackgroundScroller backgroundScroller1;
    private BackgroundScroller backgroundScroller2;
    private BackgroundScroller backgroundScroller3;

    private float relativeSpeed;
    private float higherSpeed;
    #endregion

    [Header("UI")]
    public GameObject deathUI;
    public GameObject deathBoom;


    #region Component Declaration
    #endregion

    #region Constants
    private float deathRange;//how much to camera, 25 is out of bounds in 16:9
    private bool gameOver;
    #endregion


    private void Awake()
    {
        #region Initialize Variables
        mainCamera = Camera.main;
        initialCameraSize = mainCamera.orthographicSize;
        isMaxZoom = false;

        player1Controller = player1.GetComponent<PlayerController>();
        player2Controller = player2.GetComponent<PlayerController>();

        backgroundScroller1 = background1.GetComponent<BackgroundScroller>();
        backgroundScroller2 = background2.GetComponent<BackgroundScroller>();
        backgroundScroller3 = background3.GetComponent<BackgroundScroller>();

        backgroundScroller1.scrollSpeed = initialScrollSpeed; 
        backgroundScroller2.scrollSpeed = initialScrollSpeed;
        backgroundScroller3.scrollSpeed = initialScrollSpeed;

        gameOver = false;
        deathRange = -26;
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {      
        checkMaxZoom();

        calculateSpeed();
        AdjustBackgroundScrollSpeed();
        AdjustPlayerSpeed();

        AdjustCameraSize();

        PlayerWinCondition();

        Debug.Log(gameOver);
    }

    #region Camera Functions
    private void AdjustCameraSize()
    {
        
        float deltaXPlayer1 = player1Controller.IsOutsideCameraView();
        float deltaXPlayer2 = player2Controller.IsOutsideCameraView();
        //Debug.Log("Player 1 delta x: " + deltaXPlayer1);
        //Debug.Log("Player 2 delta x: " + deltaXPlayer2);

        if (Mathf.Max(deltaXPlayer1, deltaXPlayer2) >= 0)
        {
            float newOrthographicSize = initialCameraSize + Mathf.Max(deltaXPlayer1, deltaXPlayer2);
            if (!isMaxZoom || newOrthographicSize < mainCamera.orthographicSize)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthographicSize, Time.deltaTime * cameraZoomSpeed);
            }
        }
    }

    private void checkMaxZoom()
    {
        if (mainCamera.orthographicSize >= maxZoom)
        {
            isMaxZoom = true;
            mainCamera.orthographicSize = maxZoom;
        }
        else
        {
            isMaxZoom = false;
        }
    }
    #endregion

    #region Speed Functions
    private void calculateSpeed()
    {
        relativeSpeed = Mathf.Abs(player1Controller.speed - player2Controller.speed);
        higherSpeed = Mathf.Max(player1Controller.speed, player2Controller.speed);
    }

    private void AdjustBackgroundScrollSpeed()
    {
        backgroundScroller1.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed - player1Controller.initialSpeed);
        backgroundScroller2.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed - player1Controller.initialSpeed);
        backgroundScroller3.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed - player1Controller.initialSpeed);
    }

    private void AdjustPlayerSpeed()
    {
        if (!isMaxZoom)
        {
            ((player1Controller.speed >= player2Controller.speed) ? player1Controller : player2Controller)
                .SetHorizontalSpeed(relativeSpeed * relativeSpeedMultiplier);
            ((player1Controller.speed <= player2Controller.speed) ? player1Controller : player2Controller)
                .SetHorizontalSpeed(0);
        }
        else
        {
            ((player1Controller.speed <= player2Controller.speed) ? player1Controller : player2Controller)
                .SetHorizontalSpeed(-relativeSpeed * relativeSpeedMultiplier);
            ((player1Controller.speed >= player2Controller.speed) ? player1Controller : player2Controller)
                .SetHorizontalSpeed(0);
        }
    }
    #endregion

    #region Win Codition
    private void PlayerWinCondition()
    {
        if (!gameOver){
            if (player1Controller.IsOutsideCameraView() < deathRange){
                Instantiate(deathBoom, new Vector3(-25, player1.transform.position.y, 0), Quaternion.identity);
                deathUI.SetActive(true);
                gameOver = true;  
            }
            if (player2Controller.IsOutsideCameraView() < deathRange){
                Instantiate(deathBoom, new Vector3(-25, player1.transform.position.y, 0), Quaternion.identity);
                deathUI.SetActive(true);
                gameOver = true;
            }
        }
    }
    public void RestartGame()
    {
        //Time.timeScale=1;
        gameOver = false;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    #endregion
}
