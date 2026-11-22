using System;
using UnityEngine;

[Serializable]
public struct BookEventEntry
{
    public Behaviour behaviour;
    public Sprite icon;
    [TextArea(2, 4)] public string appearance;
    [TextArea(1, 3)] public string[] solutionLines;
}

[CreateAssetMenu(fileName = "MonsterBookEntry", menuName = "Game/Monster Book Entry")]
public class MonsterBookEntry : ScriptableObject
{
    public MonsterConfig monster;
    [Range(1, 5)] public int danger = 1;
    [TextArea(3, 6)] public string background;
    public BookEventEntry[] events; 



}
