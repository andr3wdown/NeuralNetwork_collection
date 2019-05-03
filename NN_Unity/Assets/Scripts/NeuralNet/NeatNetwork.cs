using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andr3wDown.Math;

namespace Andr3wDown.NEAT
{

	// not sure if this is right. :P
	public class NEATGenome : System.IComparable<NEATGenome>
	{
		static int currentInnovation = 0;
		public static Dictionary<int, Connection> connectionLibrary = new Dictionary<int, Connection>();

		public Dictionary<int, Node> nodes = new Dictionary<int, Node>();
		public Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
		public readonly int inputDim;
		public readonly int outputDim;
		public float Fitness { get; set; }
		public float Error { get; set; }
		public NEATGenome()
		{

		}
		public NEATGenome(int inputSize, int outputSize, bool init=true)
		{
			connectionLibrary = new Dictionary<int, Connection>();

			inputDim = inputSize;
			outputDim = outputSize;
			if (init)
			{
				currentInnovation = 0;
				for (int i = 0; i < inputSize; i++)
				{
					nodes.Add(nodes.Count, new Node(nodes.Count, NodeType.input));
				}
				for (int i = 0; i < outputSize; i++)
				{
					nodes.Add(nodes.Count, new Node(nodes.Count, NodeType.output));
				}
			}
		
		}


		public void CreateNewConnection()
		{
			Node from = nodes[Random.Range(0, nodes.Count)];
			Node to = nodes[Random.Range(0, nodes.Count)];

			if(from.type == NodeType.input && to.type == NodeType.input || from.type == NodeType.output && to.type == NodeType.output)
			{
				CreateNewConnection();
				return;
			}

			bool reversed = from.type == NodeType.hidden && to.type == NodeType.input || from.type == NodeType.output && to.type == NodeType.hidden || from.type == NodeType.output && to.type == NodeType.input;
			bool exists = false;
			foreach (Connection c in connections.Values)
			{
				if(c.from == from.id && c.to == to.id || c.from == to.id && c.to == from.id)
				{
					exists = true;
					break;
				}
			}
			if (!exists)
			{

				Connection newC = new Connection(reversed ? to.id : from.id, reversed ? from.id : to.id, Random.Range(-2f, 2f), true, currentInnovation);
				bool newInnovation = true;
				foreach(Connection c in connectionLibrary.Values)
				{
					if (newC.Equals(c))
					{
						newInnovation = false;
						newC.innovation = c.innovation;
						break;
					}
				}
				
				
				if (newInnovation)
				{
					connectionLibrary.Add(newC.innovation, newC);
				}
				connections.Add(newC.innovation, newC);
				currentInnovation += newInnovation ? 1 : 0;
			}
			else
			{
				//CreateNewConnection();
				//return;
			}

		}
		public void AddConnection(Connection newC)
		{
			bool newInnovation = true;
			foreach (Connection c in connectionLibrary.Values)
			{
				if (newC.Equals(c))
				{
					newInnovation = false;
					newC.innovation = c.innovation;
					break;
				}
			}

			if (connections.ContainsKey(newC.innovation))
			{
				Debug.Log($"Connection from {newC.from} to {newC.to} already exists with innovation number {newC.innovation}");
				return;
			}

			if (newInnovation)
			{
				connectionLibrary.Add(newC.innovation, newC);
			}
			connections.Add(newC.innovation, newC);
			currentInnovation += newInnovation ? 1 : 0;
		}
		public void CreateNewNode()
		{
			Connection[] cons = connections.Values.ToArray();
			int index = Random.Range(0, cons.Length);
			Connection con = cons[index];
		


			Node from = nodes[con.from];
			Node to = nodes[con.to];

			con.Disable();

			Node newNode = new Node(nodes.Count, NodeType.hidden);
			Connection fromToNew = new Connection(from.id, newNode.id, 1f, true, currentInnovation); currentInnovation++;
			Connection newToTo = new Connection(newNode.id, to.id, con.weight, true, currentInnovation); currentInnovation++;
			currentInnovation -= 2;

			nodes.Add(newNode.id, newNode);
			AddConnection(fromToNew);
			AddConnection(newToTo);
			//connections.Add(fromToNew.innovation, fromToNew);
			//connections.Add(newToTo.innovation, newToTo);
		}

