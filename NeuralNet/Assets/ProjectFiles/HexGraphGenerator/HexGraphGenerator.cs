using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;

public class HexGraphGenerator : MonoBehaviour
{
    public int[] pokemonIVs = new int[6];
    public float hexSize = 5f;
    public float iconDistance = 0.5f;
    float stepSize;
    List<Vector3> hexPoints;
    List<Vector3> graphPoints;
    bool started;
    const int min = 0, max = 31;
	void Start ()
    {
        InitHex();       
	}
    void InitHex()
    {
        stepSize = 360 / (float)6;
        hexPoints = GetHexPoints();
        graphPoints = GetGraphPoints();
        Debug.Log(graphPoints.Count);
        started = true;
    }
    float zRot = 0;
    List<Vector3> GetHexPoints()
    {
        List<Vector3> returnables = new List<Vector3>();
        for(int i = 0; i < 6; i++)
        {
            transform.rotation = Quaternion.Euler(0, 0, zRot);
            returnables.Add(transform.up * hexSize);
            zRot += stepSize;
        }

        return returnables;
    }
    List<Vector3> GetGraphPoints()
    {
        List<Vector3> returnables = new List<Vector3>();
        for(int i = 0; i < pokemonIVs.Length; i++)
        {
            float lerpValue = (float)pokemonIVs[i] / (float)max;
            Debug.Log(Vector3.Lerp(transform.position, hexPoints[i], lerpValue) + " lerpvalue = " + lerpValue);
            returnables.Add(Vector3.Lerp(transform.position, hexPoints[i], lerpValue));
        }
        
        return returnables;
    }
    public string[] iconTags = new string[6];
    void DrawHexWithSize(float size, Color gizmoColor)
    {
        Gizmos.color = gizmoColor;
        for(int i = 0; i < 6; i++)
        {
            if (i < hexPoints.Count - 1)
            {
                Gizmos.DrawLine(Vector3.Lerp(transform.position, hexPoints[i], size), Vector3.Lerp(transform.position, hexPoints[i + 1], size));
            }
            else
            {
                Gizmos.DrawLine(Vector3.Lerp(transform.position, hexPoints[i], size), Vector3.Lerp(transform.position, hexPoints[0], size));
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (started)
        {
            for(int i = 0; i < hexPoints.Count; i++)
            {

                Gizmos.DrawIcon(hexPoints[i] + (hexPoints[i].normalized * iconDistance), iconTags[i], true);
                Gizmos.DrawWireSphere(hexPoints[i], 0.05f);
                if(i < hexPoints.Count - 1)
                {
                    Gizmos.DrawLine(hexPoints[i], hexPoints[i+1]);                   
                }
                else
                {
                    Gizmos.DrawLine(hexPoints[i], hexPoints[0]);
                }

            }
            Color c = Color.green;
            c.a = 0.2f;
            Gizmos.color = c;

            for(int i = 0; i < 6; i++)
            {
                Gizmos.DrawLine(transform.position, hexPoints[i]);
            }
            for(int sideStep = 0; sideStep < 20; sideStep++)
            {
                DrawHexWithSize(sideStep / 19f, c);
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < graphPoints.Count; i++)
            {
                Gizmos.DrawWireSphere(graphPoints[i], 0.05f);
                if (i < graphPoints.Count - 1)
                {
                    Gizmos.DrawLine(graphPoints[i], graphPoints[i + 1]);
                }
                else
                {
                    Gizmos.DrawLine(graphPoints[i], graphPoints[0]);
                }

            }

        }
    }
}
