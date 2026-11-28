using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform container;
    [SerializeField] TextMeshProUGUI txtSpeaker;
    [SerializeField] TextMeshProUGUI txtMessage;
    [SerializeField] Image imageCharacter;
    [SerializeField] Sprite[] characterImages;

    [SerializeField] bool isDebugDay;
    [SerializeField] int debugDay;

    DialogConfig _config;
    private TweenerCore<string, string, StringOptions> _curTween;
    private bool _isContinue;
    private bool _hasDialogue;

    void Start()
    {
        container.gameObject.SetActive(false);

        int day = SaveDataContainer.Instance.CurrentData?.day ?? 0;

        if (isDebugDay)
            day = debugDay;
        day = Mathf.Clamp(day, 0, GameConfigContainer.Instance.Config.dayConfigs.Length - 1);

        StartDialogue(GameConfigContainer.Instance.Config.dayConfigs[day].DialogConfig);
    }

    void Onestroy()
    {
    }

    public void StartDialogue(DialogConfig config)
    {
        _config = config;

        StartCoroutine(StartDialogueCoroutine());
    }

    void RefreshImageCharacter(Image image, int iMessage, DialogConfig.Dialog dialog)
    {
        int spriteIdx = iMessage >=0 && iMessage < dialog.SpriteIdxes.Length ? dialog.SpriteIdxes[iMessage] : -1;
        image.sprite = characterImages.ElementAtOrDefault(spriteIdx);
        image.enabled = image.sprite != null;
    }

    IEnumerator StartDialogueCoroutine()
    {
        const float FADE_DURATION = 0.2f;
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

            txtMessage.text = "";
            RefreshImageCharacter(imageCharacter, 0, dialog);

            yield return canvasGroup.DOFade(1f, FADE_DURATION).WaitForCompletion();

            while(iMessage < dialog.Speaches.Length)
            {
                RefreshImageCharacter(imageCharacter, iMessage, dialog);
                var msg = dialog.Speaches[iMessage];
                var tweenTime = msg.Split(' ').Length * PER_TEXT_DURATION;
                _curTween = DOTween.To(()=>"", x => txtMessage.text = x,  msg, tweenTime).SetEase(Ease.Linear);
                yield return _curTween.WaitForCompletion();
                _curTween = null;

                yield return new WaitUntil(()=>_isContinue);
                _isContinue = false;

                iMessage++;
            }

            yield return canvasGroup.DOFade(0f, FADE_DURATION).WaitForCompletion();

            iSpeaker++;
        }

        _hasDialogue = false;
        container.gameObject.SetActive(false);

        // Ensure not to load scene on the same frame when scene loaded
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        UILoading.Instance.Load(SceneNames.Aquarium, null);
    }

    void ContinueDialogue()
    {
        if (!_hasDialogue)
            return;

        if (_curTween != null)
            _curTween.Kill(true);
        else
            _isContinue = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ContinueDialogue();
    }
}
