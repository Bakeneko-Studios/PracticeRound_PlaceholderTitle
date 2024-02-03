using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public float spillWaitTime;
    private float spillAngle;
    private float[] angleList = { 45f, 135f, 215f, 305f };
    private GameObject cauldronIndicationAxis;
    private GameObject magicFluid;
    private Animator anim;

    [SerializeField] private float spillDuration;

    [HideInInspector] public bool isSpilled;
    // Start is called before the first frame update
    void Start()
    {
        isSpilled = false;

        cauldronIndicationAxis = transform.GetChild(1).gameObject;
        magicFluid = transform.GetChild(2).gameObject;
        magicFluid.SetActive(false);

        anim = transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();

        int index = Random.Range(0, angleList.Length);
        spillAngle = angleList[index];
        Debug.Log(spillAngle);

        cauldronIndicationAxis.transform.localEulerAngles = new Vector3(0f, 0f, spillAngle);

        StartCoroutine(Spill());
    }

    private IEnumerator Spill()
    {
        yield return new WaitForSeconds(spillWaitTime);

        cauldronIndicationAxis.SetActive(false);
        anim.SetTrigger("isLook");

        float elapsedTime = 0f;
        float startRotationZ = spillAngle<=180f? 0f:360f;
        float endRotationZ = spillAngle;
        while (elapsedTime <= spillDuration)
        {
            transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(startRotationZ, endRotationZ, elapsedTime / spillDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0f, 0f, spillAngle);

        
        magicFluid.SetActive(true);

        isSpilled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
