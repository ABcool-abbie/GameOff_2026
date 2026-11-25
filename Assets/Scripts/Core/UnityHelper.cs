using System;
using System.Collections;
using UnityEngine;

public static class UnityHelper
{
    public static T Unwrap<T>(this T self) where T : UnityEngine.Object
    {
        if (self == null)
            return null;
        return self;
    }

    public static void CallbackOnParticleSystemStop(this ParticleSystem particleSystem, Action<ParticleSystem> callback)
    {
        if (particleSystem == null)
            return;
        try
        {
            GameProgress.Instance.StartCoroutine(CallbackOnParticleSystemStopCoroutine(particleSystem, callback));
        }
        catch(Exception e)
        {
            GameObject.Destroy(particleSystem.gameObject);
        }
    }

    static IEnumerator CallbackOnParticleSystemStopCoroutine(ParticleSystem particleSystem, Action<ParticleSystem> callback)
    {
        particleSystem.Stop();

        yield return new WaitUntil(() => particleSystem == null || particleSystem.particleCount < 1);

        if (particleSystem != null)
            callback(particleSystem);
    }
}
