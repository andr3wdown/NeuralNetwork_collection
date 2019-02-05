using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;
    SimulationController sc;
    NeuralSimulationController sc2;
    public bool neuralNet = false;
    private void Start()
    {
        if (!neuralNet)
        {
            sc = FindObjectOfType<SimulationController>();
        }
        else
        {
            sc2 = FindObjectOfType<NeuralSimulationController>();
        }
        
    }
    private void FixedUpdate()
    {
        if(target == null)
        {
            target = GetTarget();
        }

        if(target != null)
        {
            target = GetTarget();
            transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(0, 2, -10), 1f * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, 0), 0.5f * Time.deltaTime);
        }
    }
    Transform GetTarget()
    {
        
        if (!neuralNet)
        {
            Organism o = sc.FittestOrganism();
            return o.transform;
        }
        else
        {
            DummyNetController o = sc2.GetFittest;
            return o.transform;
        }
            
        
    }

}
