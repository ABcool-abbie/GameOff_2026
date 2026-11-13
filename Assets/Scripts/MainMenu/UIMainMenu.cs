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
        SaveDataContainer.Instance.LoadOrNewCurrentData(0, true);
        UILoading.Instance.Load("02_Office", () =>
        {
            throw new NotImplementedException();
            // GameProgress.Instance.Register();
        });
    }
}
