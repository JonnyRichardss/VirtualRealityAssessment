using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CarRoute : MonoBehaviour
{

    public int targetNode = 0;
    public GameObject WaypointContainer;
    private List<CarNode> nodes;
    private bool waiting = false;
    public float speedMult = 1;
    public float stopDistance = 0.1f;
    public float acceleration = 1.0f;
    public float deceleration = 1.0f;
    public float maxSpeed = 5.0f;
    public float steerStrength = 1.0f;
    Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        if (!WaypointContainer) Destroy(gameObject);//if we dont have waypoint container die
        nodes = new List<CarNode>();
        foreach (CarNode n in WaypointContainer.GetComponentsInChildren<CarNode>())
        {
            nodes.Add(n);
        }
        if (nodes.Count == 0) Destroy(gameObject); //if container has no children die
        //TODO! set targetWP to random (from selected??)
        //TODO! add random delay
        transform.position = nodes[targetNode].transform.position + Vector3.up;
        SetRoute();
    }


    private void FixedUpdate()
    {
        Vector3 displacement = nodes[targetNode].transform.position - transform.position;
        displacement.y = 0;
        float dist = displacement.magnitude;
        float angle = Vector3.Angle(velocity, displacement);
        if (waiting)
        {
            //we are wating, slow down
            velocity -= transform.forward * deceleration;
            if (velocity.magnitude < 0)
            {
                //clamp
                velocity = Vector3.zero;
            }
        }
        else
        {
            //we are going, accelerate and turn
            velocity += transform.forward * acceleration;
            if (velocity.magnitude > maxSpeed)
            {
                //clamp
                velocity = velocity.normalized * maxSpeed;
            }

            //rotate velocity by steering strength to align with target
            velocity = Vector3.RotateTowards(velocity, displacement, Mathf.Deg2Rad * Mathf.Lerp(0,angle,steerStrength), 0.0f);

            if (dist < stopDistance)
            {//has to be in here to prevent recalculating if we stop AT a node
                SetRoute();
            }
        }

        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(newPosition);
        //align to velocity
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity,
        10.0f * Time.deltaTime, 0f);
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        rb.MoveRotation(rotation);
       
    }
    void SetRoute()
    {
        CarNode nextNode = nodes[targetNode].GetNextNode();
        int newNode = nodes.FindIndex(a=> a == nextNode);
        targetNode = newNode;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.CompareTag("Car") || other.CompareTag("Person"))
            {
                waiting = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        waiting = false;
    }
}
