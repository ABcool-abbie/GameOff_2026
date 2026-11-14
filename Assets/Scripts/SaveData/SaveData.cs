using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    public struct DayRecord
    {
        public List<int> monsiterTypes;
        public List<int> monisterHitCounts;
        public List<int> monisterSolveCounts;
    }

    public int day;
    public int san;
    public List<DayRecord> records;

    internal static SaveData New(GameProgressConfig config)
    {
        return new SaveData
        {
            day = 0,
            san = config.SanInit,
            records = new(),
        };
    }
}
