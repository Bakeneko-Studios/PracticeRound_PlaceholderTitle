using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TowerLight : MonoBehaviour
{

    private Light2D light;
    public float minIntensity;
    public float maxIntensity;
    private float dir;
    public float intensityChangeRate;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        light.intensity = minIntensity;
        dir = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity += intensityChangeRate * dir*Time.deltaTime;
        if (light.intensity >= maxIntensity || light.intensity <= minIntensity)
        {
            dir *= -1f;
        }
    }
}
