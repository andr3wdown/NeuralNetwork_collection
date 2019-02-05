using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNet
{
    Matrix[] weightMatrices;
    private int netType;
    List<int> sizes = new List<int>();
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
   
    public NeuralNet MutateAndReproduce(int mutationRate=2, NeuralNet partner=null)
    {
        NeuralNet newNet;
        if(netType == 0)
        {
            newNet = new NeuralNet(sizes[0], sizes[1], sizes[2]);

            return NetPasser(newNet, mutationRate, partner);
        }
        else if(netType == 1)
        {
            newNet = new NeuralNet(sizes[0], sizes[1], sizes[2], sizes[3]);
            return NetPasser(newNet, mutationRate, partner);
        }
        else
        {
            Debug.LogError("invalid net type!");
            return null;
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
                NetToModify.weightMatrices[m].matrixGenes = Mutate(NetToModify.weightMatrices[m].matrixGenes, _mutationRate, partner.weightMatrices[m].matrixGenes, mutationMode: "slice");
            else
                NetToModify.weightMatrices[m].matrixGenes = Mutate(NetToModify.weightMatrices[m].matrixGenes, _mutationRate);
        }

        NetToModify.weightMatrices = ReadGenes(NetToModify.weightMatrices, NetToModify);

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
                    _newNet.weightMatrices[m].matrix[i][j] = ReadGeneValue(_newNet.weightMatrices[m].matrixGenes[i][j], _newNet.weightMatrices[m].matrix[i][j], 0.02f);
                }
            }
        }
        return matrices;
    }
    float ReadGeneValue(GenePair gene, float value, float learningRate=0.005f)
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
    List<List<GenePair>> Mutate(List<List<GenePair>> oldGenes, int mutationRate, List<List<GenePair>> partnerGenes=null, string mutationMode="normal")
    {
        for(int i = 0; i < oldGenes.Count; i++)
        {
            for(int j = 0; j < oldGenes[i].Count; j++)
            {
                if(partnerGenes != null && mutationMode == "normal")
                {
                    oldGenes[i][j] = new GenePair(oldGenes[i][j].x, partnerGenes[i][j].y);
                }
                else if(partnerGenes != null && mutationMode == "slice")
                {
                    int index = weightMatrices[0].random.Next(0, 2);
                    switch (index)
                    {
                        case 0:
                            oldGenes[i][j] = oldGenes[i][j];
                            break;

                        case 1:
                            oldGenes[i][j] = partnerGenes[i][j];
                            break;
                    }
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
    public NeuralNet (int p1, int p2, int p3, int p4)
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
            input = weightMatrices[0].DotVector(input, activation: "tanh");
            input = weightMatrices[1].DotVector(input, activation: "tanh");
            return input;
        }
        else
        {
            input = weightMatrices[0].DotVector(input, activation: "tanh");
            input = weightMatrices[1].DotVector(input, activation: "tanh");
            input = weightMatrices[2].DotVector(input, activation: "tanh");
            return input;
        }
        
    }
}

