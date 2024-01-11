using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectileController : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public float speed;
    [HideInInspector] public float angle;

    private Rigidbody2D rb;
    private Animator anim;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        angle *= Mathf.Deg2Rad;
        rb.velocity = speed*new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }

    public void hitDeath()
    {
        StartCoroutine(death());
    }

    private IEnumerator death()
    {
        rb.velocity = new Vector2(0f,0f);
        anim.SetTrigger("isDeath");

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")|| collision.gameObject.CompareTag("Tower")|| collision.gameObject.name == "Ground" || collision.gameObject.name == "Ceiling")
        {
            hitDeath();
        }
    }
}
