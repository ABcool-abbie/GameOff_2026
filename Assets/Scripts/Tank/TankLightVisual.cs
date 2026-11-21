using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TankLightVisual : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private TankState tank;
    [SerializeField] private float offFadeTime = 0.3f;
    [SerializeField] private float onFadeTime = 0.3f;
    #endregion

    #region STATE
    private SpriteRenderer overlay;
    private float darkAlpha;
    private bool lastLightOn;
    private Tween lightTween;
    #endregion


    private void Awake()
    {
        overlay = GetComponent<SpriteRenderer>();
        darkAlpha = overlay.color.a;

        if (tank == null) tank = GetComponentInParent<TankState>();
    }

    private void OnEnable()
    {
        if (tank == null) return;
        tank.Changed += OnTankChanged;
        tank.NightReset += OnNightReset;
    }

    private void OnDisable()
    {
        if (tank == null) return;
        tank.Changed -= OnTankChanged;
        tank.NightReset -= OnNightReset;
    }

    private void Start() => Snap();

    private void OnTankChanged(TankState state)
    {
        if (state.LightOn == lastLightOn) return;
        lastLightOn = state.LightOn;

        KillTween();
        lightTween = lastLightOn ? PlayFlickerOn() : PlayFadeOff();
    }

    private void OnNightReset(TankState state) => Snap();

    #region VISUALS
    private void Snap()
    {
        KillTween();
        lastLightOn = tank != null && tank.LightOn;
        SetAlpha(lastLightOn ? 0f : darkAlpha);
    }

    private Tween PlayFlickerOn()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(overlay.DOFade(0f, 0.04f));
        seq.AppendInterval(0.1f);
        seq.Append(overlay.DOFade(darkAlpha, 0.04f));
        seq.AppendInterval(0.12f);
        seq.Append(overlay.DOFade(0f, 0.04f));
        seq.AppendInterval(0.05f);
        seq.Append(overlay.DOFade(darkAlpha * 0.6f, 0.04f));
        seq.AppendInterval(0.08f);
        seq.Append(overlay.DOFade(0f, onFadeTime));
        seq.SetLink(gameObject);
        return seq;
    }

    private Tween PlayFadeOff()
    {
        return overlay.DOFade(darkAlpha, offFadeTime).SetLink(gameObject);
    }

    private void KillTween()
    {
        if (lightTween != null) lightTween.Kill();
        lightTween = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = overlay.color;
        c.a = alpha;
        overlay.color = c;
    }
    #endregion
}
