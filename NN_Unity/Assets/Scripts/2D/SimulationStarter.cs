using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.NEAT;

public class SimulationStarter : MonoBehaviour
{
	public int[] ls = { 154, 16, 16, 10 };
	public string[] ac = { "tanh", "tanh", "tanh" };
	public int popSize;
	public static List<Creature> creatures = new List<Creature>();
	public Vector2 size;
	public GameObject creature;
	public int startConnections;
	public bool neat;
	void InitSimulation()
	{
		for(int i = 0; i < popSize; i++)
		{
			SpawnCreature(neat);
		}
	}
	void SpawnCreature(bool neat = false)
	{
		GameObject go = Instantiate(creature, GetSpawn(), Quaternion.Euler(0, 0, Random.Range(0, 360)));
		Creature c = go.GetComponent<Creature>();
		creatures.Add(c);
		Color[] cs = { new Color(Random.Range(0f,1f), Random.Range(0f, 1f),Random.Range(0f, 1f), 1f), new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f) };
		if (neat)
		{
			NEATGenome g = new NEATGenome(ls[0], ls[ls.Length-1]);
			for (int j = 0; j < startConnections; j++)
			{
				g.CreateNewConnection();
			}
			for(int j = 0; j < startConnections/2; j++)
			{
				g.CreateNewNode();
			}
			c.InitCreature(new NEATNetwork(g), cs);
		}
		else
		{
			c.InitCreature(new NeuralNet(ls, ac), cs);
		}
		
	}
	public static void SpawnCreature(Vector2 pos, NeuralNet net, Color[] cs)
	{
		GameObject go = Instantiate(instance.creature, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
		Creature c = go.GetComponent<Creature>();
		creatures.Add(c);
		c.InitCreature(net, cs);
	}
	public static void SpawnCreature(Vector2 pos, NEATNetwork net, Color[] cs)
	{
		GameObject go = Instantiate(instance.creature, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
		Creature c = go.GetComponent<Creature>();
		creatures.Add(c);
		c.InitCreature(net, cs);
	}
	Vector2 GetSpawn()
	{
		return new Vector2(Random.Range(-size.x/2, size.x/2), Random.Range(-size.y/2, size.y/2));
	}
	static SimulationStarter instance;
	private void Start()
	{
		instance = this;
		InitSimulation();
	}
	private void Update()
	{
		if(creatures.Count < popSize)
		{
			SpawnCreature();
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, size);
	}
}