		public int GenomeSize
		{
			get
			{
				return nodes.Count + connections.Count;
			}
		}
		public static float GetCompababliltyRating(NEATGenome parent1, NEATGenome parent2, float c1 = 1f, float c2 = 1f, float c3 = 0.2f)
		{
			int exccesGenes = ExcessGenes(parent1, parent2);
			int disjointGenes = DisjointGenes(parent1, parent2);
			float connectionDiffrence = GetConnectionDiffrence(parent1, parent2);
			int genomeSize = Mathf.Max(parent1.GenomeSize, parent2.GenomeSize) / 2;
			return (c1 * exccesGenes / genomeSize) + (c2 * disjointGenes / genomeSize) + c3 * connectionDiffrence;
		}
		public static float GetConnectionDiffrence(NEATGenome parent1, NEATGenome parent2)
		{
			float diffrence = 0;
			int matchingGenes = Mathf.Min(parent1.nodes.Count, parent2.nodes.Count);
			
			int[] key1 = parent1.connections.Keys.ToArray();
			int[] key2 = parent2.connections.Keys.ToArray();
			System.Array.Sort(key1);
			System.Array.Sort(key2);
			int size = Mathf.Max(key1[key1.Length - 1], key2[key2.Length - 1]);

			for (int i = 0; i < size; i++)
			{
				if (parent1.connections.ContainsKey(i) && parent2.connections.ContainsKey(i))
				{

					matchingGenes++;
					Connection c1;
					Connection c2;
					parent1.connections.TryGetValue(i, out c1);
					parent2.connections.TryGetValue(i, out c2);
					if(c1 != null && c2 != null)
					{
						diffrence += Mathf.Abs(c1.weight - c2.weight);
					}
					else
					{
						Debug.LogError("invalid connection!");
					}
			
				}
			}
			return diffrence / matchingGenes;
		}
		public static int GetMatchingGenes(NEATGenome parent1, NEATGenome parent2)
		{
			int matchingGenes = Mathf.Min(parent1.nodes.Count, parent2.nodes.Count);

			int[] key1 = parent1.connections.Keys.ToArray();
			int[] key2 = parent2.connections.Keys.ToArray();
			System.Array.Sort(key1);
			System.Array.Sort(key2);
			int size = Mathf.Max(key1[key1.Length-1], key2[key2.Length-1]);

			for(int i = 0; i < size; i++)
			{
				if(parent1.connections.ContainsKey(i) && parent2.connections.ContainsKey(i))
				{
					matchingGenes++;
				}
			}
			return matchingGenes;
		}
		public static int DisjointGenes(NEATGenome parent1, NEATGenome parent2)
		{
			int disjointGenes = 0;
			int[] key1 = parent1.connections.Keys.ToArray();
			int[] key2 = parent2.connections.Keys.ToArray();
			System.Array.Sort(key1);
			System.Array.Sort(key2);
			int size = Mathf.Max(key1[key1.Length - 1], key2[key2.Length - 1]);

			for (int i = 0; i < size; i++)
			{
				if (parent1.connections.ContainsKey(i) && !parent2.connections.ContainsKey(i) && i < key1[key1.Length - 1])
				{
					disjointGenes++;
				}
				else if (!parent1.connections.ContainsKey(i) && parent2.connections.ContainsKey(i) && i < key2[key2.Length - 1])
				{
					disjointGenes++;
				}		
			}
			return disjointGenes;
		}
		public static int ExcessGenes(NEATGenome parent1, NEATGenome parent2)
		{
			int excessGenes = Mathf.Max(parent1.nodes.Count, parent2.nodes.Count) - Mathf.Min(parent1.nodes.Count, parent2.nodes.Count);
			int[] key1 = parent1.connections.Keys.ToArray();
			int[] key2 = parent2.connections.Keys.ToArray();
			System.Array.Sort(key1);
			System.Array.Sort(key2);
			int size = Mathf.Max(key1[key1.Length - 1], key2[key2.Length - 1]);

			for (int i = 0; i < size; i++)
			{
				if (parent1.connections.ContainsKey(i) && !parent2.connections.ContainsKey(i) && i > key1[key1.Length - 1])
				{
					excessGenes++;
				}
				else if (!parent1.connections.ContainsKey(i) && parent2.connections.ContainsKey(i) && i > key2[key2.Length - 1])
				{
					excessGenes++;
				}
			}
			return excessGenes;
		}

