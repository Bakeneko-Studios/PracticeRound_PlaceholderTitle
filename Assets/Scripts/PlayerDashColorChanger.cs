using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashColorChanger : MonoBehaviour
{
    public Color normalColor;
    public Color dashingColor;
    public float frequency;
    public float amplitude;

    private SpriteRenderer spriteRenderer;

    public bool isDashing;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            float time = Time.time * frequency;
            float t = (Mathf.Sin(time) * amplitude) + amplitude;

            spriteRenderer.color = Color.Lerp(normalColor, dashingColor, t);
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }
}
