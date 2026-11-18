using System;
using UnityEngine;

public enum EventType
{
    Temp,
    Disinfection,
    Feed,
    Music,
    Oxygen,
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

[Serializable]
public struct EventConfig
{
    public EventType type;
    public int param0;
}

[Serializable]
public struct BehaviourConfig
{
    public Behaviour behaviour;
    public int damage;
}

[CreateAssetMenu(fileName = "MonsterConfig")]
public class MonsterConfig : ScriptableObject
{
    public Sprite sprThumbnail;
    public Sprite sprDetail;
    public string code;
    public string description;
    public EventConfig[] events;
    public Behaviour[] behaviour;
    public BehaviourConfig[] behaviours;
}
