using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedSpawner : MonoBehaviour
{
    public float numPeds=5;
    public PedNodeController NodeContainer;
    public float stopDistance;
    public List<GameObject> PedPrefabs;
    private void Start()
    {
        foreach (GameObject ped in PedPrefabs)
        {
            ped.SetActive(false);
        }
        for (int i = 0; i < numPeds; i++)
        {
            int pedNum = Random.Range(0, PedPrefabs.Count);
            GameObject newPed = GameObject.Instantiate(PedPrefabs[pedNum],transform.position,transform.rotation);
            newPed.GetComponent<PedestrianRoute>().NodeContainer = NodeContainer;
            newPed.SetActive(true);
        }
    }

}
