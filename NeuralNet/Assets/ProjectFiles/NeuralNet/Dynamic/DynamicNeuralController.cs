using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

public class DynamicNeuralController : MonoBehaviour
{
    const int childCount = 2;
    const float boundaryRadius = -0.1f;
    [HideInInspector]
    public NeuralNet net;
    [HideInInspector]
    public NeuralNet memoryNet;
    [HideInInspector]
    public NeuralNet inputNet;
    bool hasBeenInitialized = false;
    public float energy = 30f;
    public GameObject childPrefab;
    public Transform rotationOrgan;
    Rigidbody2D rb;
    public GameObject soundObject;
    public Cooldown soundCooldown;
    public static List<DynamicNeuralController> allControllers = new List<DynamicNeuralController>();
    LineRenderer lr;
    DynamicNeuralController partner = null;
    private void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        allControllers.Add(this);
    }
    private void OnDisable()
    {
        if (allControllers.Contains(this))
        {
            allControllers.Remove(this);
        }
    }
    public void InitNet(int[] layerSizes)
    {
        net = new NeuralNet(layerSizes[0], layerSizes[1], layerSizes[2], layerSizes[3]);
        memoryNet = new NeuralNet(layerSizes[3], layerSizes[2], layerSizes[2], layerSizes[3]);
        inputNet = new NeuralNet(layerSizes[0] - layerSizes[3], layerSizes[1], layerSizes[2], layerSizes[0] - layerSizes[3]);
        hasBeenInitialized = true;
    }
    public void InitNet(NeuralNet _net, NeuralNet _memoryNet, NeuralNet _inputNet)
    {
        net = _net;
        memoryNet = _memoryNet;
        inputNet = _inputNet;
        hasBeenInitialized = true;
    }
    private void Update()
    {
        ScreenClamp();
        if (hasBeenInitialized)
        {
            soundCooldown.c -= Time.deltaTime;
            energy -= Time.deltaTime;
            if(energy <= 0)
            {
                Destroy(gameObject);
            }
            HandleOutput(GetInput());
        }     
    }
    public void Reproduce(int _childCount = 2, int energyConsumption = 15)
    {
        for(int i = 0; i < _childCount; i++)
        {
            NeuralNet newNet = null;
            NeuralNet newMemoryNet = null;
            NeuralNet newInputNet = null;
            if(partner == null)
            {
                newNet = net.MutateAndReproduce(3);
                newMemoryNet = memoryNet.MutateAndReproduce(3);
                newInputNet = inputNet.MutateAndReproduce(3);
            }
            else
            {
                newNet = net.MutateAndReproduce(2, partner.net);
                newMemoryNet = memoryNet.MutateAndReproduce(2, partner.memoryNet);
                newInputNet = inputNet.MutateAndReproduce(2, partner.inputNet);
            }
            Vector2 dd = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            GameObject go = Instantiate(childPrefab, (Vector2)transform.position + dd, transform.rotation);
            go.GetComponent<DynamicNeuralController>().InitNet(newNet, newMemoryNet, newInputNet);
            energy -= energyConsumption;
        }
      
    }
    Vector GetInput()
    {     
        Vector inputVector = new Vector(43);
        Vector2 pos = GetOwnPosition();
        Vector2 vel = GetVelocityVector();
        //Vector2 fpos = (GridWorldController.foodPosition - (Vector2)transform.position).normalized;
        Vector2 rotVector = GetAngle();
        inputVector.SetValue(0, rotVector.x);
        inputVector.SetValue(1, rotVector.y);
        inputVector.SetValue(2, pos.x);
        inputVector.SetValue(3, pos.y);
        inputVector.SetValue(4, vel.x);
        inputVector.SetValue(5, vel.y);
        inputVector.SetValue(6, GetEnergy());
        Vector visionVector = GetVision(transform.up);
        int index = 7;
        for(int i = 0; i < visionVector.values.Length; i++)
        {
            inputVector.SetValue(index, visionVector.values[i]);
            index++;
        }

        visionVector = GetVision(transform.up + (transform.right * 0.5f));
        for (int i = 0; i < visionVector.values.Length; i++)
        {
            inputVector.SetValue(index, visionVector.values[i]);
            index++;
        }

        visionVector = GetVision(transform.up - (transform.right * 0.5f));
        for (int i = 0; i < visionVector.values.Length; i++)
        {
            inputVector.SetValue(index, visionVector.values[i]);
            index++;
        }

        visionVector = GetVision(transform.up + (transform.right * 0.25f));
        for (int i = 0; i < visionVector.values.Length; i++)
        {
            inputVector.SetValue(index, visionVector.values[i]);
            index++;
        }

        visionVector = GetVision(transform.up - (transform.right * 0.25f));
        for (int i = 0; i < visionVector.values.Length; i++)
        {
            inputVector.SetValue(index, visionVector.values[i]);
            index++;
        }
        Vector soundVector = GetSoundInput();
        inputVector.SetValue(38, soundVector.values[0]);
        inputVector.SetValue(39, soundVector.values[1]);
        inputVector.SetValue(40, soundVector.values[2]);
        inputVector.SetValue(41, soundVector.values[3]);

        Vector handledInput = inputNet.Run(inputVector);

        Vector input = new Vector(59);
        for(int i = 0; i <= 41; i++)
        {
            input.SetValue(i, handledInput.values[i]);
        }
        if (output != null && output.values.Length > 0)
        {
            Vector memoryInput = new Vector(16);
            for(int i = 0; i < output.values.Length; i++)
            {
                memoryInput.SetValue(i, output.values[i]);
            }
            Vector memoryOut = memoryNet.Run(memoryInput);
            
            input.SetValue(42, memoryOut.values[0]);
            input.SetValue(43, memoryOut.values[1]);
            input.SetValue(44, memoryOut.values[2]);
			input.SetValue(45, memoryOut.values[3]);
			input.SetValue(46, memoryOut.values[4]);
			input.SetValue(47, memoryOut.values[5]);
			input.SetValue(48, memoryOut.values[6]);
            input.SetValue(49, memoryOut.values[7]);
            input.SetValue(51, memoryOut.values[8]);
            input.SetValue(52, memoryOut.values[9]);
            input.SetValue(53, memoryOut.values[10]);
            input.SetValue(54, memoryOut.values[11]);
            input.SetValue(55, memoryOut.values[12]);
            input.SetValue(56, memoryOut.values[13]);
            input.SetValue(57, memoryOut.values[14]);
            input.SetValue(58, memoryOut.values[15]);
        }

     
        return input;
    }
   
   
    float zRot = 0;
    Vector output;
    void HandleOutput(Vector input)
    {
        output = net.Run(input);
        rb.AddForce(transform.up * (float)(output.values[0] * (output.values[1] * 25f)));
		//transform.position = Vector2.Lerp(transform.position, transform.position + transform.up, (float)(output.values[0] * (output.values[1] * 4f)) * Time.deltaTime);
		zRot += (output.values[2] * (output.values[3] * 540f) * Time.deltaTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, (float)zRot), 12f * Time.deltaTime);
		Color c = Color.white;
		c.r = output.values[4];
		c.g = output.values[5];
		c.b = output.values[6];
        GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, c, 5 * Time.deltaTime);
        Color soundColor = Color.white;
        soundColor.r = output.values[7];
        soundColor.g = output.values[8];
        soundColor.b = output.values[9];
        if(output.values[10] > 0.5f && soundCooldown.c <= 0)
        {
            soundCooldown.c = soundCooldown.d;
            float soundIntesity = 10f * MathOperations.NormalizeFloat(output.values[11], -1.0f, 1.0f); 
            GameObject go = Instantiate(soundObject, transform.position, transform.rotation);
            go.GetComponent<SoundObject>().InitSound(soundColor, soundIntesity);

        }
        MouthControl(output.values[12], output.values[13]);
        if(output.values[14] > 0.5f && energy > 15f)
        {
            Reproduce(1);
        }
    }
    public LayerMask attackLayer;

    void MouthControl(float lenght, float active, float agentRadius=0.1f, float maxDst = 0.5f)
    {
        bool isActive = true;
        if(active < 0.5f)
        {
            isActive = false;
        }

        Vector2 startPoint = Vector2.Lerp((Vector2)transform.position, (Vector2)transform.position + (Vector2)transform.up, agentRadius);
        lenght = Mathf.Clamp(lenght, 0.0f, 1.0f);
        Vector2 maxPoint = Vector2.Lerp(startPoint, startPoint + ((Vector2)transform.up * maxDst), lenght);
        

        lr.material.color = isActive ? Color.red : Color.grey;
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, maxPoint);
        
        if (isActive)
        {
            
            RaycastHit2D hit = Physics2D.Linecast(startPoint, maxPoint, attackLayer);
            if(hit.transform == this.transform)
            {
                Debug.Log("fuck");
            }
            if(hit.transform != null && hit.transform.GetComponent<DynamicNeuralController>() != null && hit.transform.GetComponent<DynamicNeuralController>() != this)
            {
                DynamicNeuralController dnc = hit.transform.GetComponent<DynamicNeuralController>();
                dnc.TakeEnergy(this);
            }
            if(hit.transform != null && hit.transform.GetComponent<Food>() != null)
            {
                hit.transform.GetComponent<Food>().Activate(this);
            }
        }
        

    }
    public void TakeEnergy(DynamicNeuralController dnc ,float power = 20.0f)
    {
        energy -= power * Time.deltaTime;
        dnc.energy += power * Time.deltaTime;       
    }
    public LayerMask soundLayer;
    Vector GetSoundInput()
    {
        Vector colorData = new Vector(4);
        Collider2D[] allObjects = Physics2D.OverlapCircleAll(transform.position, 0.1f, soundLayer);
        float r = 0;
        float g = 0;
        float b = 0;
        float a = 0;
        if (allObjects != null && allObjects.Length > 0)
        {
            for (int i = 0; i < allObjects.Length; i++)
            {
                Color c = allObjects[i].GetComponent<SpriteRenderer>().color;
                r += c.r;
                g += c.g;
                b += c.b;
                a += c.a;
            }
            r /= allObjects.Length;
            g /= allObjects.Length;
            b /= allObjects.Length;
            a /= allObjects.Length;
            colorData.SetValue(0, r);
            colorData.SetValue(1, g);
            colorData.SetValue(2, b);
            colorData.SetValue(3, a);
        }
        return colorData;
    }
    public LayerMask hittable;
    Vector GetVision(Vector2 direction, float visionDst = 5f, float agentRadius=0.1f)
    {
        direction = direction.normalized;
        Vector visionData = new Vector(6);
        Vector2 startingPoint = Vector2.Lerp(transform.position, (Vector2)transform.position + direction, agentRadius);
        Vector2 endPoint = (Vector2)transform.position + direction * visionDst;
        RaycastHit2D hit = Physics2D.Linecast(startingPoint, endPoint, hittable);
        if(hit.transform != null)
        {
            visionData.SetValue(0, Vector2.Distance(startingPoint, hit.point) / visionDst);
            
            if(hit.transform.GetComponent<Food>() != null)
            {
                visionData.SetValue(4, 1.0f);
            }
            if(hit.transform.GetComponent<DynamicNeuralController>() != null)
            {
                visionData.SetValue(1, hit.transform.GetComponent<SpriteRenderer>().color.r);
                visionData.SetValue(2, hit.transform.GetComponent<SpriteRenderer>().color.g);
                visionData.SetValue(3, hit.transform.GetComponent<SpriteRenderer>().color.b);
                visionData.SetValue(5, 1.0f);
            }
            Debug.DrawLine(startingPoint, hit.point);
        }
        else
        {
            Debug.DrawLine(startingPoint, endPoint);
        }
        return visionData;       
    }

    Vector2 GetOwnPosition()
    {
        return transform.position.normalized;
    }
    Vector2 GetAngle()
    {
        return (rotationOrgan.position - transform.position).normalized;
    }
    Vector2 prevPos;
    Vector2 GetVelocityVector()
    {
        /*Vector2 vel = new Vector2(0,0);
        if(prevPos != (Vector2)transform.position)
        {
			vel = (Vector2)transform.position - prevPos;
            prevPos = transform.position;
        }vel.normalized*/

        return rb.velocity;

    }
	float GetEnergy()
	{
		return energy / 30f;
	}
    void ScreenClamp()
    {
        Vector3 pos = transform.position;
        pos.y = MathOperations.RepeatingRange(pos.y, -Camera.main.orthographicSize + boundaryRadius, Camera.main.orthographicSize - boundaryRadius);
        //pos.y = Mathf.Clamp(pos.y, -Camera.main.orthographicSize + boundaryRadius, Camera.main.orthographicSize - boundaryRadius);
        float screenRatio = Screen.width / (float)Screen.height;
        pos.x = MathOperations.RepeatingRange(pos.x, (-Camera.main.orthographicSize * screenRatio) + boundaryRadius, (Camera.main.orthographicSize * screenRatio) - boundaryRadius);
        //pos.x = Mathf.Clamp(pos.x, (-Camera.main.orthographicSize * screenRatio) + boundaryRadius, (Camera.main.orthographicSize * screenRatio) - boundaryRadius);
        transform.position = pos;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(output != null)
        {
            if (other.transform.GetComponent<DynamicNeuralController>() != null && output.values[15] > 0.5f)
            {
                partner = other.transform.GetComponent<DynamicNeuralController>();
            }
        }
       
    }

}
