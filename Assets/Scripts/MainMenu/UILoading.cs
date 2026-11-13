using System;
using System.Collections;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class UILoading : SingletonBehaviour<UILoading>
{
    [SerializeField] RectTransform container;
    private bool _loading;

    void Start()
    {
        container.gameObject.SetActive(false);
    }

    public void Load(string scenePath, Action OnFinish)
    {
        if (_loading)
            return;
        StartCoroutine(LoadCoroutine(scenePath, OnFinish));
    }

    IEnumerator LoadCoroutine(string scenePath, Action OnFinish)
    {
        const float FADE_DURATION = 0.5f;

        _loading = true;
        container.gameObject.SetActive(true);

        yield return container.GetComponent<CanvasGroup>().Unwrap()?.DOFade(1f, FADE_DURATION);

        yield return SceneManager.LoadSceneAsync(scenePath);

        yield return container.GetComponent<CanvasGroup>().Unwrap()?.DOFade(0f, FADE_DURATION);

        container.gameObject.SetActive(false);
        _loading = false;

        OnFinish?.Invoke();
    }
}
