using UnityEngine;

public class TankStartRamdom : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private TankState[] tanks;
    [SerializeField] int firstRandomDay = 1;
    #endregion

    private void Start()
    {
        SaveData data = SaveDataContainer.Instance.CurrentData;
        if (data == null) return;
        if (data.day < firstRandomDay) return;

        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i]  == null) continue;
            tanks[i].RandomStart();
        }
    }
}