		public static NEATGenome Reproduce(NEATGenome parent1, NEATGenome parent2, float connectionRate = 0.2f, float nodeRate = 0.2f)
		{
			NEATGenome newNetwork = new NEATGenome(parent1.inputDim, parent1.outputDim, false);
			foreach (Node n in parent1.nodes.Values)
			{
				newNetwork.nodes.Add(newNetwork.nodes.Count, n);
			}
			foreach (Connection c in parent1.connections.Values)
			{
				if (parent2.connections.ContainsKey(c.innovation))
				{
					Connection n;
					if (Random.Range(0, 2) == 0)
					{
						parent1.connections.TryGetValue(c.innovation, out n);

					}
					else
					{
						parent2.connections.TryGetValue(c.innovation, out n);
					}

					if (n != null)
					{
						newNetwork.connections.Add(n.innovation, n.Copy());
					}
					else
					{
						Debug.LogError("Invalid Connection!");
					}
				}
				else
				{
					newNetwork.connections.Add(c.innovation, c.Copy());
				}
			}
			Mutate(ref newNetwork);
			if(Random.Range(0f, 1f) < connectionRate)
			{
				newNetwork.CreateNewConnection();
			}
			if (Random.Range(0f, 1f) < nodeRate)
			{
				newNetwork.CreateNewNode();
			}
			return newNetwork;

		}
		public static void Mutate(ref NEATGenome child, float treshold = 0.9f)
		{		
			foreach (Connection c in child.connections.Values)
			{
				float r = Random.Range(0f, 1f);
				if (r < treshold)
				{

					c.weight *= Random.Range(0.9f, 1.1f) * (Random.Range(0, 2) == 0 ? 1 : -1);
				}
				else
				{
					c.weight = Random.Range(-1f, 1f);
				}
			}
		}
		public int CompareTo(NEATGenome other)
		{
			if (Fitness == other.Fitness)
			{
				return 0;
			}
			else if (Fitness > other.Fitness)
			{
				return 1;
			}
			else if (Fitness < other.Fitness)
			{
				return -1;
			}
			return 0;
			//throw new System.Exception("invalid genome sorting!");
		}
	}

	public class NEATNetwork
	{

		public NEATGenome genome;
		protected List<Neuron> neurons = new List<Neuron>();
		
		public NEATNetwork(NEATGenome _genome)
		{
			genome = _genome;
			ConstructNetwork();
		}
		void ConstructNetwork()
		{
			foreach (Node n in genome.nodes.Values)
			{
				List<int> cons = new List<int>();
				List<float> ws = new List<float>();
				foreach(Connection c in genome.connections.Values)
				{
					if(c.from == n.id)
					{
						cons.Add(c.to);
						ws.Add(c.weight);
					}
				}
				Neuron ne = new Neuron(n, cons.ToArray(), ws.ToArray());
				neurons.Add(ne);
			}
		}

		public virtual Vector RunNetwork(Vector input)
		{
			List<float> tempOutput = new List<float>();
			List<int> allCons = new List<int>();
			HashSet<int> checkCons = new HashSet<int>();
			for(int i = 0; i < genome.inputDim; i++)
			{
				neurons[i].AddToValue(input.values[i]);
				int[] l = neurons[i].Activate(ref neurons);
				for (int j = 0; j < l.Length; j++)
				{
					if (!checkCons.Contains(l[j]))
					{
						allCons.Add(l[j]);
						checkCons.Add(l[j]);
					}
				}			
			}
				
			int iter = 0;
			while (iter < 3000)
			{
				bool h = false;
				int[] nextCons = allCons.ToArray();
				allCons.Clear();
				checkCons.Clear();
				for (int i = 0; i < nextCons.Length; i++)
				{
					if (!neurons[nextCons[i]].activated && neurons[nextCons[i]].type == NodeType.hidden)
					{
						h = true;
						int[] l = neurons[nextCons[i]].Activate(ref neurons);
						for (int j = 0; j < l.Length; j++)
						{
							if (!checkCons.Contains(l[j]))
							{
								allCons.Add(l[j]);
								checkCons.Add(l[j]);
							}
						}
					}
					else if(neurons[nextCons[i]].activated && neurons[nextCons[i]].type == NodeType.hidden)
					{
						neurons[nextCons[i]].Activate(ref neurons);
					}
				}
				
				if (!h)
				{

					for (int i = genome.inputDim; i < genome.inputDim + genome.outputDim; i++)
					{
						if (neurons[i].type == NodeType.output)
						{
							tempOutput.Add(neurons[i].output);						
						}
						else
						{
							throw new System.Exception("invalid neuron type!");
						}
						
					}
					break;
				}
				iter++;
			}

			for(int i = 0; i < neurons.Count; i++)
			{
				neurons[i].ResetPass();
			}
			return new Vector(tempOutput.ToArray());
		}
		
