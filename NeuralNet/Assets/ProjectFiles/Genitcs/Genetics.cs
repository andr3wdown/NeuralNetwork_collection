using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Genetics
{
    int genePairs;
    int mutationRate;
    public List<GenePair> genes = new List<GenePair>();
    public Genetics partner;
    
    public Genetics(int _genePairs ,int _mutationRate)
    {
        genePairs = _genePairs;
        mutationRate = _mutationRate;
        for(int i = 0; i < genePairs; i++)
        {
            genes.Add(new GenePair(UnityEngine.Random.Range(0,3), UnityEngine.Random.Range(0,3)));
        }
    }
    public Genetics(List<GenePair> newGenes, int _mutationRate)
    {
        mutationRate = _mutationRate;
        genes = newGenes;
    }
    public List<GenePair> MixGenes(Genetics other)
    {
        List<GenePair> newList = new List<GenePair>();
        for(int i = 0; i < genes.Count; i++)
        {
            newList.Add(genes[i].MixGenes(other.genes[i]));
        }
        newList = Mutations(newList);
        return newList;
    }
    public List<GenePair> Mutations(List<GenePair> origList)
    {
        for(int i = 0; i < mutationRate; i++)
        {
            int index = UnityEngine.Random.Range(0, origList.Count);
            origList[index].Mutate();
        }
        return origList;
    }
}
[Serializable]
public class GenePair : IEquatable<GenePair>
{
    public int x, y;
    public GenePair(int g1, int g2)
    {
        x = g1;
        y = g2;
    }
    public GenePair MixGenes(GenePair other)
    {
        return new GenePair(x, other.y);
    }
    public void Mutate()
    {
        x = UnityEngine.Random.Range(0, 3);
        y = UnityEngine.Random.Range(0, 3);
    }
    public bool Equals(GenePair otherPair)
    {
        if(otherPair.x == x && otherPair.y == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
