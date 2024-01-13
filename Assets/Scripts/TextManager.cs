using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public GameObject canvas;
    private RectTransform canvasRectTransform;

    public GameObject player1SpeedChangeText;
    public GameObject player2SpeedChangeText;

    public GameObject getThunderboltText;
    public GameObject getIceText;
    public GameObject getDashText;

    [SerializeField] private float spellTextDuration;
    [SerializeField] private float speedTextDuration;

    [SerializeField] private float abovePlayerPos;

    private bool isP1TextAlready;
    private bool isP2TextAlready;
    
    // Start is called before the first frame update
    void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        isP1TextAlready = false;
        isP2TextAlready = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowThunderboltText(Vector3 playerWorldPos, bool isP1)
    {
        GameObject thunderboltTextInstance = Instantiate(getThunderboltText, canvas.transform);

        thunderboltTextInstance.GetComponent<RectTransform>().localPosition = playerWorldPos;
        //thunderboltTextInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, getYPos(isP1));
        StartCoroutine(HideSpellText(thunderboltTextInstance));
    }

    public void ShowIceText(Vector3 playerWorldPos, bool isP1)
    {
        GameObject iceTextInstance = Instantiate(getIceText, canvas.transform);

        iceTextInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, getYPos(isP1));
        StartCoroutine(HideSpellText(iceTextInstance));
    }

    public void ShowDashText(Vector3 playerWorldPos, bool isP1)
    {
        GameObject dashTextInstance = Instantiate(getDashText, canvas.transform);

        dashTextInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, getYPos(isP1));
        StartCoroutine(HideSpellText(dashTextInstance));
    }
    private IEnumerator HideSpellText(GameObject textInstance)
    {
        yield return new WaitForSeconds(spellTextDuration);
        Destroy(textInstance);
    }

    private float getYPos(bool isP1)
    {
        return abovePlayerPos + ((isP1 && isP1TextAlready) || (!isP1 && isP2TextAlready) ? 25f : 0f);
    }

    private Vector2 WorldToCanvasPos(Vector3 playerWorldPos)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerWorldPos);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, Camera.main, out canvasPosition);
        return canvasPosition;
    }
}
