using System;
using System.Collections;
using System.Linq;
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

    [SerializeField] SpriteRenderer spriteRendererBG;
    [SerializeField] SpriteRenderer spriteRendererResultBG;
    [SerializeField] Sprite spriteDeadOfFish;
    [SerializeField] Sprite spriteRanOutOfSan;

    [SerializeField] private string strSuccess;
    [SerializeField] private string strRanOutOfSan;
    [SerializeField] private string strDeadOfFish;
    [SerializeField] private string strSubSuccess;
    [SerializeField] private string strSubRanOutOfSan;
    [SerializeField] private string strSubDeadOfFish;
    [SerializeField] private Sprite[] sprites;

    void Start()
    {
        container.gameObject.SetActive(false);

        btnContinue.onClick.AddListener(OnContinue);
        btnRetry.onClick.AddListener(OnContinue);
        btnMenu.onClick.AddListener(OnMenu);

        var data = SaveDataContainer.Instance.CurrentData;
        if (data == null)
        {
            data = SaveDataContainer.Instance.LoadOrNewCurrentData(0, true);
            data.Result = GameProgress.GameProgressState.ResultSuccess;
            data.day = 7;
        }

        txtDay.text = $"day{SaveDataContainer.Instance.CurrentData.day}";
        txtStatus.text = SaveDataContainer.Instance.CurrentData.Result switch
        {
            GameProgress.GameProgressState.ResultSuccess => strSuccess,
            GameProgress.GameProgressState.ResultRanOutOfSan => strRanOutOfSan,
            GameProgress.GameProgressState.ResultDeadOfFish => strDeadOfFish,
            _ => "Undefined",
        };
        txtStatusSub.text = SaveDataContainer.Instance.CurrentData.Result switch
        {
            GameProgress.GameProgressState.ResultSuccess => strSubSuccess,
            GameProgress.GameProgressState.ResultRanOutOfSan => strSubRanOutOfSan,
            GameProgress.GameProgressState.ResultDeadOfFish => strSubDeadOfFish,
            _ => "Undefined",
        };

        int idxStatus = Mathf.Clamp(SaveDataContainer.Instance.CurrentData.san, 0, sprites.Length - 1);
        imgStatus.sprite = sprites[idxStatus];

        btnContinue.gameObject.SetActive(SaveDataContainer.Instance.CurrentData.Result == GameProgress.GameProgressState.ResultSuccess);
        btnRetry.gameObject.SetActive(SaveDataContainer.Instance.CurrentData.Result != GameProgress.GameProgressState.ResultSuccess);

        if (SaveDataContainer.Instance.CurrentData.Result == GameProgress.GameProgressState.ResultDeadOfFish)
        {
            spriteRendererBG.gameObject.SetActive(false);
            spriteRendererResultBG.gameObject.SetActive(true);
            spriteRendererResultBG.sprite = spriteDeadOfFish;
        }
        else if (SaveDataContainer.Instance.CurrentData.Result == GameProgress.GameProgressState.ResultRanOutOfSan)
        {
            spriteRendererBG.gameObject.SetActive(false);
            spriteRendererResultBG.gameObject.SetActive(true);
            spriteRendererResultBG.sprite = spriteRanOutOfSan;
        }
        else
        {
            spriteRendererBG.gameObject.SetActive(true);
            spriteRendererResultBG.gameObject.SetActive(false);
        }

        itemEventPrefab.gameObject.SetActive(false);
        var day = SaveDataContainer.Instance.CurrentData.day;
        // day is incremented due to day finished in GameProgress.cs
        // therefore it is decrement for fetching the data
        if (SaveDataContainer.Instance.CurrentData.Result == GameProgress.GameProgressState.ResultSuccess)
            day--;

        if (day >= 0 && day < SaveDataContainer.Instance.CurrentData.records.Count)
        {
            var record = SaveDataContainer.Instance.CurrentData.records[day];
            for (int i=0; i< record.monsiterTypes.Count; ++i)
            {
                var instance = GameObject.Instantiate(itemEventPrefab, itemEventPrefab.transform.parent);
                instance.Refresh(record.monsiterTypes[i], record.monisterIdxBehaviourSolveds[i]);
                instance.gameObject.SetActive(true);
            }
        }

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
        if (SaveDataContainer.Instance.CurrentData.day < GameConfigContainer.Instance.Config.dayConfigs.Count())
        {
            UILoading.Instance.Load(SceneNames.Office, null);
        }
        else
        {
            UILoading.Instance.Load(SceneNames.GameFinished, null);
        }
    }

    private void RefreshUI()
    {
        container.gameObject.SetActive(true);
    }
}
