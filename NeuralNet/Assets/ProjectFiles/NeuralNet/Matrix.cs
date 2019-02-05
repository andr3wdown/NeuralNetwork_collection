using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

[System.Serializable]
public class Matrix
{
    public List<List<GenePair>> matrixGenes = new List<List<GenePair>>();
    public System.Random random = new System.Random(Random.Range(int.MinValue, int.MaxValue));
    public float[][] matrix;
    public Matrix(int outputSize, int inputSize)
    {
        matrix = new float[outputSize][];
        for(int i = 0; i < outputSize; i++)
        {
            matrix[i] = new float[inputSize + 1];
            for(int j = 0; j < inputSize + 1; j++)
            {
                matrix[i][j] = RndNumber(-1.0, 1.0);
            }
        }
        for (int i = 0; i < matrix.Length; i++)
        {
            List<GenePair> newList = new List<GenePair>();
            for (int j = 0; j < matrix[i].Length; j++)
            {
                newList.Add(new GenePair(random.Next(0, 3), random.Next(0, 3)));
            }
            matrixGenes.Add(newList);
        }
    }
    float RndNumber(double min, double max)
    {
        double number = min + random.NextDouble() * (max - min);
        return (float)number;
    }
    float DotProduct(Vector vector)
    {
        float dot = 0;
        
        for(int i = 0; i < matrix.Length; i++)
        {
            if(matrix[i].Length == vector.values.Length)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    dot += matrix[i][j] * vector.values[j];
                }
            }
            else
            {
                Debug.LogError("Invalid Input Vector or Weight Matrix");
                return 0;
            }           
        }
        return dot;
    } 
    public Vector DotVector(Vector vector, string activation="tanh")
    {
        Vector v = new Vector(matrix.Length);
        for (int i = 0; i < matrix.Length; i++)
        {
            if (matrix[i].Length - 1 == vector.values.Length)
            {
                for (int j = 0; j < matrix[i].Length - 1; j++)
                {
                    v.values[i] += matrix[i][j] * vector.values[j];
                }
                v.values[i] += 1 * matrix[i][matrix[i].Length - 1];
                v.values[i] = Activation(activation, v.values[i]);
            }
            else
            {
                Debug.Log(matrix.Length);
                Debug.Log(vector.values.Length);
                Debug.Log(matrix[i].Length - 1);
                Debug.LogError("Invalid Input Vector or Weight Matrix");
                return new Vector(0);
            }
            
        }
        return v;
    }
    public float Activation(string type, float value)
    {
        if(type == "relu")
        {
            return MathOperations.ReLU(value);
        }
        else if(type == "leaky_relu")
        {
            return MathOperations.LeakyReLU(value);
        }
        else if (type == "sigmoid")
        {
            return MathOperations.Sigmoid(value);
        }
        else if (type == "tanh")
        {
            return MathOperations.TanH(value);
        }
        else if(type == "softmax")
        {
            return MathOperations.Softmax(value);
        }
        else if(type == "none")
        {
            return value;
        }
        else
        {
            Debug.LogError("invalid activation function");
            return 0;
        }

    }
}
