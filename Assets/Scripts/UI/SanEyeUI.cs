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
    //[SerializeField] private bool punchOnChange = true;

    [SerializeField] private float smallDuration;
    [SerializeField] private float smallStrength;
    [SerializeField] private int smallVibrato;

    [SerializeField] private float bigDuration;
    [SerializeField] private float bigStrength;
    [SerializeField] private int bigVibrato;
    [SerializeField] private float bigPunchScale;
    #endregion

    #region STATE
    private int lastSan = int.MinValue;
    private int lastStage = -1;
    private RectTransform rect;
    #endregion

    private void Awake()
    {
        if (eyeImage == null) eyeImage = GetComponent<Image>();
        rect = eyeImage.rectTransform;
        if (progress == null) progress = FindFirstObjectByType<GameProgress>();
    }

    private void Update()
    {
        if (progress == null) return;

        int san = progress.CurrentSan;
        if (san == lastSan) return;

        bool dropped = lastSan != int.MinValue && san < lastSan;
        lastSan = san;
        Refresh(san, dropped);
    }

    #region VISUALS
    private void Refresh(int san, bool dropped)
    {
        int index = GetStageIndex(san);
        if (index < 0) return;

        bool stageChanged = index != lastStage;
        lastStage = index;

        if (stageChanged && stages[index].eye != null)
            eyeImage.sprite = stages[index].eye;

        if (!dropped) return;

        if (stageChanged)
            PlayBigShake();
        else
            PlaySmallShake();
    }

    private void PlaySmallShake()
    {
        rect.DOKill(true);
        rect.DOShakeAnchorPos(smallDuration, new Vector2(smallStrength, 0f), smallVibrato, 0f)
            .SetLink(gameObject);
    }

    private void PlayBigShake()
    {
        rect.DOKill(true);
        rect.DOShakeAnchorPos(bigDuration, bigStrength, bigVibrato, 90f).SetLink(gameObject);
        rect.DOPunchScale(Vector3.one * bigPunchScale, bigDuration, 8).SetLink(gameObject);
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
