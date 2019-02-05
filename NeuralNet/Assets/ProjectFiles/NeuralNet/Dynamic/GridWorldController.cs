using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridWorldController : MonoBehaviour
{
    public Vector2 GridWorldSize;
    public float gridElementSize = 0.5f;
    List<Vector2> gridPoints = new List<Vector2>();
    //Food currentFood;
    static GridWorldController instance;
    public GameObject foodObject;
    public GameObject creatures;
    public Text text;
    public int foodMin = 20;
    public int[] netLayerSizes = { 13, 20, 20, 7 };
    private void Start()
    {
        instance = this;
        int width = Mathf.RoundToInt(GridWorldSize.x / gridElementSize);
        int height = Mathf.RoundToInt(GridWorldSize.y / gridElementSize);
        Debug.Log("w" + width + "h" + height);
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPoints.Add(new Vector2((x - (width / 2f - 0.5f)) / 2, (y - (height / 2f - 0.5f)) / 2));
            }
        }
        SpawnRandomizedGeneration();
    }
    void SpawnRandomizedGeneration(int popSize = 30)
    {
        for(int i = 0; i < popSize; i++)
        {
            int index = Random.Range(0, gridPoints.Count);
            GameObject go = Instantiate(creatures, gridPoints[index], transform.rotation);
            go.GetComponent<DynamicNeuralController>().InitNet(netLayerSizes);
        }
    }
 
    private void Update()
    {
        if(Food.allFood.Count < foodMin)
        {
            int index = Random.Range(0, gridPoints.Count);
            GameObject go = Instantiate(foodObject, gridPoints[index], transform.rotation);      
        }
        if(DynamicNeuralController.allControllers.Count < 30)
        {
            SpawnRandomizedGeneration(1);
        }
        text.text = "" + DynamicNeuralController.allControllers.Count;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, 1));
        Color c = Color.green;
        c.a = 0.05f;
        Gizmos.color = c;
        if(gridPoints.Count > 0)
        {
            foreach (Vector3 v in gridPoints)
            {
                Gizmos.DrawWireCube(v, Vector3.one * gridElementSize);
            }
        }
      
    }
}
 