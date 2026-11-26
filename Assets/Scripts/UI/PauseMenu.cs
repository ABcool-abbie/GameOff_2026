using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button resumeBtn;
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

        Toggle();
    }

    private void OnDestroy()
    {
        if (IsOpen)
            PauseHelper.Resume(this);

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

        panel.SetActive(true);
        IsOpen = true;

        PauseHelper.Pause(this);
    }

    public void Close()
    {
        if (!IsOpen) return;

        panel.SetActive(false);
        IsOpen = false;

        PauseHelper.Resume(this);
    }
    #endregion
}