		public class Neuron
		{
			public NodeType type;
			public bool activated = false;
			int id;
			int[] connections;
			float[] weights;
			float value = 0f;
			string activation;
			public int Distance { get; set; }

			public Neuron(Node node, int[] cons, float[] ws)
			{
				id = node.id;
				type = node.type;
				connections = cons;
				weights = ws;
			}


			public int[] Activate(ref List<Neuron> neurons)
			{
				//value = Activation("sigmoid", value);
				for (int i = 0; i < connections.Length; i++)
				{
					
					neurons[connections[i]].AddToValue(output * weights[i]);
				}
				value = 0f; //!activated ? 0f : value;
				activated = true;
				return connections;
			}
			public float output
			{
				get
				{
					return Activation("tanh", value);
				}
				
			}
			public void AddToValue(float val)
			{
				value += val;
			}
			float Activation(string type, float value)
			{
				if (type == "relu")
				{
					return MathOperations.ReLU(value);
				}
				else if (type == "leaky_relu")
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
				else if (type == "softmax")
				{
					return MathOperations.Softmax(value);
				}
				else if (type == "none")
				{
					return value;
				}
				else
				{
					Debug.LogError("invalid activation function");
					return 0;
				}
			}
			public void ResetPass()
			{
				//value = 0;
				activated = false;
			}
		}
	}
	public class NEATNetworkDistance : NEATNetwork
	{
		public NEATNetworkDistance(NEATGenome g) : base(_genome: g)
		{

			HashSet<int> cons = new HashSet<int>();
			for (int i = 0; i < genome.inputDim; i++)
			{
				neurons[i].Distance = 0;
				//for(int j = 0; j < neurons[i].)
			}


			foreach (Neuron n in neurons)
			{
				if (n.type == NodeType.input)
				{
					n.Distance = 0;
					continue;
				}
				

			}
		}
	}

	public class Connection : System.IEquatable<Connection>
	{
		public readonly int from;
		public readonly int to;
		public float weight;
		bool enabled;
		public int innovation;

		public Connection(int f, int t, float w, bool u, int i)
		{
			this.from = f;
			this.to = t;
			this.weight = w;
			enabled = u;
			this.innovation = i;
		}

		public bool GetEnabled()
		{
			return enabled;
		}
		public void Disable()
		{
			enabled = false;
		}
		public Connection Copy()
		{
			return new Connection(from, to, weight, enabled, innovation);
		}
		public bool Equals(Connection other)
		{
			if(from == other.from && to == other.to)
			{
				return true;
			}
			return false;
		}
	}

	public class Node
	{
		public readonly NodeType type;
		public readonly int id;

		public Node(int i, NodeType t)
		{
			this.type = t;
			this.id = i;
		}
	}

	public enum NodeType
	{
		input,
		output,
		hidden
	}

	public abstract class NEATPopulation
	{
		public int popSize = 100;

