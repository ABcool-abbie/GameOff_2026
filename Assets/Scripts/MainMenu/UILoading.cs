using System;
using System.Collections;
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
        const float FADE_DURATION = 0.5f; //BBR: adjust frm 0.2

        _loading = true;
        container.gameObject.SetActive(true);

        //BBR: reset alpha
        var group = container.GetComponent<CanvasGroup>();
        if (group != null) group.alpha = 0f;
        Debug.Log($"[UILoading] group={(group != null)} on {container.name}, timeScale={Time.timeScale}"); 
        //BBR: reset alpha

        yield return container.GetComponent<CanvasGroup>().Unwrap()?.DOFade(1f, FADE_DURATION).WaitForCompletion();

        yield return SceneManager.LoadSceneAsync(scenePath);

        yield return container.GetComponent<CanvasGroup>().Unwrap()?.DOFade(0f, FADE_DURATION).WaitForCompletion();

        container.gameObject.SetActive(false);
        _loading = false;

        OnFinish?.Invoke();
    }
}
