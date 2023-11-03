using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Variable Declaration
    [SerializeField] private bool isPlayer1;
    private bool isGrounded;

    [Header("UI")]
    public TextMeshProUGUI speedText;

    [Header("Zoom")]
    [SerializeField] private float rightBoundaryWidth;

    [Header("Speed")]
    public float initialSpeed; //Default speed at game start
    [HideInInspector] public float baseSpeed; //Default speed + bonus speed from energy orbs
     public float speed; //Current speed

    [Header("Gliding")]
    [SerializeField] private float glideSpeedIncreaseRate;
    [SerializeField] private float glideSpeedResetDuration;
    [SerializeField] private float glideGravityDecreaseRate;

    [Header("Jump")]
    [SerializeField] private float gravity;
    [SerializeField] private float minGravity;
    private float baseGravity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    private bool isJumpCooldown;

    [Header("Energy Orb")]
    [SerializeField] private float normalOrbSpeedIncrease;
    [SerializeField] private int hopsPerOrb;
    private int hops;
    [SerializeField] private float specialEnergyOrbChance; //a decimal as a percentage
    public GameObject normalEnergyOrb;
    public GameObject[] specialEnergyOrbList;
    #endregion

    #region Component Declaration
    private Rigidbody2D rb;
    Keyboard kb;

    public GameManager gameManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialize Variables
        isGrounded = false;

        baseSpeed = initialSpeed;
        speed = initialSpeed;
        UpdateSpeedText();

        isJumpCooldown = false;

        hops = 0;

        baseGravity = gravity;
        #endregion

        #region Assign Components
        rb = gameObject.GetComponent<Rigidbody2D>();
        kb = Keyboard.current;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

        #region Glide
        if (rb.velocity.y < 0f && !isGrounded)
        {
            StopCoroutine(ResetToBaseSpeed());
            speed += glideSpeedIncreaseRate * Time.deltaTime; //accelerate when gliding
            if (gravity > minGravity)
            {
                gravity -= glideGravityDecreaseRate * Time.deltaTime;
            }

            //Debug.Log("ended");
        }
        else
        {
            StartCoroutine(ResetToBaseSpeed()); //reset to base speed when no longer gliding
            gravity = baseGravity;
        }
        #endregion

        #region Player Actions
        if ((isPlayer1? kb.qKey.wasPressedThisFrame : kb.oKey.wasPressedThisFrame)&&!isJumpCooldown)
        {
            isGrounded = false;

            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(transform.up*jumpForce, ForceMode2D.Impulse);

            StartCoroutine(JumpCooldown());

            hops += 1;
            if (hops == hopsPerOrb)
            {
                hops = 0;
                SpawnEnergyOrb();
            }
        }
        #endregion

        UpdateSpeedText();
    }

    public void SetHorizontalSpeed(float speed)
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    #region Glide Function
    private IEnumerator ResetToBaseSpeed()
    {
        float elapsedTime = 0f;
        float startSpeed = speed;

        while (elapsedTime < glideSpeedResetDuration)
        {
            speed = Mathf.Lerp(startSpeed, baseSpeed, elapsedTime / glideSpeedResetDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        speed = baseSpeed;
    }
    #endregion

    #region Jump Functions

    private void ApplyGravity()
    {
        rb.velocity += new Vector2(0, -gravity)*Time.deltaTime;
    }

    private IEnumerator JumpCooldown()
    {
        isJumpCooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        isJumpCooldown = false;
    }
    #endregion

    #region Energy Orbs
    private void SpawnEnergyOrb()
    {
        if (Random.Range(0f, 1f) >= specialEnergyOrbChance)//spawn normal orb
        {
            Instantiate(normalEnergyOrb,transform.position-new Vector3(1.5f,0f,0f),Quaternion.identity);
        }
        else if (specialEnergyOrbList.Length>0) //spawn a random special orb
        {
            int orbIndex = Random.Range(0, specialEnergyOrbList.Length);
            Instantiate(specialEnergyOrbList[orbIndex], transform.position - new Vector3(1.5f, 0f, 0f), Quaternion.identity);
        }
    }
    #endregion

    #region UI Functions
    private void UpdateSpeedText()
    {
        speedText.text = Mathf.RoundToInt(speed).ToString() + " mph";
    }
    #endregion

    public float IsOutsideCameraView()
    {
        //returns 0 if player is in camera range, returns the player's x separation from the camera boundary if player is out of camera range

        Camera mainCamera = gameManager.mainCamera;
        float initialCameraSize = gameManager.initialCameraSize;
        
        float cameraHalfWidth = initialCameraSize * mainCamera.aspect;
        //float cameraHalfHeight = initialCameraSize;

        float deltaX = Mathf.Max(0, Mathf.Abs(transform.position.x - mainCamera.transform.position.x) + rightBoundaryWidth - cameraHalfWidth);
        //float deltaY = Mathf.Max(0, Mathf.Abs(transform.position.y - mainCamera.transform.position.y) - cameraHalfHeight);

        if (transform.position.x >= mainCamera.transform.position.x)
        {
            return deltaX;
        }
        else
        {
            return -deltaX;
        }
    }

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Energy Orb"))
        {
            baseSpeed += normalOrbSpeedIncrease;
            Destroy(collision.gameObject);
        }
    }
    #endregion
}
