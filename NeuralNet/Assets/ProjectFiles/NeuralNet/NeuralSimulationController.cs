using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralSimulationController : MonoBehaviour
{
    bool started = false;
    public int PopulationSize;
    public GameObject organismPrefab;
    public List<GameObject> allGameObjects;
    public List<DummyNetController> allActiveBrains;
    public Cooldown gCooldown;
    int inputSize = 28;
    public int[] hiddenSizes = new int[2];
    int outputSize = 6;
    private void Start()
    {
        gCooldown.c = gCooldown.d;
        InitSimulation();
    }
    private void Update()
    {
        if (started)
        {
            gCooldown.c -= Time.deltaTime;
            if (gCooldown.c <= 0)
            {
                NextGeneration();
                gCooldown.c = gCooldown.d;
            }
        }
        
    }
    void InitSimulation()
    {
        for(int i = 0; i < PopulationSize; i++)
        {
            GameObject go = Instantiate(organismPrefab, Vector2.zero, Quaternion.identity);
            allGameObjects.Add(go);
            go.GetComponent<DummyNetController>().InitNet(inputSize, hiddenSizes, outputSize);
            allActiveBrains.Add(go.GetComponent<DummyNetController>());
        }
        started = true;
    }
    void NextGeneration()
    {
        List<DummyNetController> breedable = new List<DummyNetController>();

        for(int i = 0; i < PopulationSize / 2; i++)
        {
            DummyNetController dc = GetFittest;
            breedable.Add(dc);
            allActiveBrains.Remove(dc);
        }
        allActiveBrains.Clear();
        List<GameObject> oldObjects = new List<GameObject>();
        for(int i = 0; i < allGameObjects.Count; i++)
        {
            oldObjects.Add(allGameObjects[i]);
        }
        allGameObjects.Clear();
        while(allGameObjects.Count < PopulationSize)
        {
            for(int i = 0; i < breedable.Count; i++)
            {
                GameObject go = Instantiate(organismPrefab, Vector2.zero, Quaternion.identity);
                allGameObjects.Add(go);              
                if(breedable.Count > 1 && i != 0)
                {
                    NeuralNet partnerNet = breedable[i - 1].net;
                    go.GetComponent<DummyNetController>().InitNet(breedable[i].net.MutateAndReproduce(2, partnerNet), breedable[i].MutatedColors());
                }
                else
                {
                    go.GetComponent<DummyNetController>().InitNet(breedable[i].net.MutateAndReproduce(2), breedable[i].MutatedColors());
                }
                
                allActiveBrains.Add(go.GetComponent<DummyNetController>());
                if(allGameObjects.Count >= PopulationSize)
                {
                    break;
                }
            }
        }
        foreach(GameObject go in oldObjects)
        {
            Destroy(go);
        }
        oldObjects.Clear();
    }
    public DummyNetController GetFittest
    {
        get
        {
            DummyNetController dc = allActiveBrains[0];
            for(int i = 1; i < allActiveBrains.Count; i++)
            {
                if(allActiveBrains[i].Fitness > dc.Fitness)
                {
                    dc = allActiveBrains[i];
                }
            }
            return dc;
        }
    }

  
}
