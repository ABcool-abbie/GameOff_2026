using System;
using UnityEngine;
using UnityEngine.UI;

public class UISettlement : MonoBehaviour
{
    [SerializeField] RectTransform container;
    [SerializeField] Button btnContinue;
    [SerializeField] GameProgress gameProgress;

    void Start()
    {
        container.gameObject.SetActive(false);

        gameProgress.OnDayFinished += RefreshUI;

        btnContinue.onClick.AddListener(OnContinue);
    }

    void OnDestroy()
    {
        gameProgress.OnDayFinished -= RefreshUI;

        btnContinue.onClick.RemoveListener(OnContinue);
    }

    private void OnContinue()
    {
        SaveDataContainer.Instance.CurrentData.day++;
        SaveDataContainer.Instance.SaveCurrentData();
        UILoading.Instance.Load(SceneNames.Office, null);
    }

    private void RefreshUI()
    {
        container.gameObject.SetActive(true);
    }
}
