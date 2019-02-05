using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SegmentedCreatureBuilder))]
public class SCBEditor : Editor
{
    SegmentedCreatureBuilder scb;
    private void OnEnable()
    {
        scb = (SegmentedCreatureBuilder)target;
    }
    public override void OnInspectorGUI()
    {
        DisplayLabel();
        base.OnInspectorGUI();
        if (GUILayout.Button("GenerateCreature"))
        {
            scb.Generate();
        }
        if (GUILayout.Button("EditMode"))
        {
            scb.editMode = !scb.editMode;
        }
    }
    void DisplayLabel()
    {
        string mode = "";
        if (scb.editMode)
        {
            mode = "Edit Mode";
        }
        else
        {
            mode = "Idle Mode";
        }
        GUILayout.TextField(mode);
    }

}
