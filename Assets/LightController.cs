using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    //Dumb controller - will just cycle each light green in turn
    public float activeTime = 10.0f;
    public List<TrafficLight> lights;
    private float stateTimer;
    private int currentLight = 0;
    private void Awake()
    {
        lights = new List<TrafficLight>();
        stateTimer = activeTime;
    }
    void Update()
    {
        if (stateTimer > 0)
        {
            stateTimer -= Time.deltaTime;
        }
        else
        {
            NextLight();
            stateTimer = activeTime;
        }
    }
    void NextLight()
    {
        lights[currentLight].Green = false;
        currentLight++;
        currentLight %= lights.Count;
        lights[currentLight].Green = true;
    }
}
