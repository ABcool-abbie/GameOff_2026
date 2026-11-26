using UnityEngine;

public class MonsterBookUnlocker : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private int linesPerEvent = 2;
    [SerializeField] private bool saveMonsterEntry = true;
    #endregion
    private void OnEnable() => Monster.Encountered += OnEncountered;
    private void OnDisable() => Monster.Encountered -= OnEncountered;

    #region UNLOCK
    private void OnEncountered(Monster monster)
    {
        SaveData data = SaveDataContainer.Instance.CurrentData;
        if (data == null) return;

        int key = (int)monster.BehaviourType;
        int line = MonsterBookSave.UnlockRandomLine(data, key, linesPerEvent);
        if (line < 0) return; 


        if (saveMonsterEntry) SaveDataContainer.Instance.SaveCurrentData();
    }
    #endregion
}
