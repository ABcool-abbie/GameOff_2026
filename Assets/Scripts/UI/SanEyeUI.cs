using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SanEyeUI : MonoBehaviour
{
    [Serializable]
    private struct SanStage
    {
        public int minSan;
        public Sprite eye;
    }

    #region SETTINGS
    [SerializeField] private GameProgress progress;
    [SerializeField] private Image eyeImage;
    [SerializeField] private SanStage[] stages;
    [SerializeField] private bool punchOnChange = true;
    #endregion

    #region STATE
    private int lastSan = int.MinValue;
    private int lastStage = -1;
    #endregion

    private void Awake()
    {
        if (eyeImage == null) eyeImage = GetComponent<Image>();
        if (progress == null) progress = FindFirstObjectByType<GameProgress>();
    }

    private void Update()
    {
        if (progress == null) return;

        int san = progress.CurrentSan;
        if (san == lastSan) return;

        lastSan = san;
        Refresh(san);
    }

    #region VISUALS
    private void Refresh(int san)
    {
        int index = GetStageIndex(san);
        if (index < 0 || index == lastStage) return;

        bool firstTime = lastStage < 0;
        lastStage = index;

        if (stages[index].eye != null)
            eyeImage.sprite = stages[index].eye;

        if (punchOnChange && !firstTime)
        {
            eyeImage.rectTransform.DOKill(true);
            eyeImage.rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.4f, 8).SetLink(gameObject);
        }
    }

    private int GetStageIndex(int san)
    {
        if (stages == null || stages.Length == 0) return -1;

        for (int i = 0; i < stages.Length; i++)
        {
            if (san >= stages[i].minSan) return i;
        }
        return stages.Length - 1;
    }
    #endregion
}
