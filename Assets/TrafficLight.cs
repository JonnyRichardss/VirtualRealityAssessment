using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    private bool _green = false;
    public bool Green
    {
        get 
        { 
            return _green; 
        }
        set
        {
            _green = value;
            UpdateLight();
        }
    }

    public GameObject GreenLight;
    public GameObject RedLight;
    private void Start()
    {
        Debug.Assert(GreenLight != RedLight && GreenLight != null && RedLight != null, string.Format("Traffic light {0} light references incorrect!", gameObject.name));
        LightController parent = GetComponentInParent<LightController>();
        Debug.Assert(parent != null, string.Format("Traffic light {0} has no controller parent!", gameObject.name));
        parent.lights.Add(this);
        UpdateLight();

    }
    void UpdateLight()
    {
        GreenLight.SetActive(Green);
        RedLight.SetActive(!Green);
    }
}
