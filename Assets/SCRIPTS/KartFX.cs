using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartFX : MonoBehaviour
{
    ControllerTwo pcontrol;
    [SerializeField] ParticleSystem[] smoke;
    [SerializeField] TrailRenderer[] tireMarks;
    bool smokeFlag = false;
    bool isAcceleratingFromStop = false;
    bool tireMarksFlag;
    HandleInputs handleInputs;


    private void Start()
    {
        pcontrol = gameObject.GetComponent<ControllerTwo>();
        handleInputs = gameObject.GetComponent<HandleInputs>();
    }

    private void FixedUpdate()
    {
        HandleSmokeEffects();
        CheckDrift();
    }

    private void HandleSmokeEffects()
    {
        if (pcontrol.kmph < 1 && pcontrol.inputHandler.vertical > 0)
        {
            isAcceleratingFromStop = true;
        }
        else if (pcontrol.kmph > 10)
        {
            isAcceleratingFromStop = false;
        }

        if (pcontrol.toggleSmoke || isAcceleratingFromStop)
        {
            StartSmoke();
        }
        else
        {
            StopSmoke();
        }

        if (smokeFlag)
        {
            for (int i = 0; i < smoke.Length; i++)
            {
                var emission = smoke[i].emission;
                emission.rateOverTime = Mathf.Clamp((int)pcontrol.kmph * 13.33f, 0, 3000);
            }
        }
    }

    public void StartSmoke()
    {
        if (smokeFlag)
        {
            return;
        }

        smokeFlag = true;

        for (int i = 0; i < smoke.Length; i++)
        {
            var emission = smoke[i].emission;
            emission.rateOverTime = Mathf.Clamp((int)pcontrol.kmph * 13.33f, 0, 3000);
            smoke[i].Play();
        }
    }

    public void StopSmoke()
    {
        smokeFlag = false;

        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Stop();
        }
    }

    void CheckDrift()
    {
        if (handleInputs.handBrake)
        {
            StartEmitter();
        }
        else
        {
            StopEmitter();
        }
    }

    void StartEmitter()
    {
        if (tireMarksFlag)
        {
            return;
        }

        foreach (var skidMark in tireMarks)
        {
            skidMark.emitting = true;
        }

        tireMarksFlag = true;
    }

    void StopEmitter()
    {
        if (!tireMarksFlag)
        {
            return;
        }

        foreach (var skidMark in tireMarks)
        {
            skidMark.emitting = false;
        }

        tireMarksFlag = false;

    }
}
