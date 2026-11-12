using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProgressConfig")]
public class GameProgressConfig : ScriptableObject
{
    [Serializable]
    public struct DayConfig
    {
        public float TimeOfDay;
    }

    public DayConfig[] dayConfigs;
}
