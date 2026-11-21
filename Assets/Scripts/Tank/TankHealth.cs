using System;
using UnityEngine;

[RequireComponent(typeof(TankState))]
public class TankHealth : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private int maxHp = 25;
    [SerializeField] private float tickInterval = 1f;

    [SerializeField] private int perfectHeal = 1;
    [SerializeField] private int mildTempDamage = 1;
    [SerializeField] private int severeTempDamage = 2; 
    [SerializeField] private int oxygenDamage = 1;
    [SerializeField] private int soundOnDamage = 1;

    [SerializeField] private int hp;
    #endregion

    #region STATE
    private TankState tank;
    private GameProgress progress;
    private float timer;
    private bool lastSoundOn;
    public int Hp => hp;
    public int MaxHp => maxHp;
    public bool IsDead => hp <= 0;

    public event Action<TankHealth> HpChanged;
    public event Action<TankHealth> Died;
    #endregion

    private void Awake()
    {
        tank = GetComponent<TankState>();
        progress = FindFirstObjectByType<GameProgress>();
        hp = maxHp;
        lastSoundOn = tank.SoundOn;
    }

    private void Update()
    {
        if (IsDead || !IsRunning()) return;

        timer += Time.deltaTime;
        while (timer >= tickInterval)
        {
            timer -= tickInterval;
            Tick();
            if (IsDead) return;
        }
    }

    private void OnEnable() => tank.Changed += OnTankChanged;
    private void OnDisable() => tank.Changed -= OnTankChanged;

    #region HEALTH
    private bool IsRunning()
    {
        if (progress == null) return true;
        return progress.State == GameProgress.GameProgressState.Started;
    }

    private void OnTankChanged(TankState state)
    {
        bool turnedOn = state.SoundOn && !lastSoundOn;
        lastSoundOn = state.SoundOn;

        if (turnedOn && !IsDead && IsRunning())
            ApplyDelta(-soundOnDamage);
    }

    private void Tick() => ApplyDelta(GetDeltaPerTick());

    private void ApplyDelta(int delta)
    {
        int newHp = Mathf.Clamp(hp + delta, 0, maxHp);
        if (newHp == hp) return;

        hp = newHp;
        HpChanged?.Invoke(this);

        if (hp <= 0) Died?.Invoke(this);
    }

    public int GetDeltaPerTick()
    {
        int damage = 0;

        switch (tank.Temperature)
        {
            case TemperatureLevel.Warm:
            case TemperatureLevel.Cold:
                damage += mildTempDamage;
                break;
            case TemperatureLevel.Hot:
            case TemperatureLevel.VeryCold:
                damage += severeTempDamage;
                break;
        }

        if (tank.Oxygen != OxygenLevel.Air) damage += oxygenDamage;

        if (damage > 0) return -damage;
        if (tank.SoundOn) return 0; 
        return perfectHeal;
    }

    public void ResetHealth()
    {
        hp = maxHp;
        timer = 0f;
        HpChanged?.Invoke(this);
    }
    #endregion
}
