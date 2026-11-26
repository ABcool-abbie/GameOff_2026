using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public enum Mode { InGame, MainMenu }

    #region SETTINGS
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button resumeBtn;

    [Header("Mode")]
    [SerializeField] private Mode mode = Mode.InGame;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private string inGameTitle = "PAUSED";
    [SerializeField] private string mainMenuTitle = "OPTIONS";

    [SerializeField] private GameObject inGameButtons;
    [SerializeField] private Button openButton;

    #endregion

    #region STATE
    public static bool IsOpen { get; private set; }
    #endregion

    private void Awake()
    {
        if (closeBtn != null)
            closeBtn.onClick.AddListener(Close);

        if (resumeBtn != null)
            resumeBtn.onClick.AddListener(Close);

        if (openButton != null)
            openButton.onClick.AddListener(Open);

        ApplyMode();
    }

    private void Start()
    {
        panel.SetActive(false);
        IsOpen = false;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (BookToggle.IsOpen)
        {
            BookToggle.Instance.Close();
            return;
        }

        if (mode == Mode.MainMenu && !IsOpen) return;
        Toggle();
    }

    private void OnDestroy()
    {
        if (IsOpen)
            PauseHelper.Resume(this);

        IsOpen = false;
    }

    #region Options
    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (IsOpen) return;

        panel.SetActive(true);
        IsOpen = true;

        if (mode == Mode.InGame)
            PauseHelper.Pause(this);
    }

    public void Close()
    {
        if (!IsOpen) return;

        panel.SetActive(false);
        IsOpen = false;

        if (mode == Mode.InGame)
            PauseHelper.Resume(this);
    }

    private void ApplyMode()
    {
        bool inGame = mode == Mode.InGame;

        if (titleText != null)
            titleText.text = inGame ? inGameTitle : mainMenuTitle;

        if (inGameButtons != null)
            inGameButtons.SetActive(inGame);
    }
    #endregion
}
