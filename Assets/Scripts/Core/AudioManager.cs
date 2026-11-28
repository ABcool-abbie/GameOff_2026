using UnityEngine;
using UnityEngine.Audio;
using Utility;

public enum VolumeChannel { Master, Music, SFX }

// One per scene (put the prefab in every scene).
// Plays one-shot SFX, UI clicks and the scene's looping music, and saves volumes in PlayerPrefs.
public class AudioManager : SingletonBehaviour<AudioManager>
{
    #region SETTINGS
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Scene Music")]
    [SerializeField] private AudioClip[] sceneMusic;
    [SerializeField, Range(0f, 1f)] private float sceneMusicVolume = 1f;
    #endregion

    #region STATE
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource uiSource;
    #endregion

    protected override void Init()
    {
        musicSource = CreateSource("Music", musicGroup, true);
        sfxSource = CreateSource("SFX", sfxGroup, false);
        uiSource = CreateSource("UI", sfxGroup, false);

        uiSource.ignoreListenerPause = true;
    }

    private void Start()
    {
        foreach (VolumeChannel channel in System.Enum.GetValues(typeof(VolumeChannel)))
            ApplyVolume(channel, GetVolume(channel));

        AudioClip music = PickRandom(sceneMusic);
        if (music != null) PlayMusic(music, sceneMusicVolume);
    }

    #region PLAY
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayUI(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        uiSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    private AudioClip PickRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    #endregion

    #region VOLUME
    public void SetVolume(VolumeChannel channel, float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(Key(channel), value);
        ApplyVolume(channel, value);
    }

    public float GetVolume(VolumeChannel channel)
    {
        return PlayerPrefs.GetFloat(Key(channel), 1f);
    }

    private void ApplyVolume(VolumeChannel channel, float value)
    {
        if (mixer == null) return;

        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        mixer.SetFloat(channel + "Volume", db);
    }

    private string Key(VolumeChannel channel) => "Volume_" + channel;
    #endregion

    #region SETUP
    private AudioSource CreateSource(string sourceName, AudioMixerGroup group, bool loop)
    {
        GameObject go = new GameObject(sourceName);
        go.transform.SetParent(transform, false);

        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;
        source.playOnAwake = false;
        source.loop = loop;
        return source;
    }
    #endregion
}
