using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    public struct DayRecord
    {
        public int[] monsiterTypes;
        public int[] monisterHitCounts;
        public int[] monisterSolveCounts;
    }

    public int day;
    public int san;
    public DayRecord[] records;

    internal static SaveData New()
    {
        throw new NotImplementedException();
    }
}
