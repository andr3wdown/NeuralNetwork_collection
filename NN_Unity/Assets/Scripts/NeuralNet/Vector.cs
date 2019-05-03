using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Vector : IEquatable<Vector>
{
    public float[] values;
    public Vector(int lenght, bool randomize = false)
    {
        values = new float[lenght];
        for(int i = 0; i < lenght; i++)
        {
			values[i] = randomize ? UnityEngine.Random.Range(-1f, 1f) : 0.0f;
        }
    }
    public Vector(float[] _values)
    {
        values = _values;
    }
    public void SetValue(int index, float value)
    {
        if(index <= values.Length)
        {
            values[index] = value;
        }
        else
        {
            Debug.LogError("invalid index");
        }
    }
    public void SetValues(int startingIndex, float[] newValues)
    {
        if(startingIndex + newValues.Length < values.Length)
        {
            int z = 0;
            for(int i = startingIndex; i < values.Length; i++)
            {
                values[i] = newValues[z];
                z++; 
            }
        }
        else
        {
            Debug.LogError("Invalid starting index");
        }
    }
	public float GetValue(int i)
	{
		return values[i];
	}
    public bool Equals(Vector other)
    {
        if(values.Length != other.values.Length)
        {
            return false;
        }
        for(int i = 0; i < values.Length; i++)
        {
            if(values[i] != other.values[i])
            {
                return false;
            }
        }
        return true;
    }
	public void JoinVector(Vector other)
	{
		float[] newValues = new float[values.Length + other.values.Length];
		for(int i = 0; i < values.Length; i++)
		{
			newValues[i] = values[i];
		}
		for(int i = 0; i < other.values.Length; i++)
		{
			newValues[i + values.Length] = other.values[i];
		}
		values = newValues;
	}
	public override string ToString()
	{
		string s = "";
		for(int i = 0; i < values.Length; i++)
		{
			s += Math.Round(values[i], 2).ToString() + (i == values.Length -1 ? "" : ", ");
		}
		return s;
	}
	public void Rectify()
	{
		for(int i = 0; i < values.Length; i++)
		{
			if(values[i] != values[i])
			{
				values[i] = 0;
			}
		}
	}
	public static Vector Scalar(Vector v, float scale, string operation="")
	{
		switch (operation)
		{
			default:
				return v;
			case "multiply":
				Vector returnable = new Vector(v.Lenght);
				for(int i = 0; i < v.values.Length; i++)
				{
					returnable.values[i] = v.values[i] * scale;
				}
				return returnable;
		}
	}
	public int Lenght
	{
		get
		{
			return values.Length;
		}
	}
}
