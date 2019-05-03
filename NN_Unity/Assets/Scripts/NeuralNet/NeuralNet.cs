using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNet
{
    Matrix[] weightMatrices;
    private int netType;
    public List<int> sizes = new List<int>();
	public List<string> activations = new List<string>();
    public NeuralNet(int[] layerSizes, string[] _activations = null, string training = "GENETIC_ALGORITHM")
    {
        netType = 2;
        for(int i = 0; i < layerSizes.Length; i++)
        {
            sizes.Add(layerSizes[i]);
        }
        weightMatrices = new Matrix[layerSizes.Length-1];
		for (int i = 0; i < weightMatrices.Length; i++)
		{
			weightMatrices[i] = new Matrix(sizes[i + 1], sizes[i], training, _activations[i], index: i);
        }

		if(_activations != null)
		{
			for(int i = 0; i < _activations.Length; i++)
			{
				activations.Add(_activations[i]);
			}
		}
    }
    public NeuralNet(int p1, int p2, int p3)
    {
        sizes.Add(p1);
        sizes.Add(p2);
        sizes.Add(p3);
        netType = 0;
        weightMatrices = new Matrix[2];
        weightMatrices[0] = new Matrix(p2, p1);
        weightMatrices[1] = new Matrix(p3, p2);
    }
    public NeuralNet(int p1, int p2, int p3, int p4)
    {
        sizes.Add(p1);
        sizes.Add(p2);
        sizes.Add(p3);
        sizes.Add(p4);
        netType = 1;
        weightMatrices = new Matrix[3];
        weightMatrices[0] = new Matrix(p2, p1);
        weightMatrices[1] = new Matrix(p3, p2);
        weightMatrices[2] = new Matrix(p4, p3);
    }

    public NeuralNet MutateAndReproduce(int mutationRate=2, NeuralNet partner=null, bool alt=true, bool incremental = false)
    {

        NeuralNet newNet;
		if (!alt)
		{
			if (netType == 0)
			{
				newNet = new NeuralNet(sizes[0], sizes[1], sizes[2]);

				return NetPasser(newNet, mutationRate, partner);
			}
			else if (netType == 1)
			{
				newNet = new NeuralNet(sizes[0], sizes[1], sizes[2], sizes[3]);
				return NetPasser(newNet, mutationRate, partner);
			}
			else if (netType == 2)
			{
				newNet = new NeuralNet(sizes.ToArray());
				return NetPasser(newNet, mutationRate, partner);
			}
			else
			{
				Debug.LogError("invalid net type!");
				return null;
			}
		}
		else
		{
			if (netType == 0)
			{
				newNet = new NeuralNet(sizes[0], sizes[1], sizes[2]);

				return AltNetPasser(newNet, mutationRate, partner, incremental);
			}
			else if (netType == 1)
			{
				newNet = new NeuralNet(sizes[0], sizes[1], sizes[2], sizes[3]);
				return AltNetPasser(newNet, mutationRate, partner, incremental);
			}
			else if(netType == 2)
			{
				newNet = new NeuralNet(sizes.ToArray(), activations.ToArray());
				return AltNetPasser(newNet, mutationRate, partner, incremental);
			}
			else
			{
				Debug.LogError("invalid net type!");
				return null;
			}
		}
    
    }
    NeuralNet NetPasser(NeuralNet NetToModify, int _mutationRate, NeuralNet partner)
    {
        for (int m = 0; m < weightMatrices.Length; m++)
        {
            for (int i = 0; i < weightMatrices[m].matrix.Length; i++)
            {
                for (int j = 0; j < weightMatrices[m].matrix[i].Length; j++)
                {
                        NetToModify.weightMatrices[m].matrix[i][j] = weightMatrices[m].matrix[i][j];                                        
                }
            }
            if(partner != null)
                NetToModify.weightMatrices[m].matrixGenes = Mutate(NetToModify.weightMatrices[m].matrixGenes, _mutationRate, partner.weightMatrices[m].matrixGenes);
            else
                NetToModify.weightMatrices[m].matrixGenes = Mutate(NetToModify.weightMatrices[m].matrixGenes, _mutationRate);
        }

        NetToModify.weightMatrices = ReadGenes(NetToModify.weightMatrices, NetToModify);
		NetToModify.activations = activations;
		return NetToModify;
    }
	NeuralNet AltNetPasser(NeuralNet NetToModify, int _mutationRate, NeuralNet partner, bool incremental = false)
	{
		for (int m = 0; m < weightMatrices.Length; m++)
		{
			for (int i = 0; i < weightMatrices[m].matrix.Length; i++)
			{
				for (int j = 0; j < weightMatrices[m].matrix[i].Length; j++)
				{
					NetToModify.weightMatrices[m].matrix[i][j] = (partner == null ? weightMatrices[m].matrix[i][j] : (Random.Range(0, 2) == 0 ? weightMatrices[m].matrix[i][j] : partner.weightMatrices[m].matrix[i][j]));
					if(Random.Range(0f, 1000f) < _mutationRate)
					{
						NetToModify.weightMatrices[m].matrix[i][j] = incremental ? Random.Range(-0.5f, 0.5f) : Random.Range(-2f, 2f);
					}
				}
			}
		}
		NetToModify.activations = activations;
		return NetToModify;
	}
	Matrix[] ReadGenes(Matrix[] matrices, NeuralNet _newNet)
    {
        for (int m = 0; m < weightMatrices.Length; m++)
        {
            for(int i = 0; i < _newNet.weightMatrices[m].matrix.Length; i++)
            {
                for(int j = 0; j < _newNet.weightMatrices[m].matrix[i].Length; j++)
                {
                    _newNet.weightMatrices[m].matrix[i][j] = ReadGeneValue(_newNet.weightMatrices[m].matrixGenes[i][j], _newNet.weightMatrices[m].matrix[i][j], 0.005f);
                }
            }
        }
        return matrices;
    }
    float ReadGeneValue(GenePair gene, float value, float learningRate=0.01f)
    {
        if (gene.Equals(possiblePairs[0]))
        {
            return value - learningRate;
        }
        else if (gene.Equals(possiblePairs[1]))
        {
            return value - learningRate / 2;
        }
        else if (gene.Equals(possiblePairs[2]))
        {
            return value - learningRate / 2;
        }
        else if (gene.Equals(possiblePairs[3]))
        {
            return value;
        }
        else if (gene.Equals(possiblePairs[4]))
        {
            return value;
        }
        else if (gene.Equals(possiblePairs[5]))
        {
            return value;
        }
        else if (gene.Equals(possiblePairs[6]))
        {
            return value + learningRate / 2;
        }
        else if (gene.Equals(possiblePairs[7]))
        {
            return value + learningRate / 2;
        }
        else if (gene.Equals(possiblePairs[8]))
        {
            return value + learningRate;
        }
        else
        {
            Debug.LogError("Invalid GenePair!");
            return 0;
        }
    }
    [HideInInspector]
    public GenePair[] possiblePairs = { new GenePair(0, 0), new GenePair(1, 0), new GenePair(0, 1), new GenePair(1, 1), new GenePair(2, 0), new GenePair(0, 2), new GenePair(2, 1), new GenePair(1, 2), new GenePair(2, 2) };
    List<List<GenePair>> Mutate(List<List<GenePair>> oldGenes, int mutationRate, List<List<GenePair>> partnerGenes=null)
    {
        for(int i = 0; i < oldGenes.Count; i++)
        {
            for(int j = 0; j < oldGenes[i].Count; j++)
            {
                if(partnerGenes != null)
                {
                    oldGenes[i][j] = new GenePair(oldGenes[i][j].x, partnerGenes[i][j].y);
                }
                int chance = weightMatrices[0].random.Next(0, 101);
                if (chance <= mutationRate)
                {
                    oldGenes[i][j] = new GenePair(weightMatrices[0].random.Next(0, 3), weightMatrices[0].random.Next(0, 3));
                }
            }
        }
        return oldGenes;
    }

    public int GetRandomInt
    {
        get
        {
            return weightMatrices[0].random.Next(0, 3);
        }
        
    }
    public Vector Run(Vector input)
    {
        if(netType == 0)
        {
            input = weightMatrices[0].DotVector(input);
            input = weightMatrices[1].DotVector(input);
            return input;
        }
        else if(netType == 1)
        {
            input = weightMatrices[0].DotVector(input);
            input = weightMatrices[1].DotVector(input);
            input = weightMatrices[2].DotVector(input);
            return input;
        }
        else
        {
			//Debug.Log("Input: "+ r);
			for (int i = 0; i < weightMatrices.Length; i++)
			{
				//Debug.Log($"layer {i}: {input.ToString()}");
				input = weightMatrices[i].DotVector(input);

			}
			return input;
        }
        
    }
	public void Train(Vector trainingData, float lr = 0.0075f)
	{
		for (int i = weightMatrices.Length - 1; i >= 0; i--)
		{
			if (i == weightMatrices.Length - 1)
			{
				weightMatrices[i].BackPropOutput(trainingData);
			}
			else
			{
				weightMatrices[i].BackPropHidden(weightMatrices[i + 1].gamma, weightMatrices[i + 1].matrix);
			}
		}
		for (int i = 0; i < weightMatrices.Length; i++)
		{
			weightMatrices[i].UpdateMatrix(lr);
		}
	}
	public void Train(float reinforcement, float lr = 0.01f)
	{
		
		for (int i = weightMatrices.Length - 1; i >= 0; i--)
		{
			for(int j = 0; j < weightMatrices[i].layerOutputs.Count; j++)
			{
				if (i == weightMatrices.Length - 1)
				{
					weightMatrices[i].BackPropOutput(Vector.Scalar(weightMatrices[i].layerOutputs[j], reinforcement > 0 ? reinforcement + lr * (1f - j / weightMatrices[i].layerOutputs.Count) : reinforcement * (1f - j / weightMatrices[i].layerOutputs.Count)));
				}
				else
				{
					weightMatrices[i].BackPropHidden(weightMatrices[i + 1].gamma, weightMatrices[i + 1].matrix);
				}
			}
			
		}
		for (int i = 0; i < weightMatrices.Length; i++)
		{
			weightMatrices[i].UpdateMatrix(lr);
		}
	}
}

