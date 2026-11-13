using System.Data;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using Utility;

public class SaveDataContainer : Singleton<SaveDataContainer>
{
    public SaveData CurrentData => Datas.ElementAtOrDefault(_slotIdx);
    public SaveData[] Datas { get; private set; } = new SaveData[3];

    private int _slotIdx = -1;

    string GetKey(int slot)
    {
        return PlayerPrefs.GetString($"SaveData_{Mathf.Clamp(slot, 0, Datas.Length - 1)}");
    }

    public void LoadOrNewCurrentData(int slot, bool newGame)
    {
        if (slot >=0 || slot < Datas.Length)
        {
            var json = GetKey(slot);
            if (string.IsNullOrEmpty(json) || newGame)
                Datas[slot] = SaveData.New();
            else
                Datas[slot] =  JsonUtility.FromJson<SaveData>(json);
        }
        _slotIdx = slot;
    }

    public void SaveAndUnloadCurrentData()
    {
        if (_slotIdx >=0 || _slotIdx < Datas.Length)
        {
            var json = JsonUtility.ToJson(Datas[_slotIdx]);
            PlayerPrefs.SetString(GetKey(_slotIdx), json);
        }
        _slotIdx = -1;
    }
}
