using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class ClickSound : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField, Range(0f, 1f)] private float volume = 0.6f;
    [SerializeField, Range(0f, 0.3f)] private float pitchRange = 0.05f;
    #endregion

    #region STATE
    private AudioSource source;
    #endregion

    #region UNITY
    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = sfxGroup;
        source.ignoreListenerPause = true;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            Play(clip);
    }
    #endregion

    private void Play(AudioClip sound)
    {
        if (sound == null) return;

        source.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        source.PlayOneShot(sound, volume);
    }
}