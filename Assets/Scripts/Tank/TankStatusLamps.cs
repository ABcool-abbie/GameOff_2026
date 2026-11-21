using UnityEngine;

public class TankStatusLamps : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private TankHealth[] tanks;
    [SerializeField] private MonitorLamp hpLamp;

    [Header("Health Lamp")]
    [SerializeField] private Color okColor = new Color(0.2f, 1f, 0.3f); 
    [SerializeField] private Color damageColor = new Color(1f, 0.55f, 0f);
    [SerializeField] private float damageBlinkSlowest = 0.6f;
    [SerializeField] private float damageBlinkFastest = 0.08f;
    [SerializeField] private float healBlink = 0.5f;

    #endregion

    private void Update()
    {
        int index = monitorManager.CurrentCameraIndex;
        if (index < 0) return;

        UpdateHealthLamp(tanks[index]);
    }

    private void UpdateHealthLamp(TankHealth health)
    {
        if (health == null || hpLamp == null) return;

        int delta = health.GetDeltaPerTick();

        if (delta < 0)
        {
            float ratio = (float)health.Hp / health.MaxHp;
            float interval = Mathf.Lerp(damageBlinkFastest, damageBlinkSlowest, ratio);
            hpLamp.SetBlink(damageColor, interval);
        }
        else if (delta > 0 && health.Hp < health.MaxHp)
        {
            hpLamp.SetBlink(okColor, healBlink);
        }
        else
        {
            hpLamp.SetSteady(okColor);
        }
    }
}

