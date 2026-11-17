using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProgressConfig")]
public class GameProgressConfig : ScriptableObject
{
    [Serializable]
    public struct DayConfig
    {
        public float TimeOfDay;
        public int EventCount;
        public int MonisterCount;
        public int[] MonisterTypes;
        public DialogConfig DialogConfig;
    }

    public int SanInit = 6;
    public DayConfig[] dayConfigs;
}
