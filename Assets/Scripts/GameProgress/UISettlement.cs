using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettlement : MonoBehaviour
{
    [SerializeField] RectTransform container;
    [SerializeField] Button btnContinue;
    [SerializeField] Button btnRetry;
    [SerializeField] Button btnMenu;
    [SerializeField] TextMeshProUGUI txtDay;
    [SerializeField] TextMeshProUGUI txtStatus;
    [SerializeField] TextMeshProUGUI txtStatusSub;
    [SerializeField] Image imgStatus;
    [SerializeField] UIItemEvent itemEventPrefab;

    [SerializeField] private string strSuccess;
    [SerializeField] private string strRanOutOfSan;
    [SerializeField] private string strDeadOfFish;
    [SerializeField] private Sprite[] sprites;

    void Start()
    {
        container.gameObject.SetActive(false);

        btnContinue.onClick.AddListener(OnContinue);
        btnRetry.onClick.AddListener(OnContinue);
        btnMenu.onClick.AddListener(OnMenu);

        txtDay.text = $"day{SaveDataContainer.Instance.CurrentData.day}";
        txtStatus.text = SaveDataContainer.Instance.CurrentData.Result switch
        {
            GameProgress.GameProgressState.ResultSuccess => strSuccess,
            GameProgress.GameProgressState.ResultRanOutOfSan => strRanOutOfSan,
            GameProgress.GameProgressState.ResultDeadOfFish => strDeadOfFish,
            _ => "Undefined",
        };
        txtStatusSub.text = "";

        imgStatus.sprite = sprites[SaveDataContainer.Instance.CurrentData.san];

        btnContinue.gameObject.SetActive(SaveDataContainer.Instance.CurrentData.Result == GameProgress.GameProgressState.ResultSuccess);
        btnRetry.gameObject.SetActive(SaveDataContainer.Instance.CurrentData.Result != GameProgress.GameProgressState.ResultSuccess);

        itemEventPrefab.gameObject.SetActive(false);
        var day = SaveDataContainer.Instance.CurrentData.day;
        if (SaveDataContainer.Instance.CurrentData.Result == GameProgress.GameProgressState.ResultSuccess)
            day--;
        var record = SaveDataContainer.Instance.CurrentData.records[day];
        // for (int i=0; i< record.monsiterTypes.Count; ++i)
        // {
        //     var instance = GameObject.Instantiate(itemEventPrefab, itemEventPrefab.transform.parent);
        //     instance.Refresh(SaveDataContainer.Instance.CurrentData.records[i]);
        //     instance.gameObject.SetActive(true);
        // }

        RefreshUI();
    }

    void OnDestroy()
    {
        // gameProgress.OnDayFinished -= RefreshUI;

        btnContinue.onClick.RemoveListener(OnContinue);
        btnRetry.onClick.AddListener(OnContinue);
        btnMenu.onClick.RemoveListener(OnMenu);
    }

    private void OnMenu()
    {
        UILoading.Instance.Load(SceneNames.Menu, null);
    }

    private void OnContinue()
    {
        UILoading.Instance.Load(SceneNames.Office, null);
    }

    private void RefreshUI()
    {
        container.gameObject.SetActive(true);
    }
}
