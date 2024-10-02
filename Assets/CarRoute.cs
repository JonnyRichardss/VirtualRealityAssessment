using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRoute : MonoBehaviour
{

    private int targetWP = 0;
    public GameObject WaypointContainer;
    private List<Transform> wps;
    private bool waiting = false;
    public float speedMult = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (!WaypointContainer) Destroy(gameObject);//if we dont have waypoint container die
        wps = new List<Transform>();
        foreach (Transform t in WaypointContainer.GetComponentsInChildren<Transform>())
        {
            if (ReferenceEquals(WaypointContainer.transform, t)) continue; //ignore the container itself
            wps.Add(t);
        }
        if (wps.Count == 0) Destroy(gameObject); //if container has no children die
        SetRoute();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (waiting) return;
        Vector3 displacement = wps[targetWP].position - transform.position;
        displacement.y = 0;
        float dist = displacement.magnitude;
        if (dist < 0.1f)
        {
            SetRoute();

        }
        //calculate velocity for this frame
        Vector3 velocity = displacement;
        velocity.Normalize();
        velocity *= 2.5f * speedMult;
        //apply velocity
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
        targetWP++;
        targetWP %= wps.Count;
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
