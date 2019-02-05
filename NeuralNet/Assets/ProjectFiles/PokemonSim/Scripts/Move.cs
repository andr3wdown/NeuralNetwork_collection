using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Move : ScriptableObject
{
    public float damage;
    public MoveType moveType;
    public Element moveElement;
    public abstract void Activate();
}
public enum MoveType
{
    physical,
    special
}
public enum Element
{
    fire,
    water,
    nature
}
