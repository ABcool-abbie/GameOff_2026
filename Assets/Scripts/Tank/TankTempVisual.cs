using DG.Tweening;
using UnityEngine;


public class TankTempVisual : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private TankState tank;


    [SerializeField] private SpriteRenderer iceLayout;
    [SerializeField] private float coldAlpha = 0.5f;
    [SerializeField] private float veryColdAlpha = 1f;
    [SerializeField] private float coldTime = 1.5f;


    [SerializeField] private GameObject hotLayout;
    #endregion

    #region STATS
    private SpriteRenderer overlay;
    private TemperatureLevel tempLevel;
    private Tween coldTween;


    #endregion

    private void Awake()
    {
        overlay = GetComponent<SpriteRenderer>();

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

    private void OnTankChanged(TankState state)
    {
        if (state.Temperature == tempLevel) return;
        tempLevel = state.Temperature;

    }

    #region VISUALS
    private Tween PlayCold()
    {
        return overlay.DOFade(coldAlpha, coldTime).SetLink(gameObject); ;
    }

    private Tween PlayVeryCold()
    {
        return overlay.DOFade(veryColdAlpha, coldTime).SetLink(gameObject);
    }
    #endregion
}
