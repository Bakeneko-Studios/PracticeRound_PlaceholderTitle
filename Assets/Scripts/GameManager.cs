using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Variable Declaration
    public string gameState;
    
    [Header("Audio")]
    public AudioSource inGameMusic;
    public AudioSource mainMenuMusic;
    public AudioSource mainMenuAmbience;
    public AudioSource gameEndMusic;
    private AudioLowPassFilter lowPassFilter;
    private float unmuffledFrequency;
    [SerializeField] private float muffledFrequency;
    [SerializeField] private float superMuffledFrequency;

    [Header("Win Conditions")]
    [SerializeField] private float outOfBoundsMaxSurviveTime;
    private float p1OutOfBoundsSurviveTime;
    private float p2OutOfBoundsSurviveTime;

    [Header("Misc GameObjects")]
    public GameObject player1;
    public GameObject player2;
    public GameObject finishLine;

    [Header("Camera")]
    [HideInInspector] public Camera mainCamera;
    [SerializeField] private float cameraZoomSpeed;
    [SerializeField] private float maxZoom;
    private bool isMaxZoom;
    [SerializeField] private float minZoom;
    private bool isMinZoom;

    [Header("Background")]
    //public float initialScrollSpeed;
    [SerializeField] private float scrollSpeedIncreaseRate; //one more unit in the player's speed = this much increase in scroll speed
    public BackgroundScroller treesMid;
    public BackgroundScroller treesLeft1;
    public BackgroundScroller treesLeft2;
    public BackgroundScroller treesRight1;
    public BackgroundScroller treesRight2;
    public BackgroundScroller mountainsMid;
    public BackgroundScroller mountainsLeft;
    public BackgroundScroller mountainsRight;

    [Header("Map")]
    [SerializeField] private float mapLength;
    private float player1Progress;
    private float player2Progress;

    [Header("Player Horizontal Speeds")]
    [SerializeField] private float slowSpeed;
    [SerializeField] private float moderateSpeed;
    [SerializeField] private float fastSpeed;
    [SerializeField] private float extremeSpeed;

    [SerializeField] private float relativeSpeedMultiplier; //player horizontal speed = relative speed * multiplier

    [HideInInspector] public float initialCameraSize;
    [HideInInspector] public PlayerController player1Controller;
    [HideInInspector] public PlayerController player2Controller;

    private float relativeSpeed;
    [HideInInspector] public float higherSpeed;

    [Header("UI")]
    public GameObject gamePlayUI;
    public TextMeshProUGUI returnToScreenText;
    public GameObject gameOverUI;
    public TextMeshProUGUI gameOverText;
    public GameObject deathBoom;
    public Slider progressBar;
    public RectTransform player1ProgressIcon;
    public RectTransform player2ProgressIcon;
    [SerializeField] private float progressIconInitialX;
    [SerializeField] private float progressIconFinalX;
    private float progressIconXDistance;
    public GameObject pauseIndicationText;
    public TextMeshProUGUI unpauseText;
    public GameObject pauseMenu;
    public GameObject crystalBall;

    [Header("Tower")]
    public GameObject tower;
    [SerializeField] private float minSpawnTowerInterval;
    [SerializeField] private float maxSpawnTowerInterval;
    [SerializeField] private float maxTowerY;
    [SerializeField] private float minTowerY;
    [SerializeField] private float towerSpawnXPos; //Distance between tower's x pos and the camera's right boundary

    [Header("Obstacles")]
    public float obstacleSpeedIncreaseRate;
    [SerializeField] private float initialObstacleIntensity;
    [SerializeField] private float obstacleIntensityIncreaseRate; //amount the obstacle intensity increases by
    [SerializeField] private float obstacleIntensityIncreaseInterval; //interval (seconds) at which obstacle internsity increases
    private float obstacleIntensity;
    private float actualObstacleIntensity; // equals 1 / obstacleIntensity
    [SerializeField] private float minSpawnObstacleInterval;
    [SerializeField] private float maxSpawnObstacleInterval;

    [Header("BatSwarm")]
    public GameObject batSwarm;
    //public GameObject batSwarmIndication;
    [SerializeField] private float doubleBatBaseChance;
    [SerializeField] private float tripleBatBaseChance;
    [SerializeField] private float quadrupleBatBaseChance;
    [SerializeField] private float maxBatSwarmY;
    [SerializeField] private float minBatSwarmY;
    [SerializeField] private float maxBatSwarmX;
    [SerializeField] private float minBatSwarmX;

    [Header("Cauldron")]
    public GameObject cauldron;
    [SerializeField] private float minCauldronY;
    [SerializeField] private float maxCauldronY;

    [Header("EndGameSlowdown")]
    [SerializeField] private float slowdownDuration;
    [SerializeField] private float startVal;
    [SerializeField] private float endVal;

    [Header("Postprocessing")]
    public GameObject p1WinPp;
    public GameObject p2WinPp;

    [Header("MainMenuBats")]
    [SerializeField] private float minMMBatInterval;
    [SerializeField] private float maxMMBatInterval;

    private bool isPaused;
    private bool isSetMapLength;
    #endregion

    #region Component Declaration
    Keyboard kb;
    #endregion

    #region Constants
    private float deathRange;//how much to camera, 25 is out of bounds in 16:9
    private bool gameOver;
    #endregion


    private void Awake()
    {
        #region Initialize Variables
        Time.timeScale = 1;

        isSetMapLength = false;

        gameState = "MainMenu";

        lowPassFilter = inGameMusic.gameObject.GetComponent<AudioLowPassFilter>();
        unmuffledFrequency = lowPassFilter.cutoffFrequency;

        mainCamera = Camera.main;
        initialCameraSize = mainCamera.orthographicSize;
        isMaxZoom = false;
        isMinZoom = false;

        player1Controller = player1.GetComponent<PlayerController>();
        player2Controller = player2.GetComponent<PlayerController>();

        /*
        backgroundScroller1 = background1.GetComponent<BackgroundScroller>();
        backgroundScroller2 = background2.GetComponent<BackgroundScroller>();
        backgroundScroller3 = background3.GetComponent<BackgroundScroller>();
        backgroundScroller4 = background4.GetComponent<BackgroundScroller>();
        backgroundScroller5 = background5.GetComponent<BackgroundScroller>();
        */

        player1Progress = 0f;
        player2Progress = 0f;
        progressBar.value = 0f;
        player1ProgressIcon.anchoredPosition = new Vector2(progressIconInitialX, player1ProgressIcon.anchoredPosition.y);
        player2ProgressIcon.anchoredPosition = new Vector2(progressIconInitialX, player2ProgressIcon.anchoredPosition.y);
        progressIconXDistance = progressIconFinalX - progressIconInitialX;

        gameOver = false;
        deathRange = -30;

        obstacleIntensity = initialObstacleIntensity;
        actualObstacleIntensity = 1f / obstacleIntensity;

        kb = Keyboard.current;

        isPaused = false;

        p1OutOfBoundsSurviveTime = 0f;
        p2OutOfBoundsSurviveTime=0f;
        #endregion

        player1.SetActive(false);
        player2.SetActive(false);

        returnToScreenText.gameObject.SetActive(false);

        StartCoroutine(BeginMainMenuMusic());
        mainMenuAmbience.Play();

        StartCoroutine(SpawnMainMenuBats());
    }

    // Start is called before the first frame update
    void Start()
    {
        lowPassFilter.cutoffFrequency = unmuffledFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (gameState == "Gameplay")
        {
            checkMaxZoom();

            calculateSpeed();
            AdjustBackgroundScrollSpeed();
            AdjustPlayerSpeed();

            UpdatePlayerProgress();

            AdjustCameraSize();

            PlayerWinCondition();

            if (kb.escapeKey.wasPressedThisFrame)
            {
                Pause();
            }
        }
        
        //Debug.Log(gameOver);
    }

    #region GameState Functions
    public void enterGameplay()
    {
        mainMenuMusic.Stop();
        mainMenuAmbience.Stop();
        inGameMusic.Play();

        player1.SetActive(true);
        player2.SetActive(true);
        gameState = "Gameplay";

        StartCoroutine(SpawnTowers());
        StartCoroutine(IncreaseObstacleIntensity());
        StartCoroutine(SpawnObstacles());


        /*
        backgroundScroller1.scrollSpeed = initialScrollSpeed;
        backgroundScroller2.scrollSpeed = initialScrollSpeed;
        backgroundScroller3.scrollSpeed = initialScrollSpeed;
        backgroundScroller4.scrollSpeed = initialScrollSpeed;
        backgroundScroller5.scrollSpeed = initialScrollSpeed;
        */

        treesMid.scrollSpeed = treesMid.initialScrollSpeed;
        treesLeft1.scrollSpeed = treesLeft1.initialScrollSpeed;
        treesLeft2.scrollSpeed = treesLeft2.initialScrollSpeed;
        treesRight1.scrollSpeed = treesRight1.initialScrollSpeed;
        treesRight2.scrollSpeed = treesRight2.initialScrollSpeed;
        mountainsMid.scrollSpeed = mountainsMid.initialScrollSpeed;
        mountainsLeft.scrollSpeed = mountainsLeft.initialScrollSpeed;
        mountainsRight.scrollSpeed = mountainsRight.initialScrollSpeed;

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Obstacle"); //Destroy all bats

        foreach (GameObject obj in objectsWithTag)
        {
            Destroy(obj);
        }
    }
    #endregion

    #region Pause Functions
    private void Pause()
    {
        if (!isPaused)
        {
            lowPassFilter.cutoffFrequency = muffledFrequency;
            pauseIndicationText.SetActive(false);
            gamePlayUI.SetActive(false);
            //TODO: Show pause menu
            pauseMenu.SetActive(true);
            crystalBall.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
        else
        {
            StartCoroutine(Unpause());
            //TODO: Hide pause menu
            pauseMenu.SetActive(false);
            crystalBall.SetActive(false);
        }
    }

    private IEnumerator Unpause()
    {
        gamePlayUI.SetActive(true);
        unpauseText.gameObject.SetActive(true);
        unpauseText.text = "3";
        yield return new WaitForSecondsRealtime(1);
        unpauseText.text = "2";
        yield return new WaitForSecondsRealtime(1);
        unpauseText.text = "1";
        yield return new WaitForSecondsRealtime(1);
        unpauseText.gameObject.SetActive(false);
        pauseIndicationText.SetActive(true);
        Time.timeScale = 1;
        isPaused = false;
        lowPassFilter.cutoffFrequency = unmuffledFrequency;
    }
    #endregion

    private void UpdatePlayerProgress()
    {
        player1Progress += player1Controller.speed * Time.deltaTime;
        player2Progress += player2Controller.speed * Time.deltaTime;

        if (player1Progress >= mapLength || player2Progress >= mapLength) 
        {
            finishLine.SetActive(true);
        }

        progressBar.value = (player1Progress >= player2Progress ? player1Progress : player2Progress)/mapLength;
        player1ProgressIcon.anchoredPosition = new Vector2(progressIconInitialX+progressIconXDistance*(player1Progress)/mapLength,0f);
        player2ProgressIcon.anchoredPosition = new Vector2(progressIconInitialX + progressIconXDistance * (player2Progress) / mapLength, 0f);
    }

    #region Camera Functions
    private void AdjustCameraSize()
    {
        
        float deltaXPlayer1 = player1Controller.IsOutsideCameraView();
        float deltaXPlayer2 = player2Controller.IsOutsideCameraView();
        //Debug.Log("Player 1 delta x: " + deltaXPlayer1);
        //Debug.Log("Player 2 delta x: " + deltaXPlayer2);

        float largerDelta = (Mathf.Abs(deltaXPlayer1) > Mathf.Abs(deltaXPlayer2) ? deltaXPlayer1 : deltaXPlayer2);
        if (largerDelta!= 0)
        {
            float newOrthographicSize = initialCameraSize + largerDelta;
            if (!(isMaxZoom && newOrthographicSize>mainCamera.orthographicSize|| isMinZoom && newOrthographicSize < mainCamera.orthographicSize))
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
        if (mainCamera.orthographicSize <= minZoom)
        {
            isMinZoom = true;
            mainCamera.orthographicSize = minZoom;
        }
        else
        {
            isMinZoom = false;
        }
    }
    #endregion

    #region Speed Functions

    public void setSlowSpeed()
    {
        player1Controller.initialSpeed = slowSpeed;
        player2Controller.initialSpeed = slowSpeed;
        if (!isSetMapLength)
        {
            mapLength *= (slowSpeed / moderateSpeed);
            isSetMapLength = true;
        }
    }

    public void setModerateSpeed()
    {
        player1Controller.initialSpeed = moderateSpeed;
        player2Controller.initialSpeed = moderateSpeed;
        if (!isSetMapLength)
        {
            mapLength *= (moderateSpeed / moderateSpeed);
            isSetMapLength = true;
        }
           
    }

    public void setFastSpeed()
    {
        player1Controller.initialSpeed = fastSpeed;
        player2Controller.initialSpeed = fastSpeed;
        
        if (!isSetMapLength)
        {
            mapLength *= (fastSpeed / moderateSpeed);
            isSetMapLength = true;
        }
    }

    public void setExtremeSpeed()
    {
        player1Controller.initialSpeed = extremeSpeed;
        player2Controller.initialSpeed = extremeSpeed;
        if (!isSetMapLength)
        {
            mapLength *= (extremeSpeed / moderateSpeed);
            isSetMapLength = true;
        }
    }

    private void calculateSpeed()
    {
        relativeSpeed = Mathf.Abs(player1Controller.speed - player2Controller.speed);
        higherSpeed = Mathf.Max(player1Controller.speed, player2Controller.speed);
    }

    private void AdjustBackgroundScrollSpeed()
    {
        treesMid.scrollSpeed = treesMid.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        treesLeft1.scrollSpeed = treesLeft1.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        treesLeft2.scrollSpeed = treesLeft2.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        treesRight1.scrollSpeed = treesRight1.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        treesRight2.scrollSpeed = treesRight2.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        mountainsMid.scrollSpeed = mountainsMid.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        mountainsLeft.scrollSpeed = mountainsLeft.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        mountainsRight.scrollSpeed = mountainsRight.initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);

        /*
        backgroundScroller1.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        backgroundScroller2.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed );
        backgroundScroller3.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        backgroundScroller4.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        backgroundScroller5.scrollSpeed = initialScrollSpeed + scrollSpeedIncreaseRate * (higherSpeed);
        */
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

    #region Obstacle Functions
    private IEnumerator SpawnTowers()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTowerInterval, maxSpawnTowerInterval));

            float xPos = mainCamera.transform.position.x + 2f * mainCamera.orthographicSize * mainCamera.aspect +towerSpawnXPos;
            float yPos = Random.Range(minTowerY, maxTowerY);
            Instantiate(tower, new Vector3(xPos, yPos, 0f), Quaternion.identity);
        }
    }

    private IEnumerator IncreaseObstacleIntensity()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(obstacleIntensityIncreaseInterval);

            obstacleIntensity += obstacleIntensityIncreaseRate;
            actualObstacleIntensity = 1f / obstacleIntensity;
        }
    }

    private IEnumerator SpawnObstacles()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnObstacleInterval, maxSpawnObstacleInterval) * actualObstacleIntensity);

            int obstacleToSpawn = Random.Range(0, 2);
            
            if (obstacleToSpawn == 0) 
            {
                SpawnBat();
            }
            else if (obstacleToSpawn == 1)
            {
                SpawnCauldron();
            }
        }
    }

    private IEnumerator SpawnMainMenuBats()
    {
        while (gameState == "MainMenu")
        {
            SpawnBat();

            yield return new WaitForSeconds(Random.Range(minMMBatInterval, maxMMBatInterval));
        }
    }

    private void SpawnBat()
    {
        float random = Random.Range(0f, 1f);
        int numberOfBats;

        if (random <= quadrupleBatBaseChance* actualObstacleIntensity) //Spawn 4 Bats
        {
            numberOfBats = 4;
        }
        else if (random <= (quadrupleBatBaseChance + tripleBatBaseChance) * actualObstacleIntensity) //Spawn 3 Bats
        {
            numberOfBats = 3;
        }
        else if (random <= (quadrupleBatBaseChance + tripleBatBaseChance + doubleBatBaseChance) * actualObstacleIntensity) //Spawn 2 Bats
        {
            numberOfBats = 2;
        }
        else
        {
            numberOfBats = 1;
        }

        for (int i = 0; i < numberOfBats; i++)
        {
            GameObject swarm=Instantiate(batSwarm, new Vector3(
                mainCamera.transform.position.x + 1f * mainCamera.orthographicSize * mainCamera.aspect + Random.Range(minBatSwarmX, maxBatSwarmX),
                Random.Range(minBatSwarmY, maxBatSwarmY), 0f),
                Quaternion.identity);
            if (gameState == "MainMenu")
            {
                swarm.GetComponent<BatIndicationSpawner>().isDeactivate = true;
            }
        }
    }

    private void SpawnCauldron()
    {
        GameObject spawnedCauldron=Instantiate(cauldron, new Vector3(mainCamera.transform.position.x + 1f * mainCamera.orthographicSize * mainCamera.aspect + 15f,
            Random.Range(minCauldronY, maxCauldronY), 0f), Quaternion.identity);

        spawnedCauldron.GetComponent<Cauldron>().spillWaitTime *= (player1Controller.initialSpeed/moderateSpeed);
    }
    #endregion

    #region Win Condition
    private void PlayerWinCondition()
    {
        if (!gameOver){
            if (mainCamera.WorldToViewportPoint(player1.transform.position).x<0)
            {
                returnToScreenText.gameObject.SetActive(true);
                p1OutOfBoundsSurviveTime += Time.deltaTime;
                returnToScreenText.text = "Get back on screen!\n"+(Mathf.Round(outOfBoundsMaxSurviveTime - p1OutOfBoundsSurviveTime) + 1);
                if (p1OutOfBoundsSurviveTime >= outOfBoundsMaxSurviveTime)
                {
                    Instantiate(deathBoom, new Vector3(-25, player1.transform.position.y, 0), Quaternion.identity);
                    EndGame(2);
                }
                //Time.timeScale = 0;
            }
            else
            {
                if (p1OutOfBoundsSurviveTime != 0f)
                {
                    returnToScreenText.gameObject.SetActive(false);
                }
                p1OutOfBoundsSurviveTime = 0f;
            }

            if (mainCamera.WorldToViewportPoint(player2.transform.position).x < 0)
            {
                returnToScreenText.gameObject.SetActive(true);
                p2OutOfBoundsSurviveTime += Time.deltaTime;
                returnToScreenText.text = "Get back on screen!\n" + (Mathf.Round(outOfBoundsMaxSurviveTime - p2OutOfBoundsSurviveTime) + 1);
                if (p2OutOfBoundsSurviveTime >= outOfBoundsMaxSurviveTime)
                {
                    Instantiate(deathBoom, new Vector3(-25, player2.transform.position.y, 0), Quaternion.identity);
                    EndGame(1);
                }
                //Time.timeScale = 0;
            }
            else
            {
                if (p2OutOfBoundsSurviveTime != 0f)
                {
                    returnToScreenText.gameObject.SetActive(false);
                }
                p2OutOfBoundsSurviveTime = 0f;
                
            }
        }
    }

    public void EndGame(int winner)
    {
        Debug.Log("Player1: " + player1Controller.IsOutsideCameraView());
        Debug.Log("Player2: " + player2Controller.IsOutsideCameraView());
        Debug.Log("game over");

        gamePlayUI.SetActive(false);
        if (winner == 1)
        {
            p1WinPp.SetActive(true);
        }
        else
        {
            p2WinPp.SetActive(true);
        }

        returnToScreenText.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        gameOverText.text = "Player " + winner + " Wins";
        gameOver = true;

        gameEndMusic.Play();
        lowPassFilter.cutoffFrequency = superMuffledFrequency;
        //inGameMusic.Stop();
        
        StartCoroutine(EndGameSlowDown());
    }

    private IEnumerator EndGameSlowDown()
    {
        float elapsedTime = 0f;

        while (elapsedTime < slowdownDuration)
        {
            Time.timeScale = Mathf.Lerp(startVal, endVal, elapsedTime / slowdownDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        //Time.timeScale=1;
        gameOver = false;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    #endregion

    private IEnumerator BeginMainMenuMusic()
    {
        yield return new WaitForSeconds(2f);
        if (gameState == "MainMenu")
        {
            mainMenuMusic.Play();
        }
    }
}
