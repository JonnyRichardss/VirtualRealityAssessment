using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianRoute : MonoBehaviour
{

    public PedNodeController NodeContainer;

    public float stopDistance;
    public bool camWaiting;
    [SerializeField]
    private PedNode source;
    [SerializeField]
    private PedNode destination;

    private NavMeshAgent nav;
    private Animator anim;

    private Vector2 Velocity;
    private Vector2 SmoothDeltaPosition;

    [SerializeField]
    float waitTimer = -1;
    float startDelay = 0;
    private void Awake()
    {
        Debug.Assert(NodeContainer != null, string.Format("Pedestrian {0} has no nodes set!", this.name));
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.applyRootMotion = true;
        nav.updatePosition = false;
        nav.updateRotation = true;
        startDelay = Random.Range(0.0f, 3.0f);
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = anim.rootPosition;
        rootPosition.y = nav.nextPosition.y;
        transform.position = rootPosition;
        nav.nextPosition = rootPosition;
    }

    private void Update()
    {
       if (startDelay >= 0 || Time.time == 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }
       if (startDelay != Mathf.NegativeInfinity)
        {
            SetNewRoute();
            startDelay = Mathf.NegativeInfinity;
        }
        SynchronizeAnimatorAndAgent();
        // if (Time.time == 0) return; //now THIS is hacky wow --- for some reason on the first frame ONLY nav remaining distance is 0 so it triggers turning off anim, this is easily fixed :)
        if (arrived())
        {
            // anim.SetBool("IsMoving", false);

            //this all feels very hacky but it should work pretty well tbh
            
            if (destination.waitTime == -1)
            {
                if (!isOnCamera()) SetNewRoute(); //respawn based destinations
                else camWaiting = true;
            }

            else if (destination.waitTime > 0)//wait based destinations
            {
                if (waitTimer == -1) waitTimer = destination.waitTime;
                else
                {
                    waitTimer -= Time.deltaTime;
                }
                if (waitTimer <= 0) SetNewDest();
            }
            
        }   
    }
    private bool isOnCamera()
    {
        Vector3 vpPos = Camera.main.WorldToViewportPoint(transform.position);
        if(vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f)
        {
            //if the camera is looking our way, check if it is obstructed
            Vector3 dir =  Camera.main.transform.position - transform.position;
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            if(Physics.Raycast(transform.position,dir,out var hit,Mathf.Infinity,layerMask))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool arrived()
    {
       // return !(anim.GetBool("IsMoving")); //trust the root motion code that i didnt write LOL
        return Vector3.Magnitude(transform.position - destination.transform.position) <= nav.stoppingDistance;
    }
    private void SetNewRoute()
    {
        camWaiting = false;
        do
        {
            //make sure we dont get the same node twice
            source = NodeContainer.RandomNode();
            destination = NodeContainer.RandomNode();
        }
        while (source == destination && source.isOnCamera());

        nav.Warp(source.transform.position);

        if (!nav.isOnNavMesh) Debug.Log(string.Format("Starting: {0} Ending: {1}", source.name, destination.name));
        nav.destination = destination.transform.position;

        anim.SetBool("IsMoving", true);
        waitTimer = -1;
    }
    private void SetNewDest()
    {
        source = destination;
        do
        {
            //make sure we dont get the same node twice
            destination = NodeContainer.RandomNode();
        } while (source == destination);
        nav.destination = destination.transform.position;
       // anim.SetBool("IsMoving", true);
        waitTimer = -1;
    }
    private void SynchronizeAnimatorAndAgent()
    {
        //https://www.youtube.com/watch?v=uAGjKxH4sDQ
        Vector3 worldDeltaPosition = nav.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

        Velocity = SmoothDeltaPosition / Time.deltaTime;
        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            Velocity = Vector2.Lerp(Vector2.zero, Velocity, nav.remainingDistance);
        }

        bool shouldMove = nav.remainingDistance > nav.stoppingDistance;

        anim.SetBool("IsMoving", shouldMove);
        //anim.SetFloat("velx", Velocity.x);
        //anim.SetFloat("vely", Velocity.y);

       // LookAt.lookAtTargetPosition = nav.steeringTarget + transform.forward;
        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > nav.radius / 2)
        {
            transform.position = Vector3.Lerp(anim.rootPosition, nav.nextPosition, smooth);
        }
    }
}
