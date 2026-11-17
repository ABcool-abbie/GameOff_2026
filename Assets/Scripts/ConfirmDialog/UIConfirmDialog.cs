using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

public class UIConfirmDialog : SingletonBehaviour<UIConfirmDialog>, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public struct Config
    {
        public string title;
        public string message;
        public Action onConfirm;
        public Action onCancel;
    }

    [SerializeField] RectTransform container;
    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtMessage;
    [SerializeField] Button btnConfirm;
    [SerializeField] Button btnCancel;

    Queue<Config> _queueConfigs = new();

    void Start()
    {
        container.gameObject.SetActive(false);

        btnConfirm.onClick.AddListener(OnConfrim);
        btnCancel.onClick.AddListener(OnCancel);
    }

    void Oestroy()
    {
        btnConfirm.onClick.RemoveListener(OnConfrim);
        btnCancel.onClick.RemoveListener(OnCancel);
    }

    private void OnConfrim()
    {
        if (!_queueConfigs.Any())
            return;
        var cfg = _queueConfigs.Dequeue();
        cfg.onConfirm?.Invoke();
        RefreshUI();
    }

    private void OnCancel()
    {
        if (!_queueConfigs.Any())
            return;
        var cfg = _queueConfigs.Dequeue();
        cfg.onCancel?.Invoke();
        RefreshUI();
    }

    public void Show(Config config)
    {
        _queueConfigs.Enqueue(config);

        RefreshUI();
    }

    private void RefreshUI()
    {
        container.gameObject.SetActive(_queueConfigs.Any());
        if (!_queueConfigs.Any())
            return;

        var cfg = _queueConfigs.Peek();
        txtTitle.text = cfg.title;
        txtMessage.text = cfg.message;
    }

    // Swallow pointer event
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }
}
