using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectileController : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public float speed;
    [HideInInspector] public float angle;

    private Rigidbody2D rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
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
}
