using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

public class Tree : MonoBehaviour
{
	SpriteRenderer sr;
	LineRenderer[] branches;
	public Material branchMaterial;
	TreeGenes genes;
    void Init(TreeGenes genes)
	{
		this.genes = genes;
		cooldown = new Cooldown(4f);
		sr = GetComponent<SpriteRenderer>();
		sr.color = genes.color;
		branches = new LineRenderer[genes.maxBranch];
		for(int i = 0; i < branches.Length; i++)
		{
			GameObject go = new GameObject($"Branch {i}");
			branches[i] = go.AddComponent<LineRenderer>();
			go.transform.position = transform.position;
			go.transform.parent = transform;
			branches[i].material = branchMaterial;
			Vector3[] positions = { transform.position, (Vector2)transform.position + MathOperations.AngleToVector(Random.Range(0, 360), genes.branchLenght) };
			branches[i].positionCount = 2;
			branches[i].SetPositions(positions);
			branches[i].startWidth = 0.1f;
			branches[i].endWidth = 0.1f;
			branches[i].startColor = Color.Lerp(genes.color, genes.fruitColor, 0.5f); //genes.color;
			branches[i].endColor = Color.Lerp(genes.color, genes.fruitColor, 0.5f);
			branches[i].numCapVertices = 3;
		}
		GetSpawnPoints();
		maxGrow = genes.maxBranch;
	}
	private void Start()
	{
		Init(new TreeGenes());
	}

	List<Vector3> vertices = new List<Vector3>();
	int[] growing;
	int maxGrow = 5;
	int currentGrow = 0;
	Cooldown cooldown;
	public GameObject fruit;

	private void Update()
	{
		// if day
		if (true && currentGrow < maxGrow)
		{
			cooldown.CountDown();
			if (cooldown.TriggerReady())
			{
				SpawnFruit(GetPoint());
			}
		}
	}
	void SpawnFruit(PointInfo posIndex)
	{
		GameObject go = Instantiate(fruit, posIndex.pos, MathOperations.LookAt2D(posIndex.pos, transform.position, -90 + Random.Range(-30f, 30f)));
		go.transform.parent = transform;
		Food f = go.GetComponent<Food>();
		f.Init(genes.fruitColor, posIndex.index, 0.1f, 0.4f);
	}
	public void DetachFruit(int index)
	{
		currentGrow--;
		growing[index] = 0;
	}
	void GetSpawnPoints()
	{
		foreach(LineRenderer lr in branches)
		{
			vertices.Add(lr.GetPosition(1));
		}
		growing = new int[vertices.Count];
	}
	PointInfo GetPoint()
	{
		if (currentGrow >= maxGrow)
		{
			return new PointInfo(Vector2.zero, -1);
		}
		int index = Random.Range(0, vertices.Count);
		while (growing[index] == 1)
		{
			index = Random.Range(0, vertices.Count);
		}
		growing[index] = 1;
		currentGrow++;
		return new PointInfo(vertices[index], index);
	}
}
public class TreeGenes
{
	public Color color;
	public Color fruitColor;
	public float branchLenght;
	public int maxBranch;
	public const float minBranchLenght = 0.25f;
	public const float maxBranchLenght = 1f;
	public TreeGenes()
	{
		color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
		fruitColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
		branchLenght = Random.Range(minBranchLenght, maxBranchLenght);
		maxBranch = Random.Range(2, 8);
	}
	public TreeGenes(TreeGenes parent, float mutationRate = 0.1f)
	{
		color = new Color(Mutation(mutationRate) ? Random.Range(0f, 1f) : parent.color.r, Mutation(mutationRate) ? Random.Range(0f, 1f) : parent.color.g, Mutation(mutationRate) ? Random.Range(0f, 1f) : parent.color.b, 1f);
		fruitColor = new Color(Mutation(mutationRate) ? Random.Range(0f, 1f) : parent.fruitColor.r, Mutation(mutationRate) ? Random.Range(0f, 1f) : parent.fruitColor.g, Mutation(mutationRate) ? Random.Range(0f, 1f) : parent.fruitColor.b, 1f);
		branchLenght = Mutation(mutationRate) ? Random.Range(minBranchLenght, maxBranchLenght) : parent.branchLenght;
		maxBranch = Mutation(mutationRate) ? Random.Range(2, 8) : parent.maxBranch;
	}
	bool Mutation(float rate)
	{
		return Random.Range(0f, 1f) < rate;
	}
}