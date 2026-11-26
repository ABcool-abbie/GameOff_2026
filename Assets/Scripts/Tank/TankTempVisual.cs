using DG.Tweening;
using UnityEngine;


public class TankTempVisual : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private TankState tank;


    [Header("Ice")]
    [SerializeField] private SpriteRenderer ice;
    [SerializeField] private float coldAlpha = 0.5f;
    [SerializeField] private float veryColdAlpha = 1f;
    [SerializeField] private float coldTime = 1.5f;

    [Header("Hot")]
    [SerializeField] private SpriteRenderer heat;
    [SerializeField] private float warmAlpha = 0.5f;
    [SerializeField] private float hotAlpha = 1f;

    [SerializeField] private float fadeTime = 1.5f;
    #endregion

    #region STATS
    private TemperatureLevel tempLevel;
    private Tween coldTween;
    private Tween hotTween;


    #endregion

    private void Awake()
    {
        if (tank == null) tank = GetComponentInParent<TankState>();
    }

    private void OnEnable()
    {
        if (tank == null) return;
        tank.Changed += OnTankChanged;
    }

    private void OnDisable()
    {
        if (tank == null) return;
        tank.Changed -= OnTankChanged;
    }

    private void Start()
    {
        if (tank == null) return;

        tempLevel = tank.Temperature;
        SetAlpha(ice, GetColdAlpha(tempLevel));
        SetAlpha(heat, GetHotAlpha(tempLevel));
    }

    private void OnTankChanged(TankState state)
    {
        if (state.Temperature == tempLevel) return;
        tempLevel = state.Temperature;

        coldTween = FadeTo(ice, coldTween, GetColdAlpha(tempLevel));
        hotTween = FadeTo(heat, hotTween, GetHotAlpha(tempLevel));

    }

    #region VISUALS
    private float GetColdAlpha(TemperatureLevel temperature)
    {
        if (temperature == TemperatureLevel.VeryCold) return veryColdAlpha;
        if (temperature == TemperatureLevel.Cold) return coldAlpha;
        return 0f;
    }

    private float GetHotAlpha(TemperatureLevel temperature)
    {
        if (temperature == TemperatureLevel.Warm) return warmAlpha;
        if (temperature == TemperatureLevel.Hot) return hotAlpha;
        return 0f;
    }

    private Tween FadeTo(SpriteRenderer target, Tween oldTween, float alpha)
    {
        if (oldTween != null) oldTween.Kill();
        if (target == null) return null;

        return target.DOFade(alpha, fadeTime).SetLink(gameObject);
    }

    private void SetAlpha(SpriteRenderer target, float alpha)
    {
        if (target == null) return;

        Color c = target.color;
        c.a = alpha;
        target.color = c;
    }
    #endregion
}
