using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

public class DummyNetController : MonoBehaviour
{

    public NeuralNet net;
    public List<GenePair> colorGenes = new List<GenePair>();
    public LayerMask groundLayer;
    Rigidbody2D rb;
    bool hasBeenInitialized = false;
    
    public void InitNet(int inputSize, int[] layerSizes, int outputSize)
    {
        rb = GetComponent<Rigidbody2D>();
        net = new NeuralNet(inputSize, layerSizes[0], layerSizes[1], outputSize);     
        for(int i = 0; i < 3; i++)
        {
            colorGenes.Add(new GenePair(net.GetRandomInt, net.GetRandomInt));
        }
        ReadColorData();
        hasBeenInitialized = true;
    }
    public void InitNet(NeuralNet _net, List<GenePair> _cGenes)
    {
        rb = GetComponent<Rigidbody2D>();
        net = _net;
        colorGenes = _cGenes;
        ReadColorData();
        hasBeenInitialized = true;
    }
    void ReadColorData()
    {
        Color c = new Color();
        c.a = 1;
        c.r = GetColorData(colorGenes[0]);
        c.g = GetColorData(colorGenes[1]);
        c.b = GetColorData(colorGenes[2]);
        GetComponent<SpriteRenderer>().color = c;
    }
    float GetColorData(GenePair p)
    {
        float f = -1f;

        for(int i = 0; i < net.possiblePairs.Length; i++)
        {
            if (p.Equals(net.possiblePairs[i]))
            {
                f = (float)i / 8f;
            }
        }
        return f;
    }
    public List<GenePair> MutatedColors()
    {
        int index = net.GetRandomInt;
        if(index == 0)
        {
            index = net.GetRandomInt;
            colorGenes[index] = new GenePair(net.GetRandomInt, net.GetRandomInt);
        }
        return colorGenes;
    }


    private void Update()
    {
        if (hasBeenInitialized)
        {
            HandleOutput();
        }      
    }
    Vector RunBrains()
    {
        return net.Run(GetInput());
    }
    float zrot;
    float memory1;
    float memory2;
    void HandleOutput()
    {
        
        Vector output = RunBrains();
        if(GetGrounded() > 0 && output.values[0] > 0)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(transform.up * 12 * output.values[1], ForceMode2D.Impulse);   
            //rb.MovePosition(transform.position + transform.up * Time.deltaTime * 5 * output.values[1]);        
        }
        zrot += output.values[2] * (360 * output.values[3]) * Time.deltaTime;
        zrot = MathOperations.RepeatingRange(zrot, 0, 360);
        transform.rotation = Quaternion.Euler(0, 0, (float)zrot);
        //Debug.Log(output.values[0] + ", " + output.values[1] + ", " + output.values[2] + ", " + output.values[3]);
        memory1 = output.values[4];
        memory2 = output.values[5];
    }
    Vector GetInput()
    {
        Vector input = new Vector(28);
        input.values[0] = GetGrounded();
        Vector2 pos = GetOwnPosition();
        input.values[1] = pos.x;
        input.values[2] = pos.y;
        float dist = 0;
        Vector2 normal;
        pos = GetDistanceToFront(transform.up, out dist, out normal);
        input.values[3] = pos.x;
        input.values[4] = pos.y;
        input.values[5] = dist;
        input.values[6] = GetAngle();
        input.values[7] = normal.x;
        input.values[8] = normal.y;
        input.values[9] = memory1;
        input.values[10] = memory2;
        pos = GetDistanceToFront((transform.up + transform.right).normalized, out dist, out normal);
        input.values[11] = pos.x;
        input.values[12] = pos.y;
        input.values[13] = dist;
        input.values[14] = normal.x;
        input.values[15] = normal.y;
        pos = GetDistanceToFront((transform.up - transform.right).normalized, out dist, out normal);
        input.values[16] = pos.x;
        input.values[17] = pos.y;
        input.values[18] = dist;
        input.values[19] = normal.x;
        input.values[20] = normal.y;
        pos = GetDistanceToFront(-transform.up, out dist, out normal);
        input.values[21] = pos.x;
        input.values[22] = pos.y;
        input.values[23] = dist;
        input.values[24] = normal.x;
        input.values[25] = normal.y;
        input.values[26] = rb.velocity.x;
        input.values[27] = rb.velocity.y;
        return input;
    }
    Vector2 GetDistanceToFront(Vector2 dir,out float distance, out Vector2 surfNormal)
    {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, groundLayer);
        if(hit.point != null)
        {
            
            distance = MathOperations.Sigmoid(Vector2.Distance(transform.position, hit.point));
            surfNormal = hit.normal.normalized;
            return (hit.point - (Vector2)transform.position).normalized;
        }
        else
        {
            distance = 0f;
            surfNormal = Vector2.zero;
            return Vector2.zero;
        }
    }
    Vector2 GetOwnPosition()
    {
        return transform.position.normalized;
    }
    float GetAngle()
    {
        return MathOperations.NormalizeFloat(transform.rotation.z, 0, 360);
    }
    float GetGrounded()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, 1.005f, groundLayer);
        if(col == null)
        {
            return -1f;
        }
        else
        {
            return 1f;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 1.005f);
    }
    public float Fitness
    {
        get
        {
            return transform.position.x;
        }
    }

}
