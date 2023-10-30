using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variable Declaration
    [Header("GameObjects")]
    public GameObject player1;
    public GameObject player2;

    [Header("Camera")]
    [HideInInspector] public Camera mainCamera;
    [SerializeField] private float cameraZoomSpeed;

    [HideInInspector] public float initialCameraSize;
    private PlayerController player1Controller;
    private PlayerController player2Controller;
    #endregion

    #region Component Declaration
    #endregion

    private void Awake()
    {
        #region Initialize Variables
        mainCamera = Camera.main;
        initialCameraSize = mainCamera.orthographicSize;

        player1Controller = player1.GetComponent<PlayerController>();
        player2Controller = player2.GetComponent<PlayerController>();
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AdjustCameraSize();
    }

    #region Camera Functions
    private void AdjustCameraSize()
    {
        float deltaXPlayer1 = player1Controller.IsOutsideCameraView();
        float deltaXPlayer2 = player2Controller.IsOutsideCameraView();

        if (Mathf.Max(deltaXPlayer1, deltaXPlayer2) >= 0)
        {
            //Debug.Log(deltaXPlayer1);
            Debug.Log(deltaXPlayer2);
            float newOrthographicSize = initialCameraSize + Mathf.Max(deltaXPlayer1, deltaXPlayer2);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthographicSize, Time.deltaTime * cameraZoomSpeed);
        }
    }
    #endregion
}
