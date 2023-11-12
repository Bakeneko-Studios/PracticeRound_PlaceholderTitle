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
    public TextMeshProUGUI speedIncreaseText;
    [SerializeField] private float speedIncreaseTextAppearDuration;
    private Animator speedIncreaseTextAnimator;
    private bool isSpeedIncreaseTextAppearing;
    /*
    [SerializeField] private float rainbowTextSpeed;
    private float rainbowTextUpperBound;
    private float rainbowTextLowerBound;
    private string currentColor;
    private float colorDirection;
    */

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

    [Header("Obstacles")]
    //erializeField] private float towerKnockbackForce;
    private bool isTouchingTower;
    [SerializeField] private float touchingTowerJumpBoost; //increase jumpforce when touching tower
    [SerializeField] private float touchBatSwarmSpeedPenalty;

    [Header("Misc")]
    [SerializeField] private float touchGroundSpeedPenalty;
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

        speedIncreaseTextAnimator = speedIncreaseText.GetComponent<Animator>();
        isSpeedIncreaseTextAppearing = false;

        /*
        rainbowTextUpperBound = isPlayer1 ? 185f : 185f;
        rainbowTextLowerBound = isPlayer1 ? 0f : 0f;
        currentColor = isPlayer1 ? "R" : "G";
        colorDirection = isPlayer1 ? 1f : -1f;
        speedIncreaseText.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor,isPlayer1 ? new Color(0f,185,6f,255f):new Color(185f,185f,0f));
        */

        isJumpCooldown = false;

        hops = 0;

        baseGravity = gravity;

        isTouchingTower = false;
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
            rb.AddForce(transform.up*jumpForce*(isTouchingTower?touchingTowerJumpBoost:1f), ForceMode2D.Impulse);

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
        //SetSpeedIncreaseTextColor();
    }

    public void SetHorizontalSpeed(float speed)
    {
        if (!isTouchingTower)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
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
        /*
        int difference = Mathf.RoundToInt(speed)-int.Parse(speedText.text.Substring(0, speedText.text.Length - 4));
        if (difference > 0)
        {
            UpdateSpeedIncreaseText("+" + difference);
        }
        else if (difference < 0)
        {
            UpdateSpeedIncreaseText(difference.ToString());
        }
        */

        speedText.text = Mathf.RoundToInt(speed).ToString() + " mph";
    }

    private void UpdateSpeedIncreaseText(string text)
    {
        StopCoroutine(DeactivateSpeedIncreaseText());
        StartCoroutine(DeactivateSpeedIncreaseText());

        if (!isSpeedIncreaseTextAppearing)
        {
            speedIncreaseText.text = "+0";
            speedIncreaseTextAnimator.SetTrigger("Activate");
        }
        isSpeedIncreaseTextAppearing = true;

        if (speedIncreaseText.text[0]==text[0])
        {
            speedIncreaseText.text = speedIncreaseText.text[0] + (int.Parse(speedIncreaseText.text.Substring(1))+int.Parse(text.Substring(1))).ToString();
        }
        else
        {
            speedIncreaseText.text = text;
        }
        //Debug.Log(speedIncreaseText.text);
    }

    private IEnumerator DeactivateSpeedIncreaseText()
    {
        yield return new WaitForSeconds(speedIncreaseTextAppearDuration);

        isSpeedIncreaseTextAppearing = false;
        speedIncreaseTextAnimator.SetTrigger("Deactivate");
        //Debug.Log("stopped");
    }

    /*
    private void SetSpeedIncreaseTextColor()
    {
        if (speedIncreaseText.text[0].ToString() == "+")//Set Rainbow Text
        {
            Color newColor;
            if (currentColor == "R")
            {
                newColor = new Color(colorDirection* rainbowTextSpeed, 0f, 0f);
                if (speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor).r<=0 || speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor).r >= rainbowTextUpperBound)
                {
                    colorDirection = -colorDirection;
                    currentColor = "G";
                }
            }
            else if (currentColor == "G")
            {
                newColor = new Color(0f, colorDirection* rainbowTextSpeed, 0f);
                if (speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor).g <= 0 || speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor).g >= rainbowTextUpperBound)
                {
                    colorDirection = -colorDirection;
                    currentColor = "B";
                }
            }
            else
            {
                newColor = new Color(0f, 0f, colorDirection*rainbowTextSpeed);
                if (speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor).b <= 0 || speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor).b >= rainbowTextUpperBound)
                {
                    colorDirection = -colorDirection;
                    currentColor = "R";
                }
            }
            //Debug.Log(newColor);
            newColor += speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor);
            speedIncreaseText.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor, newColor);
            Debug.Log(speedIncreaseText.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor));
        }
        else
        {

        }
    }
    */  
    #endregion

    public float IsOutsideCameraView()
    {
        //returns 0 if player is in camera range, returns the player's x separation from the camera boundary if player is out of camera range

        Camera mainCamera = gameManager.mainCamera;
        float initialCameraSize = gameManager.initialCameraSize;
        
        float cameraHalfWidth = initialCameraSize * mainCamera.aspect;
        //float cameraHalfHeight = initialCameraSize;

        float deltaX = Mathf.Max(0, Mathf.Abs(transform.position.x - mainCamera.transform.position.x) - cameraHalfWidth);
        //float deltaY = Mathf.Max(0, Mathf.Abs(transform.position.y - mainCamera.transform.position.y) - cameraHalfHeight);

        if (transform.position.x >= mainCamera.transform.position.x)
        {
            return deltaX + rightBoundaryWidth;
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
            baseSpeed -= touchGroundSpeedPenalty;
            UpdateSpeedIncreaseText("-"+touchGroundSpeedPenalty);
        }
        else if (collision.gameObject.CompareTag("Tower"))
        {
            isTouchingTower = true;
            //rb.AddForce(new Vector2(-towerKnockbackForce, 0f), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            isTouchingTower = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Energy Orb"))
        {
            baseSpeed += normalOrbSpeedIncrease;
            UpdateSpeedIncreaseText("+" + normalOrbSpeedIncrease);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            baseSpeed -= touchBatSwarmSpeedPenalty;
            UpdateSpeedIncreaseText("-" + touchBatSwarmSpeedPenalty);
        }
        else if (collision.gameObject.CompareTag("Finish Line"))
        {
            gameManager.EndGame((isPlayer1) ? 1 : 2);
        }
    }
    #endregion
}
