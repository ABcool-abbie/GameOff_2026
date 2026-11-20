using UnityEngine;
using UnityEngine.UI;

public class ControlPanelUI : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private TankState[] tanks;

    [Header("Sliders")]
    [SerializeField] private Slider tempSlider;
    [SerializeField] private Slider oxygenSlider;

    [Header("Buttons")]
    [SerializeField] private Button feedButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button lightButton;

    [Header("Light Sprites")]
    [SerializeField] private Image lightImage;
    [SerializeField] private Sprite lightOnSprite;
    [SerializeField] private Sprite lightOffSprite;
    #endregion

    private TankState current;

    private void Awake()
    {
        SetupSlider(tempSlider, -2, 2);
        SetupSlider(oxygenSlider, -1, 1);

        tempSlider.onValueChanged.AddListener(OnTemperatureChanged);
        oxygenSlider.onValueChanged.AddListener(OnOxygenChanged);

        feedButton.onClick.AddListener(OnFeedClicked);
        soundButton.onClick.AddListener(OnSoundClicked);
        lightButton.onClick.AddListener(OnLightClicked);

        var none = new Navigation { mode = Navigation.Mode.None };
        tempSlider.navigation = none;
        oxygenSlider.navigation = none;
        feedButton.navigation = none;
        soundButton.navigation = none;
        lightButton.navigation = none;
    }

    private void OnEnable() => monitorManager.ViewChanged += OnViewChanged;
    private void OnDisable() => monitorManager.ViewChanged -= OnViewChanged;

    #region INPUTS
    private void OnTemperatureChanged(float value)
    {
        if (current == null) return;
        current.SetTemperature((TemperatureLevel)(int)value);
    }

    private void OnOxygenChanged(float value)
    {
        if (current == null) return;
        current.SetOxygen((OxygenLevel)(int)value);
    }

    private void OnFeedClicked()
    {
        if (current == null) return;
        current.Feed();
    }

    private void OnSoundClicked()
    {
        if (current == null) return;
        current.SetSound(!current.SoundOn);
    }

    private void OnLightClicked()
    {
        if (current == null) return;
        current.SetLight(!current.LightOn);
    }
    #endregion

    #region SETUP
    private void SetupSlider(Slider slider, int min, int max)
    {
        slider.wholeNumbers = true;
        slider.minValue = min;
        slider.maxValue = max;
    }

    private void OnViewChanged(int view)
    {
        if (current != null) current.Changed -= OnTankChanged;

        int index = monitorManager.CurrentCameraIndex;
        current = index >= 0 ? tanks[index] : null;

        if (current == null) return;
        current.Changed += OnTankChanged;
        RefreshUI();
    }

    private void OnTankChanged(TankState tank) => RefreshUI();
    #endregion

    #region UI
    private void RefreshUI()
    {
        tempSlider.SetValueWithoutNotify((int)current.Temperature);
        oxygenSlider.SetValueWithoutNotify((int)current.Oxygen);

        SetSprite(lightImage, current.LightOn, lightOnSprite, lightOffSprite);
    }

    private void SetSprite(Image image, bool on, Sprite onSprite, Sprite offSprite)
    {
        if (image == null) return;
        Sprite sprite = on ? onSprite : offSprite;
        if (sprite != null) image.sprite = sprite;
    }
    #endregion
}