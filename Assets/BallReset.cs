using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 startPos;
    Rigidbody RB;
    void Start()
    {
        startPos = transform.position;
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Reset()
    {
        //transform.position = startPos;
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero; 
        RB.position = startPos;
    }
}
