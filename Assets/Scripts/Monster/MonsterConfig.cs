using System;
using UnityEngine;
using UnityEngine.U2D.IK;

public enum EventType
{
    Temp,
    Light,
    Feed,
    Music,
    Oxygen,
    View,
}

public enum Behaviour
{
    Bubble = 10, Glared,    Pollution,
    Season = 20, Molting,   Shadow,
    Stare  = 30, Tempt,     Foam,
    Flash  = 40, Piapia,    FishingNet,
    Sing   = 50, Countdown, Sharpening,
    Interf = 60, Pattern,   Toxin,
    Ink    = 70, Knocking,  PullTheCircuit,
}

public enum Gradient
{
    Undefine, Down, Up
}

[Serializable]
public struct EventConfig
{
    public EventType type;

    /// <summary>
    // Temp: 0 - 4 low temperature to high temperature
    // Light: 0 Off, 1 on
    // Feed: 0 Off, 1 on
    // Music: 0 Off, 1 on
    // Oxygen: 0 - 2 zero oxygen to high oxygen
    // View: 0 off screen, 1 on screen
    /// </summary>
    public int param0; 
}

[Serializable]
public struct BehaviourConfig
{
    public Behaviour behaviour;
    public int damage;
    public EventConfig[] solveMethod; 
}

[CreateAssetMenu(fileName = "MonsterConfig")]
public class MonsterConfig : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprThumbnail;
    public Sprite sprDetail;
    public string code;
    public string description;
    // public EventConfig[] events;
    public BehaviourConfig[] behaviours;
}
