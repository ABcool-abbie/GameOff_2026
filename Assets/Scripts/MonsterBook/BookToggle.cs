using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility;

public class BookToggle : SingletonBehaviour<BookToggle>
{
    #region SETTINGS
    [SerializeField] private GameObject book;
    [SerializeField] private Button openButton; 
    [SerializeField] private Button closeButton;
    #endregion

    #region STATE
    public static bool IsOpen { get; private set; }
    #endregion
    protected override void Init()
    {
        if (openButton != null) openButton.onClick.AddListener(Open);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        book.SetActive(false);
        IsOpen = false;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame) Toggle();
    }

    private void OnDestroy()
    {
        if (IsOpen) PauseHelper.Resume(this);
        IsOpen = false;
    }


    #region OPEN / CLOSE
    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (IsOpen) return;
        PauseHelper.Pause(this);

        book.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen) return;
        PauseHelper.Resume(this);

        book.SetActive(false);
        IsOpen = false;
    }
    #endregion
}
