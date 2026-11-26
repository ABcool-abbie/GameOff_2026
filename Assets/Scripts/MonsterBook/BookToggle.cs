using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BookToggle : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private GameObject book;
    [SerializeField] private Button openButton; 
    [SerializeField] private Button closeButton;
    #endregion

    #region STATE
    public static bool IsOpen { get; private set; }
    private float previousTimeScale = 1f;
    #endregion

    private void Awake()
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
        if (IsOpen) Time.timeScale = previousTimeScale;

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
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        book.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen) return;
        Time.timeScale = previousTimeScale;

        book.SetActive(false);
        IsOpen = false;
    }
    #endregion
}
