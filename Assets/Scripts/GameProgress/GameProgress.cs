using System;
using UnityEngine;
using Utility;

public interface IGameProgressEventSource
{
    public event Action OnWorkFault;
    public event Action OnSanDrop;
}

public class GameProgress : MonoBehaviour
{
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
    }

    public GameProgressState State {get; private set;} = GameProgressState.Prepare;
    public bool IsGameOver => State >= GameProgressState._Result;

    public int TotalDay => GameConfigContainer.Instance.Config?.dayConfigs?.Length ?? 0;
    public int DayLeft {get; private set;}

    public float TimePerDay = 60f;
    public float TimeLeft {get; private set;}

    public int TotalSan = 6;
    public int CurrentSan {get; private set;}

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

            if (Mathf.FloorToInt(oldTime) != Mathf.FloorToInt(TimeLeft))
                OnTimeTicked?.Invoke();

            if (TimeLeft <= 0f)
            {
                TimeLeft += TimePerDay;
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
        TimePerDay = dayConfig.TimeOfDay;
        TimeLeft = dayConfig.TimeOfDay;

        State = GameProgressState.Started;
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
