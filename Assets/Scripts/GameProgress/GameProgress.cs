using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.ShaderGraph;
using UnityEngine;

public interface IGameProgressEventSource
{
    public event Action OnWorkFault;
    public event Action<int> OnSanDrop;
}

public class GameEventSource : IGameProgressEventSource
{
    public event Action OnWorkFault;
    public event Action<int> OnSanDrop;

    private bool _isDone = false;

    internal void DropSan(int damage)
    {
        OnSanDrop?.Invoke(damage);
    }

    internal void WorkFault()
    {
        OnWorkFault?.Invoke();
    }
}

public class GameProgress : MonoBehaviour
{
    [Serializable]
    struct AbnormalConfig
    {
        public MonsterConfig monster;
        public float appearTime;
        public float eventTime;
        internal Monster instance;
    }

    public TankState[] tanks;

    public event Action OnDayFinished;
    public event Action OnTimeTicked;
    public event Action OnSanChanged;
    public event Action OnStateChanged;

    public enum GameProgressState
    {
        Prepare,
        Started,
        DayFinished,

        _Result = 100,
        ResultSuccess,
        ResultRanOutOfSan,
        ResultDeadOfFish,
        ResultPlayerIsAte,
    }

    public GameProgressState State { get; private set; } = GameProgressState.Prepare;
    public bool IsGameOver => State >= GameProgressState._Result;

    public int TotalDay => GameConfigContainer.Instance.Config?.dayConfigs?.Length ?? 0;
    public int DayLeft { get; private set; }

    public float TimeOfDay = -1f;
    public float TimeLeft { get; private set; }

    public int TotalSan = 6;
    public int CurrentSan { get; private set; }

    private AbnormalConfig[] abnormalConfigs;

    public IGameProgressEventSource EventSource => _eventSource;
    private IGameProgressEventSource _eventSource;

    void OnDestroy()
    {
        Unregister();
    }

    void Update()
    {
        if (IsGameOver || State == GameProgressState.DayFinished)
            return;

        if (TimeLeft > 0f)
        {
            float oldTime = TimeLeft;
            TimeLeft -= Time.deltaTime;

            TickMonsters(TimeOfDay - TimeLeft);

            if (Mathf.FloorToInt(oldTime) != Mathf.FloorToInt(TimeLeft))
                OnTimeTicked?.Invoke();

            if (TimeLeft <= 0f)
            {
                // TimeLeft += TimePerDay;
                if (DayLeft <= 0)
                {
                    OnGameOver(GameProgressState.ResultSuccess);
                }
                else
                {
                    State = GameProgressState.DayFinished;
                    OnStateChanged?.Invoke();
                    OnDayFinished?.Invoke();
                }
                TimeLeft = Mathf.Max(0f, TimeLeft);
            }
        }
    }

    public void Register(IGameProgressEventSource eventSource)
    {
        _eventSource = eventSource;
        _eventSource.OnWorkFault += OnWorkFault;
        _eventSource.OnSanDrop += OnSanDrop;
    }

    void Unregister()
    {
        if (_eventSource != null)
        {
            _eventSource.OnWorkFault -= OnWorkFault;
            _eventSource.OnSanDrop -= OnSanDrop;
        }
    }

    public void StartGame(SaveData data)
    {
        CurrentSan = data.san;
        Debug.Assert(TotalDay >= data.day);
        DayLeft = TotalDay - data.day;
        State = GameProgressState.Started;

        int day = data.day;
        var config = GameConfigContainer.Instance.Config;
        if (day < 0 || day >= config.dayConfigs.Length)
            return;

        var dayConfig = config.dayConfigs[day];
        TimeOfDay = dayConfig.TimeOfDay;
        TimeLeft = dayConfig.TimeOfDay;

        abnormalConfigs = new AbnormalConfig[dayConfig.abnormalCount];
        var monsterBuffer = (MonsterConfig[])dayConfig.monsterConfigs.Clone();
        for (int i = monsterBuffer.Length - 1; i > 0; --i)
        {
            int rnd = UnityEngine.Random.Range(0, i);
            (monsterBuffer[rnd], monsterBuffer[i]) = (monsterBuffer[i], monsterBuffer[rnd]);
        }
        for (int i = 0; i < dayConfig.abnormalCount; ++i)
        {
            var cfg = new AbnormalConfig();
            float rndRatio = UnityEngine.Random.Range(dayConfig.abnormalRange.x, dayConfig.abnormalRange.y);
            float rndWondering = UnityEngine.Random.Range(dayConfig.wonderSecondRange.x, dayConfig.wonderSecondRange.y);
            cfg.eventTime = rndRatio * dayConfig.TimeOfDay;
            cfg.appearTime = Mathf.Max(0f, cfg.eventTime - rndWondering);
            cfg.monster = monsterBuffer[i];

            abnormalConfigs[i] = cfg;
        }

        State = GameProgressState.Started;
    }

    void TickMonsters(float time)
    {
        for (int i = 0; i < abnormalConfigs.Length; ++i)
        {
            ref var cfg = ref abnormalConfigs[i];
            if (cfg.instance != null)
            {
                cfg.instance.Tick(time, cfg.eventTime);
            }
            else if (time > cfg.appearTime)
            {
                cfg.instance = Monster.Spawn(this, cfg.monster);
            }
        }
    }

    void OnWorkFault()
    {
        OnGameOver(GameProgressState.ResultDeadOfFish);
    }

    void OnSanDrop(int damage)
    {
        if (CurrentSan <= 0)
            return;
        CurrentSan -= damage;
        OnSanChanged?.Invoke();
        if (CurrentSan <= 0)
        {
            OnGameOver(GameProgressState.ResultRanOutOfSan);
        }
    }

    void OnGameOver(GameProgressState state)
    {
        if (IsGameOver)
            return;

        State = state;
        OnStateChanged?.Invoke();
    }

    internal void _debugTimeMinus(float v)
    {
        TimeLeft -= v;
        TimeLeft = Mathf.Max(0.1f, TimeLeft);
    }

    internal TankState GetTank(GameProgressConfig.RoomType roomType)
    {
        int idx = (int)roomType;
        idx = Mathf.Clamp(idx, 0, tanks.Length);
        return tanks[idx];
    }
}
