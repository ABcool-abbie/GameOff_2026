using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDialogue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform container;
    [SerializeField] TextMeshProUGUI txtSpeaker;
    [SerializeField] TextMeshProUGUI txtMessage;
    [SerializeField] GameProgress gameProgress;

    DialogConfig _config;
    private TweenerCore<string, string, StringOptions> _curTween;
    private bool _isContinue;
    private bool _hasDialogue;

    void Start()
    {
        gameProgress.OnDayChanged += OnDayChanged;
        container.gameObject.SetActive(false);
    }

    void Onestroy()
    {
        gameProgress.OnDayChanged -= OnDayChanged;
    }

    public void StartDialogue(DialogConfig config)
    {
        _config = config;

        StartCoroutine(StartDialogueCoroutine());
    }

    IEnumerator StartDialogueCoroutine()
    {
        const float FADE_DURATION = 1f;
        const float PER_TEXT_DURATION = 0.2f;

        _hasDialogue = true;

        canvasGroup.alpha = 0f;
        container.gameObject.SetActive(true);

        int iSpeaker = 0;
        while (iSpeaker < _config.dialogs.Length)
        {
            var dialog = _config.dialogs[iSpeaker];

            int iMessage = 0;
            txtSpeaker.text = dialog.Speaker;
            txtSpeaker.enabled = !string.IsNullOrWhiteSpace(dialog.Speaker);

            yield return canvasGroup.DOFade(1f, FADE_DURATION);

            while(iMessage < dialog.Speaches.Length)
            {
                var msg = dialog.Speaches[iMessage];
                var tweenTime = msg.Split(' ').Length * PER_TEXT_DURATION;
                _curTween = DOTween.To(()=>"", x => txtMessage.text = x,  msg, tweenTime).SetEase(Ease.Linear);
                yield return _curTween;
                _curTween = null;

                yield return new WaitUntil(()=>_isContinue);
                _isContinue = false;

                iMessage++;
            }

            yield return canvasGroup.DOFade(0f, FADE_DURATION);

            iSpeaker++;
        }

        _hasDialogue = false;
        container.gameObject.SetActive(false);

        gameProgress.Continue();

        yield break;
    }

    void ContinueDialogue()
    {
        if (!_hasDialogue)
            return;

        if (_curTween != null)
            _curTween.Done();
        else
            _isContinue = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ContinueDialogue();
    }

    private void OnDayChanged()
    {
        int day = gameProgress.TotalDay - gameProgress.DayLeft;
        var config = gameProgress.Config.dayConfigs.ElementAtOrDefault(day);
        if (config.DialogConfig == null)
            gameProgress.Continue();
        else
            StartDialogue(config.DialogConfig);
    }
}
