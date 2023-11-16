using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatIndicationSpawner : MonoBehaviour
{
    public GameObject batSwarmIndication;
    private GameObject indication;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        indication = Instantiate(batSwarmIndication, new Vector2(mainCamera.transform.position.x + 1f * mainCamera.orthographicSize * mainCamera.aspect, transform.position.y), Quaternion.identity);
        indication.transform.position -= new Vector3(indication.GetComponent<Renderer>().bounds.size.x / 2f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x<= mainCamera.transform.position.x + 1f * mainCamera.orthographicSize * mainCamera.aspect)
        {
            Destroy(indication);
        }
    }
    /*
    public void spawnIndication(float xPos)
    {

    }
    */
}
