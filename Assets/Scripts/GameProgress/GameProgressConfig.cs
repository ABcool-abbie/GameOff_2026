using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProgressConfig")]
public class GameProgressConfig : ScriptableObject
{
    public enum RoomType
    {
        Arcuate, Middle, Undercurrent, DeepSea
    }

    [Serializable]
    public struct RoomConfig
    {
        public RoomType roomType;
        public MonsterConfig[] monsterConfigs;
    }

    [Serializable]
    public struct DayConfig
    {
        public float TimeOfDay;
        public int abnormalCount;
        // public Vector2 abnormalRange;
        public Vector2 wonderSecondRange;
        public MonsterConfig[] monsterConfigs;
        public DialogConfig DialogConfig;
    }

    public int SanInit = 6;
    public DayConfig[] dayConfigs;
    public RoomConfig[] roomConfigs;
}
