using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleInputs : MonoBehaviour
{
    internal enum driverType { AI, keyboard }

    [SerializeField] driverType driveController;


    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool handBrake;
    [HideInInspector] public bool boosting;

    trackWaypoints wayPoints;
    List<Transform> nodes = new List<Transform>();
    public Transform currentWayPoint;
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public int steerForce;
    int currentNode;


    private void Awake()
    {
        if (gameObject.CompareTag("Bot"))
        {
            driveController = driverType.AI;
        }
        else
        {
            driveController = driverType.keyboard; 
        }


        wayPoints = GameObject.FindGameObjectWithTag("path").GetComponent<trackWaypoints>();
        nodes = wayPoints.nodes;

    }

    private void FixedUpdate()
    {



        CalcWayPointDistance();


        if(driveController == driverType.AI)
        {
            AIDriver();
        }
        else if(driveController == driverType.keyboard)
        {
            KeyBoardDriver();
        }

    }


    void AIDriver()
    {
        vertical = .3f;
        AISteer();
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

    void CalcWayPointDistance()
    {

        if (nodes == null || nodes.Count == 0)
        {
            Debug.LogError("No waypoints available to calculate distance.");
            return;
        }

        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 difference = nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if (currentDistance < distance)
            {
                if ((i + distanceOffset) >= nodes.Count)
                {
                    currentWayPoint = nodes[1];
                    distance = currentDistance;
                }
                else
                {
                    currentWayPoint = nodes[i + distanceOffset];
                    distance = currentDistance;
                }
                currentNode = i;
            }

        }
    }

    void AISteer()
    {
        Vector3 relative =transform.InverseTransformPoint(currentWayPoint.transform.position);
        relative /= relative.magnitude;

        horizontal = (relative.x / relative.magnitude) * steerForce;
    }

//    private void OnDrawGizmos()
//    {
        
//            Gizmos.DrawWireSphere(currentWayPoint.position, 3);
        
//    }
}