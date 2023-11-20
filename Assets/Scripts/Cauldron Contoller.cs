using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronContoller : MonoBehaviour
{
    [SerializeField] private float spillWaitTime;
    private float spillAngle;
    private float[] angleList = { 45f, 135f, 215f, 305f };
    private GameObject cauldronIndicationAxis;
    private GameObject cauldronIndication;
    // Start is called before the first frame update
    void Start()
    {
        cauldronIndicationAxis = transform.GetChild(1).gameObject;
        cauldronIndication = cauldronIndicationAxis.transform.GetChild(0).gameObject;

        int index = Random.Range(0, angleList.Length);
        spillAngle = angleList[index];

        cauldronIndicationAxis.transform.rotation = new Quaternion(0f, 0f, spillAngle, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
