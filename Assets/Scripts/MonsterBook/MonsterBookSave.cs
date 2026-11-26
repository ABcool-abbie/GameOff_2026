using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MonsterBookSave
{
    public static int GetMask(SaveData data, int behaviour)
    {
        if (data == null || data.bookUnlocks == null) return 0;

        foreach (SaveData.BookUnlock unlock in data.bookUnlocks)
        {
            if (unlock.behaviour == behaviour) return unlock.mask;
        }
        return 0;
    }

    public static void SetMask(SaveData data, int behaviour, int mask)
    {
        if (data == null) return;
        if (data.bookUnlocks == null) data.bookUnlocks = new List<SaveData.BookUnlock>();

        for (int i = 0; i < data.bookUnlocks.Count; i++)
        {
            if (data.bookUnlocks[i].behaviour != behaviour) continue;

            SaveData.BookUnlock unlock = data.bookUnlocks[i];
            unlock.mask = mask;
            data.bookUnlocks[i] = unlock;
            return;
        }

        data.bookUnlocks.Add(new SaveData.BookUnlock { behaviour = behaviour, mask = mask });
    }

    public static int UnlockRandomLine(SaveData data, int behaviour, int lineCount)
    {
        if (data == null) return -1;

        int mask = GetMask(data, behaviour);

        List<int> locked = new List<int>();
        for (int i = 0; i < lineCount; i++)
        {
            bool unlocked = (mask & (1 << i)) != 0;
            //if (unlocked) locked.Add(i);
            if (!unlocked) locked.Add(i);
        }

        if (locked.Count == 0) return -1;

        int pick = locked[Random.Range(0, locked.Count)];

        mask |= (1 << pick);
        SetMask(data, behaviour, mask);

        return pick;
    }
}
    
