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
        [Range(0f, 1f)]
        public Vector2 ratioRange;
        public RoomType roomType;
        public MonsterConfig[] monsterConfigs;
    }

    [Serializable]
    public struct DayConfig
    {
        public float TimeOfDay;
        public int EventCount;
        public MonsterConfig[] monsterConfigs;
        public DialogConfig DialogConfig;
    }

    public int SanInit = 6;
    public DayConfig[] dayConfigs;
    public RoomConfig[] roomConfigs;
}
