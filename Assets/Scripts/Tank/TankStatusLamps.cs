using UnityEngine;

public class TankStatusLamps : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private TankHealth[] tanks;
    [SerializeField] private MonitorLamp hpLamp;
    [SerializeField] private MonitorLamp monsterLamp;

    [Header("Health Lamp")]
    [SerializeField] private Color okColor = new Color(0.2f, 1f, 0.3f); 
    [SerializeField] private Color damageColor = new Color(1f, 0.55f, 0f);
    [SerializeField] private float damageBlinkSlowest = 0.6f;
    [SerializeField] private float damageBlinkFastest = 0.08f;
    [SerializeField] private float healBlink = 0.5f;

    [Header("Anomaly Lamp")]
    [SerializeField] private Color clearColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color monsterColor = Color.red;
    [SerializeField] private float monsterBlinkSlowest = 0.5f;
    [SerializeField] private float monsterBlinkFastest = 0.06f;

    #endregion
    private TankState[] tankStates;

    private void Awake()
    {
        tankStates = new TankState[tanks.Length];
        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i] != null) tankStates[i] = tanks[i].GetComponent<TankState>();
        }
    }

    private void Update()
    {
        int index = monitorManager.CurrentCameraIndex;
        if (index < 0) return;

        UpdateHealthLamp(tanks[index]);
        UpdateMonsterLamp(tankStates[index]);
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

    private void UpdateMonsterLamp(TankState tank)
    {
        if (monsterLamp == null) return;
        Monster monster = FindUrgentMonster(tank);
        if (monster == null)
        {
            monsterLamp.SetSteady(clearColor);
            return;
        }

        float interval = Mathf.Lerp(monsterBlinkFastest, monsterBlinkSlowest, monster.TimeLeftRatio);
        monsterLamp.SetBlink(monsterColor, interval);

    }

    private Monster FindUrgentMonster(TankState tank)
    {
        if (tank == null) return null;

        Monster urgent = null;
        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        foreach (Monster m in monsters)
        {
            if (m.Tank != tank) continue;
            if (m.State != MonsterState.Wondering) continue;

            if (urgent == null || m.TimeLeftRatio < urgent.TimeLeftRatio)
                urgent = m;
        }

        return urgent;
    }

}