		protected Dictionary<NEATNetwork, NEATGenome> currentPopulation = new Dictionary<NEATNetwork, NEATGenome>();
		public List<Species> species = new List<Species>();
		public List<NEATGenome> genomes = new List<NEATGenome>();
		public List<NEATNetwork> networks = new List<NEATNetwork>();
	
		
		protected void Init(int inputSize, int outputSize, int startConnections)
		{
			for (int i = 0; i < popSize; i++)
			{
				NEATGenome newGenome = new NEATGenome(inputSize, outputSize);
				for(int j = 0; j < startConnections; j++)
				{
					newGenome.CreateNewConnection();
				}
				Connection c = new Connection(0, 2, 1, true, 0);
				Connection c2 = new Connection(1, 2, 1, true, 1);
				newGenome.AddConnection(c);
				newGenome.AddConnection(c2);
				genomes.Add(newGenome);
				NEATNetwork net = new NEATNetwork(newGenome);
				networks.Add(net);
				currentPopulation.Add(net, newGenome);
			}
		}
		public void NextGeneration(List<NEATGenome> newGenomes)
		{
			if(newGenomes.Count != popSize)
			{
				throw new System.Exception("Invalid population initializer!");
			}
			currentPopulation.Clear();
			species.Clear();
			genomes.Clear();
			networks.Clear();
			//NEATGenome[] a = new NEATGenome[newGenomes.Count];
			//newGenomes.CopyTo(a);
			//genomes = a.ToList();
			//newGenomes.Clear();
			for(int i = 0; i < newGenomes.Count; i++)
			{
				genomes.Add(newGenomes[i]);
				NEATNetwork net = new NEATNetwork(newGenomes[i]);
				networks.Add(net);
				currentPopulation.Add(net, newGenomes[i]);
			}
			newGenomes.Clear();
		}
		public NEATNetwork AddToPop(NEATGenome newGenome)
		{
			genomes.Add(newGenome);
			NEATNetwork net = new NEATNetwork(newGenome);
			networks.Add(net);
			SpeciateOne(newGenome);
			currentPopulation.Add(net, newGenome);
			return net;
		}
		public void RemoveFromPop(NEATNetwork toRemove)
		{
			foreach(Species s in species)
			{
				if (s.Contains(toRemove.genome))
				{
					int sSize = s.RemoveMember(toRemove.genome);
					if(sSize <= 0)
					{
						species.RemoveAt(species.IndexOf(s));
						break;
					}

				}
			}
			networks.Remove(toRemove);
			genomes.Remove(toRemove.genome);
			currentPopulation.Remove(toRemove);
		}
		public void Speciate()
		{
			species.Clear();
			species.Add(new Species(genomes[0]));
			for (int i = 1; i < genomes.Count; i++)
			{
				bool newSpecies = true;
				foreach (Species s in species)
				{
					if (s.BelongsToSpecies(genomes[i]))
					{
						newSpecies = false;
						s.AddMemeber(genomes[i]);
						break;
					}
				}
				if (newSpecies)
				{
					species.Add(new Species(genomes[i]));
				}
			}
		}
		public void SpeciateOne(NEATGenome genome)
		{
			bool newSpecies = true;
			foreach (Species s in species)
			{
				if (s.BelongsToSpecies(genome))
				{
					newSpecies = false;
					s.AddMemeber(genome);
					break;
				}
			}
			if (newSpecies)
			{
				species.Add(new Species(genome));
			}
		}
		public abstract void Evaluate();
		public abstract NEATGenome GetLastGenBest();
	}

