using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float energy;
    bool activated = false;
    public static List<Food> allFood = new List<Food>();
    private void OnEnable()
    {
        allFood.Add(this);
    }
    private void OnDisable()
    {
        allFood.Remove(this);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated)
        {
            if (other.GetComponent<DynamicNeuralController>() != null)
            {        
                activated = true;
                DynamicNeuralController dnc = other.GetComponent<DynamicNeuralController>();
                Activate(dnc);
            }
        }

    }
    public void Activate(DynamicNeuralController dnc)
    {
        dnc.energy += energy;
        Destroy(gameObject);
    }

}
