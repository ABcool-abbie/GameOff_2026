using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class UIGameProgress : MonoBehaviour
{
    [SerializeField] GameProgress gameProgress;

    [SerializeField] TextMeshProUGUI txtDay;
    [SerializeField] TextMeshProUGUI txtTime;
    [SerializeField] TextMeshProUGUI txtSan;
    [SerializeField] TextMeshProUGUI txtState;

    class Debug : IGameProgressEventSource
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
    [SerializeField] bool isDebug = false;
    Debug _debug;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isDebug)
        {
            _debug = new();
            gameProgress.Register(_debug);
            gameProgress.StartGame();
        }

        gameProgress.OnDayChanged += RefreshUI;
        gameProgress.OnTimeTicked += RefreshUI;
        gameProgress.OnSanChanged += RefreshUI;
        gameProgress.OnStateChanged += RefreshUI;
    }

    void Oestroy()
    {
        gameProgress.OnDayChanged -= RefreshUI;
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
        txtDay.text = gameProgress.DayLeft.ToString();
        txtTime.text = Mathf.CeilToInt(gameProgress.TimeLeft).ToString();
        txtSan.text = gameProgress.CurrentSan.ToString();
        txtState.text = gameProgress.State.ToString();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Drop San"))
        {
            _debug?.SanDrop();
        }
        if (GUILayout.Button("Work Fault"))
        {
            _debug?.WorkFault();
        }
        if (GUILayout.Button("Time - 10s"))
        {
            gameProgress._debugTimeMinus(10f);
        }
        if (GUILayout.Button("Continue"))
        {
            gameProgress.Continue();
        }
        if (GUILayout.Button("Restart"))
        {
            gameProgress.StartGame();
        }
    }
}
