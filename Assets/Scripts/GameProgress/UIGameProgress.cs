using System;
using System.Net;
using System.Xml.Schema;
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

    internal class Debug : Singleton<Debug>, IGameProgressEventSource
    {
        public event Action OnWorkFault;
        public event Action OnSanDrop;

        internal void SanDrop()
        {
            OnSanDrop?.Invoke();
        }

        internal void WorkFault()
        {
            OnWorkFault?.Invoke();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameProgress.OnDayFinished += OnDayFinished;
        gameProgress.OnTimeTicked += RefreshUI;
        gameProgress.OnSanChanged += RefreshUI;
        gameProgress.OnStateChanged += RefreshUI;

        var data = SaveDataContainer.Instance.CurrentData;
        if (data == null)
            SaveDataContainer.Instance.LoadOrNewCurrentData(0, true);
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
        txtDay.text = (gameProgress.TotalDay - gameProgress.DayLeft + 1).ToString();
        txtTime.text = Mathf.CeilToInt(gameProgress.TimeLeft).ToString();
        txtSan.text = gameProgress.CurrentSan.ToString();
        txtState.text = gameProgress.State.ToString();
    }

    private void OnDayFinished()
    {
    }

    void OnGUI()
    {
        if (GUILayout.Button("Drop San"))
        {
            Debug.Instance?.SanDrop();
        }
        if (GUILayout.Button("Work Fault"))
        {
            Debug.Instance?.WorkFault();
        }
        if (GUILayout.Button("Time - 10s"))
        {
            gameProgress._debugTimeMinus(10f);
        }
    }
}
