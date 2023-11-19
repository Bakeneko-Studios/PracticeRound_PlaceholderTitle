using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatIndicationController : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;
    private Renderer renderer;
    void Start()
    {
        mainCamera = Camera.main;
        renderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(mainCamera.transform.position.x + 1f * mainCamera.orthographicSize * mainCamera.aspect - renderer.bounds.size.x / 2f, 0f);
    }
}
