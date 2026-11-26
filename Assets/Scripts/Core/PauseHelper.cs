using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHelper : MonoBehaviour
{
    private static readonly HashSet<object> pausers = new HashSet<object>();

    public static bool IsPaused => pausers.Count > 0;

    public static void Pause(object who)
    {
        pausers.Add(who);
        Apply();
    }

    public static void Resume(object who)
    {
        pausers.Remove(who);
        Apply();
    }

    private static void Apply()
    {
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        pausers.Clear();
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            pausers.Clear();
            Apply();
        };
    }
}
