using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public int organismAmount;
    public GameObject organismPrefab;
    List<GameObject> allSimulatedObjects = new List<GameObject>();
    public List<Organism> currentPopulation = new List<Organism>();
    public Cooldown simulationTimer;
    public bool simulationStarted = false;
    private void Start()
    {
        simulationTimer.c = simulationTimer.d;
        InitSimulation();
    }
    
    void InitSimulation()
    {
        for (int i = 0; i < organismAmount; i++)
        {
            GameObject go = Instantiate(organismPrefab, Vector3.zero, Quaternion.identity);
            Organism o = go.GetComponent<Organism>();
            o.genes = new Genetics(9, 1);

            o.flipTimer.d = o.cooldown.d = Random.Range(3f, 7f);
            o.jumpForce = Random.Range(-20f, 20f);
            o.rotationSpeed = Random.Range(-120f, 120f);
            o.mass = Random.Range(1f, 5f);
            o.drag = Random.Range(0, 1f);
            o.cooldown.d = Random.Range(1f, 3f);

            o.ConstructOrganism();
            currentPopulation.Add(o);
            o.started = true;
            allSimulatedObjects.Add(go);
        }
        simulationStarted = true;
    }
    private void Update()
    {
        if (simulationStarted)
        {
            simulationTimer.c -= Time.deltaTime;
            if(simulationTimer.c <= 0)
            {
                simulationStarted = false;
                simulationTimer.c = simulationTimer.d;
                MateGeneration();
            }
        }
    }
    void MateGeneration()
    {
        List<Organism> fittest = new List<Organism>();
        while(fittest.Count < organismAmount / 2)
        {
            Organism f = FittestOrganism();
            fittest.Add(f);
            currentPopulation.Remove(f);
        }
        currentPopulation.Clear();
        List<GameObject> oldSimulatedObjects = new List<GameObject>();
        foreach(GameObject go in allSimulatedObjects)
        {
            oldSimulatedObjects.Add(go);
        }
        allSimulatedObjects.Clear();
        while(allSimulatedObjects.Count < organismAmount)
        {
            foreach (Organism o in fittest)
            {
                GameObject go = Instantiate(organismPrefab, Vector3.zero, Quaternion.identity);
                Organism org = go.GetComponent<Organism>();
                org.genes = new Genetics(o.genes.MixGenes(fittest[Random.Range(0, fittest.Count)].genes), 1);

                org.jumpForce = o.jumpForce;
                org.rotationSpeed = o.rotationSpeed;
                org.mass = o.mass;
                org.drag = o.drag;
                org.cooldown.d = o.cooldown.d;
                org.flipTimer.d = o.flipTimer.d;

                org.ConstructOrganism();
                currentPopulation.Add(org);
                org.started = true;
                allSimulatedObjects.Add(go);

                if (allSimulatedObjects.Count >= organismAmount)
                    break;
            }
        }
        foreach (GameObject go in oldSimulatedObjects)
        {
            Destroy(go);
        }
        simulationStarted = true;
       
    }
    public Organism FittestOrganism()
    {
        Organism fittest = currentPopulation[0];
        for(int i = 1; i < currentPopulation.Count; i++)
        {
            if(currentPopulation[i].Fitness > fittest.Fitness)
            {
                fittest = currentPopulation[i];
            }
        }
        return fittest;
    }
}
