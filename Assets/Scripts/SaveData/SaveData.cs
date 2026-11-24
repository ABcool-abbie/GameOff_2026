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

    //BBR: monster book unlocks
    [Serializable]
    public struct BookUnlock
    {
        public int behaviour; 
        public int mask; 
    }
    //BBR: One entry per event

    public int day;
    public int san;
    public List<DayRecord> records;
    public List<BookUnlock> bookUnlocks;

    public GameProgress.GameProgressState Result { get; internal set; }

    internal static SaveData New(GameProgressConfig config)
    {
        return new SaveData
        {
            day = 0,
            san = config.SanInit,
            records = new(),
            bookUnlocks = new(),
        };
    }
}
