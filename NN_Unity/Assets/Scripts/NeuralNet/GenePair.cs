using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
		if (otherPair.x == x && otherPair.y == y)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
