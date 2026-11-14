using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button btnStartGame;

    void Start()
    {
        btnStartGame.onClick.AddListener(OnBtnStart);
    }

    void OnDestroy()
    {
        btnStartGame.onClick.RemoveListener(OnBtnStart);
    }

    private void OnBtnStart()
    {
        SaveDataContainer.Instance.LoadOrNewCurrentData(0, false);
        UILoading.Instance.Load(SceneNames.Office, null);
    }
}
