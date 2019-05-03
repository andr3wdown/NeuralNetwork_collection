using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cooldown
{
	public float d;
	float c;
    public Cooldown(float d, bool init = false)
	{
		this.d = d;
		this.c = init ? d : 0f;
	}
	public void CountDown(float rate = 1f)
	{
		c -= rate * Time.deltaTime;
	}
	public bool TriggerReady(bool reset = true)
	{
		if(c <= 0)
		{
			c = reset ? d : 0;
			return true;
		}
		return false;
	}
}
