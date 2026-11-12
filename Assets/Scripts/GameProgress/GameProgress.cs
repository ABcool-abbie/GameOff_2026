using System;
using UnityEngine;

public interface IGameProgressEventSource
{
    public event Action OnWorkFault;
    public event Action OnSanDrop;
}

public class GameProgress : MonoBehaviour
{
    public event Action OnDayChanged;
    public event Action OnTimeTicked;
    public event Action OnSanChanged;
    public event Action OnStateChanged;

    public enum GameProgressState
    {
        Prepare,
        Started,

        Paused,

        _Result = 100,
        ResultSuccess,
        ResultRanOutOfSan,
        ResultDeadOfFish,
    }

    public GameProgressState State {get; private set;} = GameProgressState.Prepare;
    public bool IsGameOver => State >= GameProgressState._Result;

    public GameProgressConfig Config => config;
    [SerializeField] private GameProgressConfig config;

    public int TotalDay => config?.dayConfigs?.Length ?? 0;
    public int DayLeft {get; private set;}

    public float TimePerDay = 60f;
    public float TimeLeft {get; private set;}

    public int TotalSan = 6;
    public int CurrentSan {get; private set;}

    private IGameProgressEventSource _eventSource;

    void OnDestroy()
    {
        if (_eventSource != null)
        {
            _eventSource.OnWorkFault -= OnWorkFault;
            _eventSource.OnSanDrop -= OnSanDrop;
        }
    }

    void Update()
    {
        if (IsGameOver || State == GameProgressState.Paused)
            return;

        if (TimeLeft > 0f)
        {
            float oldTime = TimeLeft;
            TimeLeft -= Time.deltaTime;

            if (Mathf.FloorToInt(oldTime) != Mathf.FloorToInt(TimeLeft))
                OnTimeTicked?.Invoke();

            if (TimeLeft <= 0f)
            {
                if (DayLeft > 0)
                {
                    DayLeft -= 1;
                    TimeLeft += TimePerDay;
                    if (DayLeft <= 0)
                    {
                        OnGameOver(GameProgressState.ResultSuccess);
                    }
                    else
                    {
                        NewDay(TotalDay - DayLeft);
                    }
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

    public void StartGame()
    {
        CurrentSan = TotalSan;
        DayLeft = TotalDay;
        State = GameProgressState.Started;
        NewDay(0);
    }

    public void Continue()
    {
        if (State == GameProgressState.Paused)
            State = GameProgressState.Started;
    }

    void NewDay(int day)
    {
        if (day < 0 || day >= config.dayConfigs.Length)
            return;

        var dayConfig = config.dayConfigs[day];
        TimePerDay = dayConfig.TimeOfDay;
        TimeLeft = dayConfig.TimeOfDay;

        State = GameProgressState.Paused;
        OnDayChanged?.Invoke();
    }

    void OnWorkFault()
    {
        OnGameOver(GameProgressState.ResultDeadOfFish);
    }

    void OnSanDrop()
    {
        if (CurrentSan <= 0)
            return;
        CurrentSan -= 1;
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
}
