using System;
using System.Collections.Generic;
using UnityEngine;

public enum TemperatureLevel { VeryCold = -2, Cold = -1, Normal = 0, Warm = 1, Hot = 2 }
public enum OxygenLevel { Deoxygenated = -1, Air = 0, HighOxygen = 1 }

public class TankState : MonoBehaviour
{
    public event Action<int, int> OnMonsterCountChanged;

    #region SETTING
    [SerializeField] private TemperatureLevel temperature = TemperatureLevel.Normal;
    [SerializeField] private OxygenLevel oxygen = OxygenLevel.Air;
    [SerializeField] private bool lightOn = false;
    [SerializeField] private bool soundOn = false;
    #endregion

    #region STATE
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

    [SerializeField] TankEventConfigs eventConfigs;

    private int _monsterCount = 0;
    public int MonsterCount { 
        get => _monsterCount; 
        internal set
        {
            if (_monsterCount == value)
                return;

            var old = _monsterCount;
            _monsterCount = value;
            OnMonsterCountChanged?.Invoke(old, value);
        } 
    }

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

    public void RandomStart()
    {
        temperature = (TemperatureLevel)UnityEngine.Random.Range(-2, 3);
        oxygen = (OxygenLevel)UnityEngine.Random.Range(-1, 2);
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

    Dictionary<(MonsterConfig, int), GameObject> _behaviourInstances = new();
    internal void SetBehaviour(MonsterConfig monsterConfig, int idxBehaviour, bool isOn)
    {
        if (isOn)
        {
            foreach(var config in eventConfigs.Configs)
            {
                if (config.monsterConfig == monsterConfig && config.idxBehaviour == idxBehaviour)
                {
                    var instance = GameObject.Instantiate(config.behaviourPrefab, transform, false);
                    _behaviourInstances.Add((monsterConfig, idxBehaviour), instance);
                    break;
                }
            }
        }
        else
        {
            if (_behaviourInstances.TryGetValue((monsterConfig, idxBehaviour), out var insatnce))
            {
                _behaviourInstances.Remove((monsterConfig, idxBehaviour));

                if (insatnce.gameObject.GetComponent<ParticleSystem>() is ParticleSystem particleSystem)
                {
                    particleSystem.transform.SetParent(null, true);
                    UnityHelper.CallbackOnParticleSystemStop(particleSystem, instance => GameObject.Destroy(instance.gameObject));
                }
                else
                {
                    GameObject.Destroy(insatnce.gameObject);
                }
            }
        }
    }
}
