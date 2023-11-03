using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyOrbController : MonoBehaviour
{

    [SerializeField] private float initialSpeed;
    private float speed;
    private Rigidbody2D rb;

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = initialSpeed;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = initialSpeed * (gameManager.backgroundScroller1.scrollSpeed/gameManager.initialScrollSpeed);
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }
}