	public class Population : NEATPopulation
	{
		public NEATNetwork lastGenBest;
		public NEATGenome lastGenGenome;
		int generations = 0;
		public Population(int _popSize, int _inputSize, int _outputSize, int startConnections = 5) 
		{
			generations = 0;
			this.popSize = _popSize;
			Init(_inputSize, _outputSize, startConnections);
		}
		public override void Evaluate()
		{

		}
		public virtual void Evaluate(bool verbose = false)
		{
			genomes.Sort();
			float bestFitness = float.MinValue;
			for(int i = 0; i < networks.Count; i++)
			{
				if(networks[i].genome.Fitness > bestFitness)
				{
					bestFitness = networks[i].genome.Fitness;
					lastGenBest = networks[i];
				}
			}
			species.Clear();
			species.Add(new Species(genomes[0]));
			for (int i = 1; i < genomes.Count; i++)
			{
				bool newSpecies = true;
				foreach (Species s in species)
				{
					if (s.BelongsToSpecies(genomes[i]))
					{
						newSpecies = false;
						s.AddMemeber(genomes[i]);
						break;
					}
				}
				if (newSpecies)
				{
					species.Add(new Species(genomes[i]));
				}
			}
			foreach (Species s in species)
			{

				s.AdjustFitnesses();
				s.SortMembersByFitness();
			}
			species.Sort();

			List<NEATGenome> newPop = new List<NEATGenome>();
			for (int i = 0; i < popSize; i++)
			{
				Species rSpecies = GetRandomSpecies();
				NEATGenome newGenome = NEATGenome.Reproduce(rSpecies.GetRandomMember(), rSpecies.GetRandomMember());
				newPop.Add(newGenome);
			}
			generations++;
			NextGeneration(newPop);
		}
		public virtual void Evaluate(Vector x, Vector y)
		{
			NEATNetwork best = networks[0];
			float bestError = float.MaxValue;
			for(int i = 0; i < networks.Count; i++)
			{
				Vector output = networks[i].RunNetwork(x);
				float error = Error(output.GetValue(0), y.GetValue(0));
				if(error < bestError)
				{
					bestError = error;
					best = networks[i];
					
				}
				NEATGenome genome;
				currentPopulation.TryGetValue(networks[i], out genome);

				if(genome != null)
				{
					genome.Fitness = (1f / error) * 100f;
				}
				else
				{
					throw new System.Exception("invalid network to genome");
				}
			}
			
			lastGenBest = best;
			lastGenGenome = null;
			currentPopulation.TryGetValue(best, out lastGenGenome);
			if(lastGenGenome == null)
			{
				throw new System.Exception("Can't find genome for network?");
			}
			species.Add(new Species(genomes[0]));
			for(int i = 1; i < genomes.Count; i++)
			{
				bool newSpecies = true;
				foreach(Species s in species)
				{
					if (s.BelongsToSpecies(genomes[i]))
					{
						newSpecies = false;
						s.AddMemeber(genomes[i]);
						break;
					}
				}
				if (newSpecies)
				{
					species.Add(new Species(genomes[i]));
				}
			}
			foreach(Species s in species)
			{
				
				s.AdjustFitnesses();
				s.SortMembersByFitness();
			}
			species.Sort();

			List<NEATGenome> newPop = new List<NEATGenome>();
			for(int i = 0; i < popSize; i++)
			{
				Species rSpecies = GetRandomSpecies();
				NEATGenome newGenome = NEATGenome.Reproduce(rSpecies.GetRandomMember(), rSpecies.GetRandomMember());
				newPop.Add(newGenome);
			}
			Debug.Log("Generation: " + generations + " | Error: " + bestError * 100f + "%" + " | Species: " + species.Count);
			generations++;
			NextGeneration(newPop);
		}
		public virtual void Evaluate(Vector[] x, Vector[] y)
		{
			for(int k = 0; k < x.Length; k++)
			{
				
				for (int i = 0; i < networks.Count; i++)
				{
					Vector output = networks[i].RunNetwork(x[k]);
					float error = Error(output.GetValue(0), y[k].GetValue(0));
					/*if (error < bestError)
					{
						bestError = error;
						best = networks[i];

					}*/
					NEATGenome genome;
					currentPopulation.TryGetValue(networks[i], out genome);

					if (genome != null)
					{
						genome.Error += error;
						genome.Fitness += (1f / (error + 0.001f)) * 100f;
					}
					else
					{
						throw new System.Exception("invalid network to genome");
					}
				}
			}
			NEATNetwork best = networks[0];
			float BestFitness = float.MinValue;
			for(int i = 0; i < networks.Count; i++)
			{
				NEATGenome genome;
				currentPopulation.TryGetValue(networks[i], out genome);
				if (genome.Fitness > BestFitness)
				{
					BestFitness = genome.Fitness;
					best = networks[i];
				}
			}


			lastGenBest = best;
			lastGenGenome = null;
			currentPopulation.TryGetValue(best, out lastGenGenome);
			if (lastGenGenome == null)
			{
				throw new System.Exception("Can't find genome for network?");
			}
			species.Add(new Species(genomes[0]));
			for (int i = 1; i < genomes.Count; i++)
			{
				bool newSpecies = true;
				foreach (Species s in species)
				{
					if (s.BelongsToSpecies(genomes[i]))
					{
						newSpecies = false;
						s.AddMemeber(genomes[i]);
						break;
					}
				}
				if (newSpecies)
				{
					species.Add(new Species(genomes[i]));
				}
			}
			foreach (Species s in species)
			{

				s.AdjustFitnesses();
				s.SortMembersByFitness();
			}
			species.Sort();
			genomes.Sort();
			List<NEATGenome> newPop = new List<NEATGenome>();
			for(int i = 0; i < 5; i++)
			{
				newPop.Add(genomes[i]);
			}
			for (int i = 0; i < popSize - 5; i++)
			{
				Species rSpecies = GetRandomSpecies();
				NEATGenome newGenome = NEATGenome.Reproduce(rSpecies.GetRandomMember(), rSpecies.GetRandomMember());
				newPop.Add(newGenome);
			}
			Debug.Log("Generation: " + generations + " | Error: " + lastGenGenome.Error / x.Length * 100f + "%" + " | Species: " + species.Count);
			generations++;
			NextGeneration(newPop);
		}
		public void TestOutput(Vector x, Vector y)
		{
			Vector output = lastGenBest.RunNetwork(x);
			float error = Error(output.values[0], y.values[0]);

			Debug.Log("Network prediction: " + output.values[0] + " | Answer: " + y.values[0] + " | Network error: " + error * 100);
		}
		Species GetRandomSpecies()
		{
			float[] fitnesstresholds = new float[species.Count];
			float currentTreshold = 0f;
			for(int i = 0; i < fitnesstresholds.Length; i++)
			{
				currentTreshold += Mathf.Pow(species[i].BestFitness, 2);
				fitnesstresholds[i] = currentTreshold;
			}
			float r = Random.Range(0f, currentTreshold);

			for (int i = 0; i < fitnesstresholds.Length; i++)
			{
				if(r < fitnesstresholds[i])
				{
					return species[i];
				}
			}

			return species[Random.Range(0, species.Count)];
		}
		public float Error(float f, float l)
		{
			//Debug.Log(f + "P " + l + "L");
			float e = f-l;
			//Debug.Log(Mathf.Abs(e) * 100);
			return Mathf.Abs(e);
		}
		public override NEATGenome GetLastGenBest()
		{
			if(lastGenGenome == null)
			{
				throw new System.Exception("Last generation best genome missing?");
			}
			return lastGenGenome;
		}
		
	}
	
