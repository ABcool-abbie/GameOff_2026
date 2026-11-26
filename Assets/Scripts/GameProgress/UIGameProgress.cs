using System;
using System.Net;
using System.Xml.Schema;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Utility;

public class UIGameProgress : MonoBehaviour
{
    [SerializeField] GameProgress gameProgress;
    [SerializeField] TextMeshProUGUI txtDay;
    [SerializeField] TextMeshProUGUI txtTime;
    [SerializeField] TextMeshProUGUI txtSan;
    [SerializeField] TextMeshProUGUI txtState;

    private GameEventSource src = null;
    // internal class Debug : Singleton<Debug>, IGameProgressEventSource
    // {
    //     public event Action OnWorkFault;
    //     public event Action<int> OnSanDrop;
    //     public event Action OnPlayerIsAte;

    //     internal void SanDrop()
    //     {
    //         OnSanDrop?.Invoke(1);
    //     }

    //     internal void WorkFault()
    //     {
    //         OnWorkFault?.Invoke();
    //     }
    // }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameProgress.OnDayFinished += OnDayFinished;
        gameProgress.OnTimeTicked += RefreshUI;
        gameProgress.OnSanChanged += RefreshUI;
        gameProgress.OnStateChanged += RefreshUI;

        var data = SaveDataContainer.Instance.CurrentData;
        if (data == null)
            data = SaveDataContainer.Instance.LoadOrNewCurrentData(0, true);
        // gameProgress.Register(Debug.Instance);
        src = new GameEventSource();
        gameProgress.Register(src);
        gameProgress.StartGame(data);

        RefreshUI();
    }

    void Oestroy()
    {
        gameProgress.OnDayFinished -= OnDayFinished;
        gameProgress.OnTimeTicked -= RefreshUI;
        gameProgress.OnSanChanged -= RefreshUI;
        gameProgress.OnStateChanged -= RefreshUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RefreshUI()
    {
        if (gameProgress.State >= GameProgress.GameProgressState._Result
            && gameProgress.State < GameProgress.GameProgressState._Finished)
        {
            SaveDataContainer.Instance.CurrentData.Result = gameProgress.State;
            SaveDataContainer.Instance.SaveCurrentData();
            UILoading.Instance.Load(SceneNames.Settlement, null);
            gameProgress.Done();
            return;
        }

        txtDay.text = (gameProgress.Day + 1).ToString();
        txtTime.text = Mathf.CeilToInt(gameProgress.TimeLeft).ToString();
        txtSan.text = gameProgress.CurrentSan.ToString();
        txtState.text = gameProgress.State.ToString();
    }

    private void OnDayFinished()
    {
        if (gameProgress.State == GameProgress.GameProgressState._Finished)
            return;
        SaveDataContainer.Instance.CurrentData.Result = GameProgress.GameProgressState.ResultSuccess;
        SaveDataContainer.Instance.CurrentData.san = gameProgress.CurrentSan;
        SaveDataContainer.Instance.CurrentData.day++;
        SaveDataContainer.Instance.SaveCurrentData();
        UILoading.Instance.Load(SceneNames.Settlement, null);
        gameProgress.Done();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Drop San"))
        {
            src?.DropSan(1);
        }
        if (GUILayout.Button("Work Fault"))
        {
            src?.WorkFault();
        }
        if (GUILayout.Button("Time - 10s"))
        {
            gameProgress._debugTimeMinus(10f);
        }
    }
}
