using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNode : MonoBehaviour
{

    NodeDebugSettings settings;
    public List<CarNode> NextNodes;
    private void Start()
    {
       
        Debug.Assert(NextNodes.Count != 0 && NextNodes[0] != null, string.Format("Car node {0} did not have a valid destination set",gameObject.name));
    }
    public CarNode GetNextNode()
    {
        
        int numNodes = NextNodes.Count;
        int nextNode = Random.Range(0, numNodes);
        return NextNodes[nextNode];
    }
    private void OnDrawGizmos()
    {
        if (settings == null)
        {
            settings = GetComponentInParent<NodeDebugSettings>();
        }
        if (!(settings.debug)) return;
        Gizmos.color = settings.sphereCol;
        Gizmos.DrawSphere(transform.position, settings.sphereRad);
        Gizmos.color = settings.lineCol;
        if (NextNodes.Count != 0)
        {
            foreach (CarNode node in NextNodes)
            {
                if (node == null) continue;
                Gizmos.DrawLine(transform.position + Vector3.up, node.transform.position+ Vector3.up);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (!(settings.debug)) return;
        Gizmos.color = settings.selectedCol;
        Gizmos.DrawSphere(transform.position, settings.selectedRad);
        Gizmos.color = settings.selectLineCol;
        if (NextNodes.Count != 0)
        {
            foreach (CarNode node in NextNodes)
            {
                if (node == null) continue;
                Gizmos.DrawLine(transform.position + Vector3.up, node.transform.position + Vector3.up);
                Gizmos.DrawSphere(node.transform.position, settings.selectedRad);
            }
        }
    }
}
