using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    GameObject player;
    GameObject child;
    [SerializeField] float followSpeed;
    ControllerTwo pControl;
    [ SerializeField,Range(0,5)] float smoothTime= 0;

    float defaultFOV = 0, newFOV = 85;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        child = player.transform.Find("CamFollow").gameObject;
        pControl = player.GetComponent<ControllerTwo>();
        defaultFOV = Camera.main.fieldOfView;
    }

    private void FixedUpdate()
    {
        FollowPlayer();
        IncreaseFOV();

    }

    void FollowPlayer()
    {
        if(followSpeed <= 20)
        followSpeed = Mathf.Lerp(followSpeed, pControl.kmph / 4, Time.deltaTime);
        else
        followSpeed = 20;



        gameObject.transform.position = Vector3.Lerp(transform.position, child.transform.position, Time.deltaTime * followSpeed);
        gameObject.transform.LookAt(player.gameObject.transform.position);

    }

    void IncreaseFOV(){
        if(pControl.nosFlag){
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, newFOV, Time.deltaTime * smoothTime);
        }
        else{
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFOV, Time.deltaTime * smoothTime);
        }
    }
}
