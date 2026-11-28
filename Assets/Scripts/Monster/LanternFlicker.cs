using DG.Tweening;
using UnityEngine;

public class LanternFlicker : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private SpriteRenderer flickerSprite;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 0.8f;
    [SerializeField] private float minStep = 0.03f; 
    [SerializeField] private float maxStep = 0.25f; 
    #endregion

    private void Start()
    {
        TankState tank = GetComponentInParent<TankState>();
        if (tank != null) tank.SetLight(false);

        if (flickerSprite != null) FlickerOnce();
    }

    private void FlickerOnce()
    {
        float alpha = Random.Range(minAlpha, maxAlpha);
        float time = Random.Range(minStep, maxStep);

        flickerSprite.DOFade(alpha, time)
            .SetLink(gameObject) 
            .OnComplete(FlickerOnce);
    }
}
