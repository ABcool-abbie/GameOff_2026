using UnityEngine;
using UnityEngine.UI;

public class VolumeOption : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private Slider slider;
    [SerializeField] private VolumeChannel channel = VolumeChannel.Master;
    #endregion

    private void Awake()
    {
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnEnable()
    {
        if (slider == null || !AudioManager.HasInstance) return;

        float volume = AudioManager.Instance.GetVolume(channel);
        slider.SetValueWithoutNotify(volume);
    }

    private void OnSliderChanged(float value)
    {
        if (!AudioManager.HasInstance) return;

        AudioManager.Instance.SetVolume(channel, value);
    }
}
