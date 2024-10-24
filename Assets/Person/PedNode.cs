using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedNode : MonoBehaviour
{
    public float waitTime = -1;//if wait time is set to -1, ped will despawn when not in view of the camera
    private PedNodeController controller;
    private void Awake()
    {
        if (controller == null) controller = GetComponentInParent<PedNodeController>();
        //This isnt very OO but who cares -- getters and setters can do one atm tbh
        if (!controller.initialised) controller.Init();
        controller.nodes.Add(this);
    }
    public bool isOnCamera()
    {
        Vector3 vpPos = Camera.main.WorldToViewportPoint(transform.position);
        if (vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f)
        {
            //if the camera is looking our way, check if it is obstructed
            Vector3 dir = Camera.main.transform.position - transform.position;
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            if (Physics.Raycast(transform.position, dir, out var hit, Mathf.Infinity, layerMask))
            {
                if (!hit.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        if (controller == null) controller = GetComponentInParent<PedNodeController>();
        if (!(controller.debug)) return;
        Gizmos.color = controller.nodeCol;
        Gizmos.DrawSphere(transform.position,controller.sphereRadius);
    }
    private void OnDrawGizmosSelected()
    {
        if (controller == null) controller = GetComponentInParent<PedNodeController>();
        if (!(controller.debug)) return;
        Gizmos.color = controller.nodeCol;
        Gizmos.DrawSphere(transform.position, controller.selectedRadius);
    }
}
