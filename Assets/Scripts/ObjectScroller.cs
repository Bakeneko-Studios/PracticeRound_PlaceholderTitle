using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScroller : MonoBehaviour
{

    private float initialSpeed=8.4f;
    private float speed;
    private Rigidbody2D rb;

    private GameManager gameManager;

    private float selfDestructTime = 30f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = initialSpeed;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        speed = initialSpeed * (1+gameManager.obstacleSpeedIncreaseRate*gameManager.higherSpeed);
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTime);
        Destroy(gameObject);
    }
}
