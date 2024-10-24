using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedNodeController : MonoBehaviour
{

    //debug variables
    public bool debug = true;
    public float sphereRadius =1.0f;
    public float selectedRadius = 1.5f;
    public Color nodeCol = Color.blue;


    public List<PedNode> nodes;

    public bool initialised { get; private set; } = false;
    //using awake to make it happen before start
    private void Awake()
    {
        if (!initialised) Init(); //I hate this but because I need to guarantee that nodes init before peds i have to put them in awake so i then can't guarantee controller is initialised before nodes
    }
    public void Init()
    {
        nodes = new List<PedNode>();
        initialised = true;
    }

    public PedNode RandomNode()
    {
        int numNodes = nodes.Count;
        int nextNode = Random.Range(0, numNodes);
        return nodes[nextNode];
    }
    

}
