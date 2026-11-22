using System;
using UnityEngine;

public enum TemperatureLevel { VeryCold = -2, Cold = -1, Normal = 0, Warm = 1, Hot = 2 }
public enum OxygenLevel { Deoxygenated = -1, Air = 0, HighOxygen = 1 }

public class TankState : MonoBehaviour
{
    #region SETTING
    [SerializeField] private TemperatureLevel temperature = TemperatureLevel.Normal;
    [SerializeField] private OxygenLevel oxygen = OxygenLevel.Air;
    [SerializeField] private bool lightOn = false;
    [SerializeField] private bool soundOn = false;
    #endregion

    #region STATS
    public TemperatureLevel Temperature => temperature;
    public OxygenLevel Oxygen => oxygen;
    public bool LightOn => lightOn;
    public bool SoundOn => soundOn;
    public float LastFedTime { get; private set; } = -1f;
    public bool Viewing {get; private set;}

    public event Action<TankState> Changed;
    public event Action<TankState> Fed;
    public event Action<TankState> NightReset;
    #endregion

    #region START VALUES
    private TemperatureLevel startTemperature;
    private OxygenLevel startOxygen;
    private bool startLightOn;
    private bool startSoundOn;
    #endregion

    [SerializeField] Vector4 boundsMinMax;
    public Vector2 BoundsMin => transform.position + new Vector3(boundsMinMax.x, boundsMinMax.y);
    public Vector2 BoundsMax => transform.position + new Vector3(boundsMinMax.z, boundsMinMax.w);

    private void Awake()
    {
        startTemperature = temperature;
        startOxygen = oxygen;
        startLightOn = lightOn;
        startSoundOn = soundOn;
    }

    #region CONTROLS
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
        Changed?.Invoke(this);
    }

    public void View(bool on)
    {
        if (Viewing == on) return;
        Viewing = on;
        Debug.Log($"{name} viewing: {on}");
        Changed?.Invoke(this);
    }
    #endregion

    #region NEW NIGHT
    public void ResetForNewNight()
    {
        temperature = startTemperature;
        oxygen = startOxygen;
        lightOn = startLightOn;
        soundOn = startSoundOn;
        LastFedTime = -1f;

        NightReset?.Invoke(this);
        Changed?.Invoke(this);
    }

    #endregion

    void OnDrawGizmos()
    {
        var old = Gizmos.color;
        Gizmos.color = Color.greenYellow;
        Vector2 min = BoundsMin;
        Vector2 max = BoundsMax;
        Gizmos.DrawLine(min, new Vector3(min.x, max.y));
        Gizmos.DrawLine(min, new Vector3(max.x, min.y));
        Gizmos.DrawLine(max, new Vector3(min.x, max.y));
        Gizmos.DrawLine(max, new Vector3(max.x, min.y));
        Gizmos.color = old;
    }
}
