using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectileController : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public float speed;

    private Rigidbody2D rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed, rb.velocity.y);

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
}
