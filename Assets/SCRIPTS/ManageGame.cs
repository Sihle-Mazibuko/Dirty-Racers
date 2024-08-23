using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManageGame : MonoBehaviour
{
    [SerializeField] GameObject needleImg;
    float startPos = 228 , endPos = -43;
    float newPos;
    public float carSpeed;

    public ControllerTwo pControll;

    [SerializeField] TextMeshProUGUI speedTxt;


    [SerializeField] Slider nosSlider;

 

    private void FixedUpdate()
    {
        speedTxt.text = pControll.kmph.ToString("0") ;
        carSpeed = pControll.kmph;
        UpdateNeedle();
        NitrosSlider();

    }

    void UpdateNeedle(){

        newPos = startPos - endPos;

        float tempF = carSpeed / 180;

        needleImg.transform.eulerAngles = new Vector3(0,0,(startPos - tempF * newPos));



    }

    public void NitrosSlider()
    {
        nosSlider.value = pControll.nosValue / 24.5f; 
    }
}
