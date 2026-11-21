using System;
using UnityEngine;

public class TankFishDead : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private GameProgress progress;
    [SerializeField] private TankHealth[] tanks;
    #endregion


    private void OnEnable()
    {
        foreach (TankHealth t in tanks)
        {
            if (t != null) t.Died += OnTankDied;
        }
    }

    private void OnDisable()
    {
        foreach (TankHealth t in tanks)
        {
            if (t != null) t.Died -= OnTankDied;
        }
    }

    #region BE
    private void OnTankDied(TankHealth tank)
    {
        Debug.Log($"{tank.name} fish died!");

        if (progress == null) return;

        var source = progress.EventSource as GameEventSource;
        if (source == null) return;

        source.WorkFault();
    }
    #endregion
}
