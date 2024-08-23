using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleInputs : MonoBehaviour
{
    internal enum driverType { AI, keyboard}

    [SerializeField] driverType driveController;


    [HideInInspector]public  float vertical;
    [HideInInspector]public float horizontal;
    [HideInInspector]public bool handBrake;
    [HideInInspector]public bool boosting;

    private void FixedUpdate()
    {

        switch (driveController)
        {
            case driverType.AI: break; 
            case driverType.keyboard: KeyBoardDriver(); break;
            default: break;
        }
    }


    void AIDriver()
    {

    }

    void KeyBoardDriver()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        handBrake = (Input.GetAxis("Jump") != 0) ? true : false;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            boosting = true;
        }
        else
        {
            boosting = false;
        }

    }
}
