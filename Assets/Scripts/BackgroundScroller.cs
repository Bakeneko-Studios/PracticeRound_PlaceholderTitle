using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{

    [HideInInspector] public float scrollSpeed;
    private float offset;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset += (scrollSpeed * Time.deltaTime) / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
