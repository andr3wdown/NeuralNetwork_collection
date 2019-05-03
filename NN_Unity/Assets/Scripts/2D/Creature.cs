using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;
using Andr3wDown.NEAT;

public class Creature : MonoBehaviour
{
	NeuralNet net;
	NEATNetwork neatNet;
	SpriteRenderer sr;
	SpriteRenderer sound;
	public float moveSpeed;
	public float rotSpeed;
	public float maxSound = 5f;
	public bool debug = false;
	Color[] colors;
	public LineRenderer mouth;
	float vitality = 6f;
	const float maxVitality = 15f;
	bool neat;
	public void InitCreature(NEATNetwork neatNet, Color[] c)
	{
		neat = true;
		colors = c;
		sr = GetComponent<SpriteRenderer>();
		sound = transform.GetChild(0).GetComponent<SpriteRenderer>();
		this.neatNet = neatNet;
	}
	public void InitCreature(NeuralNet net, Color[] c)
	{
		neat = false;
		colors = c;
		sr = GetComponent<SpriteRenderer>();
		sound = transform.GetChild(0).GetComponent<SpriteRenderer>();
		this.net = net;
	}
	Vector op = new Vector(9);
	/* 
	 * VISION: 20 * 7
	 * distance
	 * r
	 * g
	 * b
	 * food
	 * creature
	 * tree
	 * 
	 * SOUND: 3
	 * r * a
	 * g * a
	 * b * a
	 * 
	 * OTHER: 1 + output
	 * vitality
	 * memory
	 * 
	 * inputSize = 144 + outputSize
	 */

	
	private void FixedUpdate()
	{
		Vector input = GetInputs();
		if (!neat)
		{
			HandleOutput(net.Run(input));
		}
		else
		{
			HandleOutput(neatNet.RunNetwork(input));
		}
	

		vitality -= 0.06f * Time.fixedDeltaTime + (( sound.transform.localScale.x * sound.color.a / maxSound ) * 0.06f);
		if(vitality <= 0)
		{
			Destroy(gameObject);
		}

		if(vitality >= 10 && !gestating)
		{
			cd.CountDown();
			if (cd.TriggerReady())
			{
				if(Random.Range(0, 1) == 0)
				{
					gestating = true;
					StartCoroutine(Gestation());
				}
			}
		}
	}
	Cooldown cd = new Cooldown(2f);
	bool gestating = false;
	Creature partner = null;
	IEnumerator Gestation()
	{
		gestating = true;
		
		yield return new WaitForSeconds(2f);
		
		Color[] cs = new Color[2];
		for(int i = 0; i < colors.Length; i++)
		{
			float r = Mutate() ? Random.Range(0f, 1f) : partner != null ? (Random.Range(0, 2) == 0 ? partner.colors[i].r : colors[i].r) : colors[i].r;
			float g = Mutate() ? Random.Range(0f, 1f) : partner != null ? (Random.Range(0, 2) == 0 ? partner.colors[i].g : colors[i].g) : colors[i].g;
			float b = Mutate() ? Random.Range(0f, 1f) : partner != null ? (Random.Range(0, 2) == 0 ? partner.colors[i].b : colors[i].b) : colors[i].b;


			cs[i] = new Color(r, g, b, 1f);
		}
		if (!neat)
		{
			NeuralNet newNet = net.MutateAndReproduce(2, partner == null ? null : partner.net, true);
			SimulationStarter.SpawnCreature(transform.position, newNet, cs);
		}
		else
		{
			if(neatNet != null && partner != null && gameObject != null && partner.neatNet != null)
				SimulationStarter.SpawnCreature(transform.position, new NEATNetwork(NEATGenome.Reproduce(neatNet.genome, partner == null ? neatNet.genome : partner.neatNet.genome)), cs);
		}
	
		vitality /= 2f;
		gestating = false;
		partner = null;
	}
	bool Mutate(float mutationChance = 0.1f)
	{
		float s = Random.Range(0f, 1f);
		return s < mutationChance;

	}

