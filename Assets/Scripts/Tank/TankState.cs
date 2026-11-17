using System;
using UnityEngine;

public enum TemperatureLevel { VeryCold = -2, Cold = -1, Normal = 0, Warm = 1, Hot = 2 }
public enum OxygenLevel { Deoxygenated = -1, Air = 0, HighOxygen = 1 }

public class TankState : MonoBehaviour
{
    #region SETTING
    [SerializeField] private TemperatureLevel temperature = TemperatureLevel.Normal;
    [SerializeField] private OxygenLevel oxygen = OxygenLevel.Air;
    [SerializeField] private bool lightOn = true;
    [SerializeField] private bool soundOn = false;
    #endregion

    #region STATS
    public TemperatureLevel Temperature => temperature;
    public OxygenLevel Oxygen => oxygen;
    public bool LightOn => lightOn;
    public bool SoundOn => soundOn;
    public float LastFedTime { get; private set; } = -1f;
    #endregion
    public event Action<TankState> Changed;
    public event Action<TankState> Fed;

    public void SetTemperature(TemperatureLevel value)
    {
        if (temperature == value) return;
        temperature = value;
        Changed?.Invoke(this);
    }

    public void SetOxygen(OxygenLevel value)
    {
        if (oxygen == value) return;
        oxygen = value;
        Changed?.Invoke(this);
    }

    public void SetLight(bool on)
    {
        if (lightOn == on) return;
        lightOn = on;
        Changed?.Invoke(this);
    }

    public void SetSound(bool on)
    {
        if (soundOn == on) return;
        soundOn = on;
        Changed?.Invoke(this);
    }

    public void Feed()
    {
        LastFedTime = Time.time;
        Fed?.Invoke(this);
    }

}
