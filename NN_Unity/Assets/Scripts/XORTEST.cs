using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XORTEST : MonoBehaviour
{
	NeuralNet net;
	public int[] layerSizes = { 2, 8, 1 };
	public string[] activations = { "tanh", "sigmoid" };
	public int epochs = 1000;

	public float learningRate = 0.1f;
	private void Start()
	{
		net = new NeuralNet(layerSizes, activations, "BACK_PROPAGATION");
		StartCoroutine(Train());
	}
	IEnumerator Train()
	{
		CreateTables();
		for(int i = 0; i < epochs; i++)
		{
			for(int j = 0; j < xorTable.Length; j++)
			{
				net.Run(xorTable[j]);
				net.Train(labels[j], lr: learningRate);
				print($"{ net.Run(xorTable[j]) } predicted { labels[j] } actual");
			}
		
			yield return new WaitForSeconds(0.02f);
		}

	}


	Vector[] xorTable = new Vector[8];
	Vector[] labels = new Vector[8];
	float[][] xor = { new float[2] { 0, 0 }, new float[2] { 1, 0 }, new float[2] { 1, 1 }, new float[2] { 0, 1 } };
	float[] labls = { 0, 1, 0, 1 };
	void CreateTables()
	{
		for (int i = 0; i < 4; i++)
		{
			xorTable[i] = new Vector(xor[i]);
			labels[i] = new Vector(1);
			labels[i].SetValue(0, labls[i]);
		}
		for (int i = xorTable.Length / 2; i < xorTable.Length; i++)
		{
			int index = xorTable.Length - 1 - i;
			xorTable[i] = new Vector(xor[index]);
			labels[i] = new Vector(1);
			labels[i].SetValue(0, labls[index]);
		}
	}
}
