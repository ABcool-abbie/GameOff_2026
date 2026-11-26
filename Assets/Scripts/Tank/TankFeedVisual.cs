using UnityEngine;

public class TankFeedVisual : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private TankState tank;
    [SerializeField] private ParticleSystem feedfood;
    #endregion


    private void Awake()
    {
        //tank = GetComponent<TankState>(); DAMN!AGAIN!
        if (tank == null) tank = GetComponentInParent<TankState>();
        if (feedfood == null) feedfood = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (tank != null) tank.Fed += OnFed;
    }

    private void OnDisable()
    {
        if (tank != null) tank.Fed -= OnFed;
    }

    private void OnFed(TankState state)
    {
        if (feedfood != null) feedfood.Play();
    }

}