	Vector GetInputs()
	{
		Vector input = new Vector(0);
		int half = visions / 2;
		for (int i = 0; i < visions; i++)
		{
			float val = ((float)i - half + (visions % 2 == 0 ? 0.5f : 0f)) * (visionAngle * (1f / (float)visions));
			Vector2 dir = MathOperations.AngleToVector(val + transform.rotation.eulerAngles.z + 90);
			Vector visionVector = GetVision(dir);
			input.JoinVector(visionVector);
			//Debug.DrawLine(transform.position, transform.position + (Vector3)dir * visionDst, Color.blue);
		}
		input.JoinVector(GetSound());
		float[] arr = { vitality / maxVitality };
		input.JoinVector(new Vector(arr));
		input.JoinVector(op);

		return input; 
	}
	float rotZ;
	public float maxMouth = 0.4f;
	void HandleOutput(Vector output)
	{
		op = output;
		transform.position += transform.up * output.values[0] * moveSpeed * Time.fixedDeltaTime;
		rotZ += output.values[1] * Time.fixedDeltaTime * rotSpeed;
		transform.rotation = Quaternion.Euler(0, 0, rotZ);
		Color c = new Color(0, 0, 0, 1f);
		c.r = (output.values[2] + 1f) / 2f;
		c.g = (output.values[3] + 1f) / 2f;
		c.b = (output.values[4] + 1f) / 2f;
		c.a = (output.values[5] + 1f) / 2f;
		sound.color = c;
		sound.transform.localScale = new Vector3((output.values[6] +1f) / 2f, (output.values[6] + 1f) / 2f, (output.values[6] + 1f) / 2f) * maxSound;
		sr.color = Color.Lerp(colors[0], colors[1], (output.values[7] + 1f) / 2f);
		mouth.startColor = Color.Lerp(colors[1], colors[0], (output.values[7] + 1f) / 2f);
		mouth.endColor = Color.Lerp(colors[0], colors[1], (output.values[7] + 1f) / 2f);
		Vector2 mouthPos = transform.position + transform.up * ((output.values[8] + 1f) / 2f * maxMouth);
		mouth.positionCount = 2;
		mouth.SetPosition(0, transform.position);
		mouth.SetPosition(1, mouthPos);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, (output.values[8] + 1f) / 2f * maxMouth, creatureLayer);
		if (hit)
		{
			if(hit.transform.GetComponent<Creature>() != null)
			{
				vitality += 4f * Time.fixedDeltaTime;
				hit.transform.GetComponent<Creature>().vitality -= 5f * Time.fixedDeltaTime;
			}
			if(hit.transform.GetComponent<Food>() != null)
			{
				hit.transform.GetComponent<Food>().Eaten(this);
			}	
		}
		vitality -= ((output.values[8] + 1f) / 2f) * 0.12f * Time.deltaTime; 
	}
	public int visions = 10;
	public float visionAngle = 45;
	public float visionDst = 10;
	public LayerMask visible;
	public LayerMask audible;
	public LayerMask creatureLayer;
	Vector GetVision(Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, visionDst, visible);
		Vector v = new Vector(7);
		if (hit)
		{
			if (debug)
			{
				Debug.DrawLine(transform.position, hit.point, Color.blue);
			}
			v.SetValue(0, Vector2.Distance(transform.position, hit.point));
			Color c = hit.transform.GetComponent<SpriteRenderer>().color;
			v.SetValue(1, c.r);
			v.SetValue(2, c.g);
			v.SetValue(3, c.b);
			if(hit.transform.GetComponent<Food>() != null)
			{
				v.SetValue(4, 1f);
			}
			if(hit.transform.GetComponent<Tree>() != null)
			{
				v.SetValue(5, 1f);
			}
			if(hit.transform.GetComponent<Creature>() != null)
			{
				v.SetValue(6, 1f);
			}
		}
		return v;
	}
	Vector GetSound()
	{
		Collider2D[] sounds = Physics2D.OverlapCircleAll(transform.position, 0.1f, audible);
		float r = 0, g = 0, b = 0;
		foreach(Collider2D sound in sounds)
		{
			Color c = sound.GetComponent<SpriteRenderer>().color;
			r += c.r * c.a;
			g += c.g * c.a;
			b += c.b * c.a;
		}
		r /= sounds.Length;
		g /= sounds.Length;
		b /= sounds.Length;
		float[] arr = { r, g, b };
		return new Vector(arr);
	}
	public void Feed()
	{
		vitality += 4f;
		if(vitality >= maxVitality)
		{
			vitality = maxVitality;
		}
	}
	private void OnDrawGizmosSelected()
	{
	/*	if (debug)
		{
			int half = visions / 2;
			for(int i = 0; i < visions; i++)
			{
				float val = ((float)i - half  + (visions % 2 == 0 ? 0.5f : 0f) ) * (visionAngle * (1f / (float)visions));
				Vector2 dir = MathOperations.AngleToVector(val + transform.rotation.eulerAngles.z + 90);
				Debug.DrawLine(transform.position, transform.position + (Vector3)dir * visionDst, Color.blue);
			}
		}
		*/
	}
	private void OnDisable()
	{
		SimulationStarter.creatures.Remove(this);
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.layer == 10)
		{
			Creature c = collision.gameObject.GetComponent<Creature>();

			if (neat)
			{
				if(c!= null && c.neatNet != null)
				if(NEATGenome.GetCompababliltyRating(neatNet.genome, c.neatNet.genome) < 0.5f)
				{
					c.partner = this;
					partner = c;
				}
			}
			else
			{
				c.partner = this;
				partner = c;
			}
		

		}
	}
	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<Tree>() != null)
		{
			vitality -= 0.2f * Time.deltaTime;
		}
	}
}
