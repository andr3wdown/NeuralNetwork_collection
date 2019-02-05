using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Vector : IEquatable<Vector>
{
    public float[] values;
    public Vector(int lenght)
    {
        values = new float[lenght];
        for(int i = 0; i < lenght; i++)
        {
            values[i] = 0.0f;
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
}
