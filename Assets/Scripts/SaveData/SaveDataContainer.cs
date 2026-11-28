using System;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

public class SaveDataContainer : Singleton<SaveDataContainer>
{
    public SaveData CurrentData => Datas.ElementAtOrDefault(_slotIdx);
    public SaveData[] Datas { get; private set; } = new SaveData[3];

    private int _slotIdx {
        get; 
        set;} = -1;

    string GetKey(int slot)
    {
        return slot < 0 ? null : $"SaveData_{Mathf.Clamp(slot, 0, Datas.Length - 1)}";
    }

    public SaveData LoadOrNewCurrentData(int slot, bool newGame)
    {
        if (slot >=0 || slot < Datas.Length)
        {
            var key = GetKey(slot);
            var json = key != null ? PlayerPrefs.GetString(key) : null;
            if (string.IsNullOrEmpty(json) || newGame)
                Datas[slot] = SaveData.New(GameConfigContainer.Instance.Config);
            else
                Datas[slot] =  JsonUtility.FromJson<SaveData>(json);
        }
        _slotIdx = slot;
        return Datas[_slotIdx];
    }

    public void SaveCurrentData()
    {
        if (_slotIdx >=0 || _slotIdx < Datas.Length)
        {
            var json = JsonUtility.ToJson(Datas[_slotIdx]);
            PlayerPrefs.SetString(GetKey(_slotIdx), json);
        }
    }

    public void SaveAndUnloadCurrentData()
    {
        SaveCurrentData();
        _slotIdx = -1;
    }

    internal bool HasSaveData(int slot)
    {
        var json = PlayerPrefs.GetString(GetKey(slot));
        try{
            if (string.IsNullOrEmpty(json))
                return false;
            return JsonUtility.FromJson<SaveData>(json) != null;
        }
        catch(Exception e)
        {
            return false;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/ClearAllSaveDatas")]
    public static void ClearAllSaveDatas()
    {
        SaveDataContainer.Instance.Datas = new SaveData[3];
        SaveDataContainer.Instance._slotIdx = -1;
        PlayerPrefs.DeleteKey(SaveDataContainer.Instance.GetKey(0));
        PlayerPrefs.DeleteKey(SaveDataContainer.Instance.GetKey(1));
        PlayerPrefs.DeleteKey(SaveDataContainer.Instance.GetKey(2));
    }
#endif
}
