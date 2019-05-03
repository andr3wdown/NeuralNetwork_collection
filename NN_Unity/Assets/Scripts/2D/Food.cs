using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
	public float maxSize;
	float currentSize;
	bool isInitialized = false;
	bool hasDetached = false;
	float growthRate;
	int index = 0;
	SpriteRenderer sr;
	public void Init(Color fruitColor, int index, float growthRate, float initialSize = 0.4f)
	{
		sr = GetComponent<SpriteRenderer>();
		sr.color = fruitColor;//GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", fruitColor);

		this.index = index;
		this.growthRate = growthRate;
		currentSize = initialSize;
		transform.localScale = new Vector3(currentSize, currentSize, currentSize);
		isInitialized = true;
	}
	void Grow()
	{

		if (!hasDetached)
		{
			if (currentSize >= maxSize)
			{

				Detach();
			}
			else
			{
				currentSize += growthRate * Time.deltaTime;
			}
			transform.localScale = new Vector3(currentSize, currentSize, currentSize);
		}
	
	}
	void Detach()
	{
		hasDetached = true;
		if(transform.parent != null)
		{
			transform.root.SendMessage("DetachFruit", index);
			transform.parent = null;
		}

		currentSize = maxSize;
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.AddForce(transform.up * 1f, ForceMode2D.Impulse);
	}
	private void Update()
	{
		if (isInitialized && !hasDetached)
			Grow();
		if (hasDetached)
		{
			float f = sr.color.a;
			f -= 0.05f * Time.deltaTime;
			Color c = sr.color;
			c.a = f;
			sr.color = c;
			if(c.a <= 0)
			{
				Destroy(gameObject);
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.layer == 10)
		{
			Eaten(collision.GetComponent<Creature>());
		}
	}
	public void Eaten(Creature c)
	{
		Detach();
		c.Feed();
		Destroy(gameObject);
	}
}
