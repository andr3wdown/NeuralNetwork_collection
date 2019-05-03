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

	//Backprop and RL stuff
	public int outputDimensions;
	public int inputDimensions;
	public float[][] matrixDelta;
	public Vector layerInputs;
	public Vector layerOutput;
	public List<Vector> layerOutputs = new List<Vector>();
	public float[] gamma;
	public float[] error;
	string training;
	string activation;
	bool useBackProp
	{
		get
		{
			return training == "BACK_PROPAGATION" || training == "MIXED" || training == "REINFORCEMENT_LEARNING";
		}
	}
	int index;
	public Matrix(int outputSize, int inputSize, string trainings = "GENETIC_ALGORITHM", string activation = "tanh", int index = -1)
    {
		this.index = index;
		training = trainings;
		this.activation = activation;
		if (useBackProp)
		{
			outputDimensions = outputSize;
			inputDimensions = inputSize + 1;
			matrixDelta = new float[outputSize][];
			gamma = new float[outputSize];
			error = new float[outputSize];
		}

		matrix = new float[outputSize][];
		for (int i = 0; i < outputSize; i++)
		{
			if (useBackProp)
			{
				matrixDelta[i] = new float[inputSize + 1];
			}

			matrix[i] = new float[inputSize + 1];
			for (int j = 0; j < inputSize + 1; j++)
			{
				matrix[i][j] = Random.Range(-2f, 2f);//RndNumber(-1.0, 1.0);
			}
		}


		if (training == "GENETIC_ALGORITHM" || training == "MIXED")
		{
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

    public Vector DotVector(Vector vector)
    {
        Vector v = new Vector(matrix.Length);
        for (int i = 0; i < matrix.Length; i++)
        {
            if (matrix[i].Length - 1 == vector.values.Length)
            {
                for (int j = 0; j < matrix[i].Length - 1; j++)
                {
					float add = vector.values[j] * matrix[i][j];
					if(add != add)
					{
						add = 0;
					}
					v.values[i] += add;//matrix[i][j] * vector.values[j];
                }
				v.values[i] += 1f * matrix[i][matrix[i].Length - 1];
				v.values[i] = Activation(activation, v.values[i]);
				if (v.values[i] != v.values[i])
				{
					v.values[i] = 0;
				}
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
		if (useBackProp)
		{
			layerInputs = new Vector(vector.values.Length + 1);

			for (int i = 0; i < vector.values.Length; i++)
			{
				layerInputs.SetValue(i, vector.values[i]);
			}
			layerInputs.SetValue(vector.values.Length, 1f);

			if (training == "REINFORCEMENT_LEARNING")
			{
				layerOutputs.Add(v);
				while (layerOutputs.Count > 100)
				{
					layerOutputs.Remove(layerOutputs[0]);
				}
			}
			layerOutput = v;

		}
		//Debug.Log("matrix " + index + " output: "+  v + $" with {activation} activation");
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
	public float ActivationDeriv(string type, float value)
	{
		if (type == "tanh")
		{
			return MathOperations.TanHDerivative(value);//1 - (value * value);
		}
		else if(type == "sigmoid")
		{
			return MathOperations.SigmoidDerivative(value);
		}
		else if (type == "leaky_relu")
		{
			return MathOperations.LeakyReLUDerivative(value);
		}
		else if (type == "relu")
		{
			return MathOperations.ReLUDerivative(value);
		}
		else if (type == "softmax")
		{
			return MathOperations.SoftmaxDerivative(value);
		}
		else
		{
			throw new System.Exception("activation derivation not implemented or is invalid!");
		}
	}

	public void BackPropOutput(Vector expection)
	{
		if (training == "REINFORCEMENT_LEARNING")
		{
			List<Vector> errors = new List<Vector>();
			for (int i = 0; i < layerOutputs.Count; i++)
			{
				Vector roundError = new Vector(outputDimensions);
				for (int j = 0; j < outputDimensions; j++)
				{
					roundError.values[j] = (layerOutputs[i].values[j] - expection.values[j]) / (i * i + 2);
				}
				errors.Add(roundError);
			}
			layerOutputs.Clear();
			error = new float[outputDimensions];
			for (int i = 0; i < errors.Count; i++)
			{
				for (int j = 0; j < outputDimensions; j++)
				{
					error[j] += errors[i].values[j];
				}
			}

			for (int i = 0; i < outputDimensions; i++)
			{
				gamma[i] = error[i] * ActivationDeriv(activation, layerOutput.values[i]);
			}

			for (int i = 0; i < outputDimensions; i++)
			{
				for (int j = 0; j < inputDimensions; j++)
				{
					matrixDelta[i][j] = gamma[i] * layerInputs.values[j];


				}
			}
		}
		else
		{
			for (int i = 0; i < outputDimensions; i++)
			{
				error[i] = layerOutput.values[i] - expection.values[i];
				//Debug.Log(i.ToString() + " : " + error[i]);
				gamma[i] = error[i] * ActivationDeriv(activation, layerOutput.values[i]);
			}

			for (int i = 0; i < outputDimensions; i++)
			{
				for (int j = 0; j < inputDimensions; j++)
				{
					matrixDelta[i][j] = gamma[i] * layerInputs.values[j];


				}
			}
		}

	}
	public void BackPropHidden(float[] gammaForward, float[][] weightsForward)
	{
		for (int i = 0; i < outputDimensions; i++)
		{
			gamma[i] = 0;
			for (int j = 0; j < gammaForward.Length; j++)
			{
				gamma[i] += gammaForward[j] * weightsForward[j][i];
			}
			gamma[i] *= ActivationDeriv(activation, layerOutput.values[i]);
		}
		for (int i = 0; i < outputDimensions; i++)
		{
			for (int j = 0; j < inputDimensions; j++)
			{
				matrixDelta[i][j] = gamma[i] * layerInputs.values[j];

			}
		}
	}
	public void UpdateMatrix(float learningRate = 0.001f)
	{
		for (int i = 0; i < outputDimensions; i++)
		{
			for (int j = 0; j < inputDimensions; j++)
			{
				matrix[i][j] -= matrixDelta[i][j] * learningRate;
			}
		}
	}
}
