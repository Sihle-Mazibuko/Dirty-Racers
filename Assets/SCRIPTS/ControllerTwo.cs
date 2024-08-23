using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;

public class ControllerTwo : MonoBehaviour
{
    #region variables
    //DRIVE TYPES
    internal enum driveType
    {
        frontWheelDrive, rearWheelDrive, allWheelDrive
    }
    [SerializeField] driveType drive;


    [Header("Car Variables")] 
    //MOVEMENT
    GameObject wheelMeshes, wheelColliders;
     WheelCollider[] wheels = new WheelCollider[4];
     GameObject[] wheelMesh  = new GameObject[4];
    [SerializeField] float carTorque;
    //[SerializeField] float steeringMax = 4;
    [SerializeField] float brakePower;
    public HandleInputs inputHandler;

    //ENGINE
    //public AnimationCurve enginePower;
    //float wheelsRPM;
    [SerializeField] float totalPower;
    //float smoothTime = .1f;
    //public float engineRPM;
    //public float[] gears;
    //int gearNum = 0;


    //TURNING
    [SerializeField] float radius = 6;
    Rigidbody rb;
    public float kmph;

    //DOWN FORCE
    float downForceValue = 50;
    GameObject centerOfMass;

    //FRICTION
    [SerializeField] float[] slip = new float[4];


    //NITROS BOOSTER
    float thrust = 10000;

    //DRIFTING
    WheelFrictionCurve forwardFriction, sidewaysFriction;
    float tempo, handBrakeFriction;
    //[SerializeField] float handBrakeFrictionMultiplier = 2f;
    float driftFactor ;

    //EFFECTS
    [HideInInspector] public bool toggleSmoke = false;
    public float nosValue;
    public bool nosFlag;
    [SerializeField] ParticleSystem[] nosParticle;


    #endregion


    private void Start()
    {
        GetObjects();
        StartCoroutine(TimeLoop());
    }

    private void FixedUpdate()
    {
        AddDownForce();
        WheelAnimation();
        MoveVehicle();
        Steering();
        GetFriction();
        AdjustTraction();
        CheckWheelSpin();
        ActivateNOS();
        //CalcEnginePower(); THIS DOESNT WORK
    }

    //DOESNT WORK
    //void CalcEnginePower()
    //{
    //    WheelRevsPM();

    //    totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * inputHandler.vertical;
    //    float velocity = .0f;
    //    engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);


    //}

    //void WheelRevsPM()
    //{
    //    float sum = 0;
    //    int revs = 0;

    //    for (int i = 0; i < 4; i++)
    //    {
    //        sum += wheels[i].rpm;
    //        revs++;
    //    }
    //    wheelsRPM = (revs != 0 ) ? sum / revs : 0;
    //}

    void GetFriction()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.sidewaysSlip;
        }
    }

    void AddDownForce()
    {
        rb.AddForce(-transform.up * downForceValue * rb.velocity.magnitude);
    }

    void MoveVehicle()
    {

        kmph = rb.velocity.magnitude * 3.6f;

        float maxSpeed = 153f;
        if(kmph > maxSpeed)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = 0;
            }
        }
        else
        {
            if (drive == driveType.allWheelDrive)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = inputHandler.vertical * (totalPower / 4);
                }


            }
            else if (drive == driveType.frontWheelDrive)
            {
                for (int i = 2; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = inputHandler.vertical * (totalPower / 2);
                }

            }
            else
            {
                for (int i = 0; i < wheels.Length - 2; i++)
                {
                    wheels[i].motorTorque = inputHandler.vertical * (totalPower / 2);
                }

            }

        }



      

        //if (inputHandler.boosting && kmph <= 175)
        //{
        //    rb.AddForce(-Vector3.forward * thrust);
        //}

    }

    void Steering()
    {
   
        if(inputHandler.horizontal > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputHandler.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputHandler.horizontal;
        }
        else if(inputHandler.horizontal <0) 
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputHandler.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputHandler.horizontal;

        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }

    }

    void WheelAnimation()
    {

        Vector3 wheelPos = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPos, out wheelRotation);
            wheelMesh [i].transform.position = wheelPos;
            wheelMesh [i].transform.rotation = wheelRotation;
        }
    }

    void GetObjects()
    {
        inputHandler = GetComponent<HandleInputs>();
        rb = GetComponent<Rigidbody>();

        //wheels
        wheelColliders = GameObject.Find("wheelColliders");
        wheelMeshes = GameObject.Find("wheelMeshes");

        wheelMesh[0] = wheelMeshes.transform.Find("0").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("1").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("2").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("3").gameObject;

        wheels[0] = wheelColliders.transform.Find("0").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("1").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("2").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("3").gameObject.GetComponent<WheelCollider>();



    
        //centre of mass
        centerOfMass = GameObject.Find("Mass");
        rb.centerOfMass = centerOfMass.transform.localPosition;



    }

    void AdjustTraction()
    {
        float driftSmoothFactor = .7f * Time.deltaTime;

        forwardFriction = wheels[0].forwardFriction;
        sidewaysFriction = wheels[0].sidewaysFriction;

        if (inputHandler.handBrake)
        {
            for (int i = 2; i < 4; i++) 
            {
                wheels[i].brakeTorque = brakePower;
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = 0.5f; 
                forwardFriction.extremumValue = forwardFriction.asymptoteValue = 0.5f;
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;
            }
        }
        else
        {
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            for (int i = 0; i < 4; i++)
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;
            }
        }
    }

    IEnumerator TimeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + kmph/20;
        }
    }

    void CheckWheelSpin()
    {
        bool isDrifting = false;

        if (inputHandler.handBrake)
        {
            for (int i = 2; i < 4; i++)
            {
                WheelHit wheelHit;
                wheels[i].GetGroundHit(out wheelHit);

                if (Mathf.Abs(wheelHit.sidewaysSlip) >= .3f || Mathf.Abs(wheelHit.forwardSlip) >= -0.3f) 
                { 

                    isDrifting = true;
                    wheels[i].brakeTorque = brakePower;
                }
                else
                {
                    wheels[i].brakeTorque = 0;
                }

            }
        }
        else
        {
            for (int i = 2; i < 4; i++) 
            {
                wheels[i].brakeTorque = 0; 
            }
        }

        toggleSmoke = isDrifting;
    }

    public void ActivateNOS()
    {
        if(!inputHandler.boosting && nosValue <= 5)
        {
            nosValue += Time.deltaTime / 5;

        }
        else
        {
            nosValue -= (nosValue <= 0 ) ? 0 : Time.deltaTime ;
        }

        if(inputHandler.boosting)
        {
            if(nosValue>0)
            {
                StartNOS();
            }
            else
            {
                StopNOS();
            }
        }
        else
        {
            StopNOS();
        }
    }

    public void StartNOS()
    {
        if (nosFlag)
        {
            return;
        }


        for (int i = 0; i < nosParticle.Length; i++)
        {
            nosParticle[i].Play();
        }


        rb.AddForce(transform.forward * thrust);

        nosFlag = true;
    }

    public void StopNOS()
    {
        if (!nosFlag)
        {
            return;
        }


        for (int i = 0; i < nosParticle.Length; i++)
        {
            nosParticle[i].Stop();
        }

        nosFlag = false;

    }
}

