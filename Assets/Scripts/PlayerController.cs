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

    [Header("Obstacles")]
    [SerializeField] private float touchingTowerJumpBoost; //increase jumpforce when touching tower
    [SerializeField] private float touchBatSwarmSpeedPenalty;
    private bool isTouchingTower;

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

    private enum Spell
    {
        unequipped, thunderbolt, dash, ice, shield
    }
    private Spell equippedSpell;
    [Header("Spells")]
    public GameObject thunderbolt;
    [SerializeField] private float thunderboltAppearDuration;
    [SerializeField] private float thunderboltSpeedPenalty;
    [SerializeField] private float thunderboltStunDuration;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashSpeedIncrease;
    private bool isDashing;
    public GameObject iceProjectile;
    [SerializeField] private float iceSpellSpeedPenalty;
    [SerializeField] private float iceProjectileSpeed;
    [SerializeField] private int iceProjectileCount;
    private int iceProjectilesLeft;
    [SerializeField] private float shieldDuration;

    [Header("Misc")]
    [SerializeField] private float touchGroundSpeedPenalty;
    private bool isStunned;
    #endregion

    #region Component Declaration
    private Rigidbody2D rb;
    private Collider2D collider;
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

        equippedSpell = Spell.dash;

        isStunned = false;
        isDashing = false;
        #endregion

        #region Assign Components
        rb = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<Collider2D>();
        kb = Keyboard.current;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

        #region Glide
        if (!isDashing)
        {
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
        }
        else
        {
            StopCoroutine(ResetToBaseSpeed());
        }
        
        #endregion

        #region Player Actions
        if ((isPlayer1? kb.qKey.wasPressedThisFrame : kb.oKey.wasPressedThisFrame)&&!isJumpCooldown && !isStunned &&!isDashing) //Jump
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

        if ((isPlayer1 ? kb.wKey.wasPressedThisFrame : kb.pKey.wasPressedThisFrame)&&!isStunned && !isDashing)//Cast Spell
        {
            switch (equippedSpell)
            {
                case Spell.thunderbolt:
                    StartCoroutine(CastThunderbolt());
                    break;
                case Spell.dash:
                    StartCoroutine(CastDash());
                    break;
                case Spell.ice:
                    CastIceSpell();
                    break;
                case Spell.shield:
                    Shield();
                    break;
            }
            
        }
        #endregion
        if (isDashing)
        {
            Debug.Log(speed);
        }
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
            if (!isDashing) speed = Mathf.Lerp(startSpeed, baseSpeed, elapsedTime / glideSpeedResetDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isDashing)  speed = baseSpeed;
    }
    #endregion

    #region Jump Functions

    private void ApplyGravity()
    {
        if (!isDashing)
        {
            rb.velocity += new Vector2(0, -gravity) * Time.deltaTime;
        }
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
            //Debug.Log(orbIndex);
            Instantiate(specialEnergyOrbList[orbIndex], transform.position - new Vector3(1.5f, 0f, 0f), Quaternion.identity);
        }
    }
    #endregion

    #region Spell Functions
    private IEnumerator CastThunderbolt()
    {
        thunderbolt.SetActive(true);
        equippedSpell = Spell.unequipped;
        yield return new WaitForSeconds(thunderboltAppearDuration);
        thunderbolt.SetActive(false);
    }

    private IEnumerator GetHitThunderbolt()
    {
        baseSpeed -= thunderboltSpeedPenalty;
        UpdateSpeedIncreaseText("-" + thunderboltSpeedPenalty);
        Debug.Log("Hit");
        isStunned = true;
        yield return new WaitForSeconds(thunderboltStunDuration);
        isStunned = false;
    }

    private IEnumerator CastDash()
    {
        equippedSpell = Spell.unequipped;

        isDashing = true;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacles"));
        speed += dashSpeedIncrease;
        //Debug.Log(speed);
        //StopCoroutine(ResetToBaseSpeed());
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacles"),false);
        //Debug.Log(speed);
        speed -= dashSpeedIncrease;
        //StartCoroutine(ResetToBaseSpeed());
    }

    private void CastIceSpell()
    {
        GameObject projectile = Instantiate(iceProjectile,transform.position+new Vector3(1.75f,0f,0f),Quaternion.identity);
        projectile.GetComponent<IceProjectileController>().speed = iceProjectileSpeed;
        iceProjectilesLeft -= 1;
        if (iceProjectilesLeft <= 0)
        {
            equippedSpell = Spell.unequipped;
        }
    }

    private void Shield()
    {
        equippedSpell = Spell.unequipped;
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
        if (collision.gameObject.CompareTag("Energy Orb") &&!isDashing)
        {
            baseSpeed += normalOrbSpeedIncrease;
            UpdateSpeedIncreaseText("+" + normalOrbSpeedIncrease);
            

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle") && !isDashing)
        {
            baseSpeed -= touchBatSwarmSpeedPenalty;
            UpdateSpeedIncreaseText("-" + touchBatSwarmSpeedPenalty);
        }
        else if (collision.gameObject.CompareTag("Ice Projectile"))
        {
            baseSpeed -= iceSpellSpeedPenalty;
            UpdateSpeedIncreaseText("-" + iceSpellSpeedPenalty);
            speed = baseSpeed;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Finish Line"))
        {
            gameManager.EndGame((isPlayer1) ? 1 : 2);
        }

        if (!isDashing)
        {
            switch (collision.gameObject.name)
            {
                case "ThunderboltEnergyOrb":
                    equippedSpell = Spell.thunderbolt;
                    break;
                case "DashEnergyOrb":
                    equippedSpell = Spell.dash;
                    break;
                case "IceEnergyOrb":
                    equippedSpell = Spell.ice;
                    iceProjectilesLeft = iceProjectileCount;
                    break;
                case "ShieldEnergyOrb":
                    equippedSpell = Spell.shield;
                    break;

                case "P1Thunderbolt":
                    if (!isPlayer1)
                    {
                        StartCoroutine(GetHitThunderbolt());
                    }
                    break;
                case "P2Thunderbolt":
                    if (isPlayer1)
                    {
                        StartCoroutine(GetHitThunderbolt());
                    }
                    break;
                /*
                case "IceProjectile":
                    baseSpeed -= iceSpellSpeedPenalty;
                    UpdateSpeedIncreaseText("-" + iceSpellSpeedPenalty);
                    speed = baseSpeed;
                    Destroy(collision.gameObject);
                    break;
                */
            }
        }
        
     }
    #endregion
}
