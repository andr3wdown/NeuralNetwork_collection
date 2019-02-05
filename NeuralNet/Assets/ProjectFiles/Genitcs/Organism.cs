using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour
{
    public Genetics genes;
    public float jumpForce;
    public float rotationSpeed;
    public float mass;
    public float drag;
    public Cooldown cooldown;
    public Cooldown flipTimer;
    Rigidbody2D rb;
    GenePair[] possiblePairs = { new GenePair(0, 0), new GenePair(1, 0), new GenePair(0, 1), new GenePair(1, 1), new GenePair(2, 0), new GenePair(0, 2), new GenePair(2, 1), new GenePair(1, 2), new GenePair(2, 2) };
    public bool started = false;
    public void ConstructOrganism()
    {
        rb = GetComponent<Rigidbody2D>();
        ReadGenes();
        rb.mass = mass;
        rb.drag = drag;
    }
    void ReadGenes()
    {
        jumpForce = ReadGeneticsFloat(genes.genes[0], jumpForce, 1.5f);
        rotationSpeed = ReadGeneticsFloat(genes.genes[1], rotationSpeed, 5f);
        cooldown.d = ReadGeneticsFloat(genes.genes[2], cooldown.d, 0.1f);
        mass = ReadGeneticsFloat(genes.genes[3], mass, 0.1f);
        drag = ReadGeneticsFloat(genes.genes[4], drag, 0.1f);
        flipTimer.d = ReadGeneticsFloat(genes.genes[5], flipTimer.d, 0.1f);
        ReadColorGenes();

        mass = Mathf.Clamp(mass, 0, 10);
        drag = Mathf.Clamp(drag, 0, 10);
        rotationSpeed = Mathf.Clamp(rotationSpeed, -180, 180);
        jumpForce = Mathf.Clamp(jumpForce, -50, 50);
        cooldown.d = Mathf.Clamp(cooldown.d, 0.2f, 10);
        flipTimer.d = Mathf.Clamp(flipTimer.d, 0.2f, 10);
    }
    void ReadColorGenes()
    {
        Color c = Color.white;
        c.a = 1;
        c.r = GetColorData(genes.genes[6]);
        c.g = GetColorData(genes.genes[7]);
        c.b = GetColorData(genes.genes[8]);
        GetComponent<SpriteRenderer>().color = c;
    }
    float GetColorData(GenePair pair)
    {
        int index = -1;
        for(int i = 0; i < possiblePairs.Length; i++)
        {
            if (pair.Equals(possiblePairs[i]))
            {
                index = i;
                break;
            }
        }
        if(index == -1)
        {
            Debug.LogError("InvalidPair");
        }
        return index + 1 / 9;
    }
    float ReadGeneticsFloat(GenePair pair, float value, float rate)
    {
        if (pair.Equals(possiblePairs[0]))
        {
            return value - rate;
        }
        else if (pair.Equals(possiblePairs[1]))
        {
            return value - rate / 2;
        }
        else if (pair.Equals(possiblePairs[2]))
        {
            return value - rate / 2;
        }
        else if (pair.Equals(possiblePairs[3]))
        {
            return value;
        }
        else if (pair.Equals(possiblePairs[4]))
        {
            return value;
        }
        else if (pair.Equals(possiblePairs[5]))
        {
            return value;
        }
        else if (pair.Equals(possiblePairs[6]))
        {
            return value + rate / 2;
        }
        else if (pair.Equals(possiblePairs[7]))
        {
            return value + rate / 2;
        }
        else if (pair.Equals(possiblePairs[8]))
        {
            return value + rate;
        }
        else
        {
            Debug.LogError("Invalid GenePair!");
            return 0;
        }

    }
    private void Update()
    {
        if (started)
        {
            RunOrganism();
        }
    }
    float zrot = 0;
    bool flipped = false;
    void RunOrganism()
    {
        int val = 1;
        if (flipped)
        {
            val = -1;
        }
        flipTimer.c -= Time.deltaTime;
        if(flipTimer.c <= 0)
        {
            flipped = !flipped;
            flipTimer.c = flipTimer.d;
        }

        zrot += rotationSpeed * val * Time.deltaTime * 5f;
        transform.rotation = Quaternion.Euler(0, 0, zrot);
        cooldown.c -= Time.deltaTime;
        if(cooldown.c <= 0)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            cooldown.c = cooldown.d;
        }
    }
    public float Fitness
    {
        get
        {
            return transform.position.x;
        }
    }
}
[System.Serializable]
public class Cooldown
{
    [HideInInspector]
    public float c;
    public float d;
}