	public class Species : System.IComparable<Species>
	{
		NEATGenome mascot;
		public List<NEATGenome> members = new List<NEATGenome>();
		public float BestFitness { get; set; }
		public float AverageFitness
		{
			get
			{
				float total = 0f;
				for(int i = 0; i < members.Count; i++)
				{
					total += members[i].Fitness;
				}
				return total / members.Count;
			}
		}
		public Species(NEATGenome genome)
		{
			mascot = genome;
			members.Add(genome);
		}
		public void AddMemeber(NEATGenome genome)
		{
			members.Add(genome);
		}
		public bool BelongsToSpecies(NEATGenome toCompare, float compTreshold = 0.3f)
		{
			return NEATGenome.GetCompababliltyRating(mascot, toCompare) < compTreshold;
		}
		public void AdjustFitnesses()
		{
			for(int i = 0; i < members.Count; i++)
			{
				members[i].Fitness = AdjustedFitness(members[i].Fitness);
			}
		}
		public float AdjustedFitness(float fitness)
		{
			return fitness / members.Count();
		}
		public void SortMembersByFitness()
		{
			members.Sort();
			BestFitness = members[0].Fitness;
		}
		public NEATGenome GetRandomMember()
		{
			float[] fitnesstresholds = new float[members.Count];
			float currentTreshold = 0f;
			for (int i = 0; i < fitnesstresholds.Length; i++)
			{
				currentTreshold += Mathf.Pow(members[i].Fitness, 4);
				fitnesstresholds[i] = currentTreshold;
			}
			float r = Random.Range(0f, currentTreshold);

			for (int i = 0; i < fitnesstresholds.Length; i++)
			{
				if (r < fitnesstresholds[i])
				{
					return members[i];
				}
			}

			return members[Random.Range(0, members.Count)];
		}
		public int CompareTo(Species other)
		{
			if(BestFitness == other.BestFitness)
			{
				return 0;
			}
			else if(BestFitness > other.BestFitness)
			{
				return -1;
			}
			else if(BestFitness < other.BestFitness)
			{
				return 1;
			}
			return 0;
		}
		public int RemoveMember(NEATGenome genome)
		{
			members.Remove(genome);
			return members.Count;
		}
		public bool Contains(NEATGenome genome)
		{
			return members.Contains(genome);
		}
	}
}
