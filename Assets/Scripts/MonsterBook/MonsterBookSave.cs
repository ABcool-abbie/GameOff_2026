using System.Collections.Generic;

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
}
