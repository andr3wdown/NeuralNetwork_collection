using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedCreature : MonoBehaviour
{
    public List<GameObject> nodes = new List<GameObject>();
    void Init(List<GameObject> _nodes)
    {

        nodes = _nodes;
    }
    private void Update()
    {
        if(nodes.Count > 1)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                LineRenderer lr = nodes[i].GetComponent<LineRenderer>();
                lr.SetPosition(0, nodes[i].transform.position);
                lr.SetPosition(1, nodes[i].GetComponent<HingeJoint2D>().connectedBody.transform.position);
            }
        }
    }


    private void OnDrawGizmos()
    {
        if(nodes != null && nodes.Count > 1)
        foreach(GameObject g in nodes)
        {
            Debug.DrawLine(g.GetComponent<HingeJoint2D>().connectedBody.transform.position, g.transform.position);
        }
    }
}
