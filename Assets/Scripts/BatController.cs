using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    private float speed;
    private float direction;

    private Rigidbody2D rb;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0, 2)) == 0 ? -1f:1f ; //assign either -1 of 1 to direction
        speed = Random.Range(minSpeed, maxSpeed);

        rb = GetComponent<Rigidbody2D>();
        

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= mainCamera.transform.position.x + 1f * mainCamera.orthographicSize * mainCamera.aspect)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed * direction);
        }
        if (direction==1 && transform.position.y>=6.8f || direction==-1 && transform.position.y<=-6.8f)
        {
            direction *= -1f;
        }
    }
}
