using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenOption : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private string onText = "ON";
    [SerializeField] private string offText = "OFF";
    #endregion

    #region STATE
    private bool isFullscreen;
    #endregion

    private void Awake()
    {
        if(prevBtn != null)
        {
            prevBtn.onClick.AddListener(Toggle);
        }

        if (nextBtn != null)
        {
            nextBtn.onClick.AddListener(Toggle);
        }
    }

    private void OnEnable()
    {
        isFullscreen = Screen.fullScreen;
        ShowValue(isFullscreen);
    }

    #region OPTION

    public void Toggle()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
        ShowValue(isFullscreen);
    }

    private void ShowValue(bool on)
    {
        valueText.text = on ? onText : offText;
    }
    #endregion
}
