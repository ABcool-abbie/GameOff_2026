using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public enum MonsterState
{
    Wondering,
    Escape,
}

public class Monster : MonoBehaviour
{
    [SerializeField] private float _freq = 0.1f;
    [SerializeField] private float _speed = 0.01f;
    private int _hp = 10;
    private bool _hurt = false;
    private float _hurtCooldown = 0f;
    private bool _fed = false;
    private BehaviourConfig _behaviour;
    private GameProgress _game;
    private TankState _tank;

    public MonsterState State {get; private set;}

    public event Action OnHurt;
    public event Action OnDead;

    internal static Monster Spawn(GameProgress gameProgress, MonsterConfig monster)
    {
        var instance = GameObject.Instantiate(monster.prefab.gameObject).GetComponent<Monster>();
        int idx = UnityEngine.Random.Range(0, monster.behaviours.Length);
        instance._behaviour = monster.behaviours[idx];

        instance._game = gameProgress;

        var list = ListPool<GameProgressConfig.RoomConfig>.Get();
        foreach (GameProgressConfig.RoomConfig room in GameConfigContainer.Instance.Config.roomConfigs)
        {
            room.monsterConfigs.Contains(monster);
            list.Add(room);
        }
        var targetIdx = UnityEngine.Random.Range(0, list.Count);
        var targetRoom = list[targetIdx];
        ListPool<GameProgressConfig.RoomConfig>.Release(list);

        var tankInstance = gameProgress.GetTank(targetRoom.roomType);
        Vector2 pos = default;
        pos.x = UnityEngine.Random.Range(tankInstance.BoundsMin.x, tankInstance.BoundsMax.x);
        pos.y = UnityEngine.Random.Range(tankInstance.BoundsMin.y, tankInstance.BoundsMax.y);
        instance.SetPos(pos);
        instance._tank = tankInstance;

        instance._tank.Changed += instance.OnTankStateChanged;
        instance._tank.Fed += instance.OnFeed;

        return instance;
    }

    internal void AttackEvent()
    {
        bool requireFeed = false;
        foreach(var solve in _behaviour.solveMethod)
        {
            if (solve.type == EventType.Feed)
            {
                requireFeed = true;
                break;
            }
        }

        {
            if (_game.EventSource is GameEventSource src)
                src.DropSan(_behaviour.damage);
        }
    }

    internal void Tick()
    {
        const float HURT_COOLDOWN = 1f;
        const float ATK_COOLDOWN = 5f;

        if (_hurt && _hurtCooldown >= 0f && _hp > 0)
        {
            _hurtCooldown -= Time.deltaTime;
            if (_hurtCooldown < 0f)
            {
                _hurtCooldown += HURT_COOLDOWN;
                _hp = Mathf.Max(0, _hp - 1);
                if (_hp <= 0)
                    OnDead?.Invoke();
                else
                    OnHurt?.Invoke();
            }
        }

        if (State == MonsterState.Wondering)
        {
            TickWondering();
        }
        else if (State == MonsterState.Escape)
        {
        }
    }

    void TickWondering()
    {
        Vector2 pos = transform.position;
        int hash = GetEntityId().GetHashCode();
        float rand01 = hash % 1000 / 1000f;
        float phaseShift = Mathf.PI * 2f * rand01;
        var freq = _freq * Time.time;
        var vel = new Vector2(
            Mathf.Sin(5f * freq + phaseShift) + 0.8f * Mathf.Sin(11f * freq + phaseShift),
            Mathf.Sin(7f * freq + phaseShift) + 0.8f * Mathf.Sin(13f * freq + phaseShift));
        vel = Vector2.ClampMagnitude(vel, 1f);
        pos += vel * _speed;
        pos.x = Mathf.Clamp(pos.x, _tank.BoundsMin.x, _tank.BoundsMax.x);
        pos.y = Mathf.Clamp(pos.y, _tank.BoundsMin.y, _tank.BoundsMax.y);
        SetPos(pos);
    }

    void SetPos(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    void OnTankStateChanged(TankState tankState)
    {
        int hurtCount = 0;
        int solveCount = 0;
        foreach (var solve in _behaviour.solveMethod)
        {
            switch (solve.type)
            {
                case EventType.Temp:
                    if (Mathf.Abs(solve.param0 - (int)tankState.Temperature + 2) > 1)
                        hurtCount++;
                    solveCount++;
                    break;
                case EventType.Light:
                    if (solve.param0 > 0 == tankState.LightOn)
                        hurtCount++;
                    solveCount++;
                    break;
                case EventType.Music:
                    if (solve.param0 > 0 == tankState.SoundOn)
                        hurtCount++;
                    solveCount++;
                    break;
                case EventType.Oxygen:
                    if (solve.param0 == (int)tankState.Oxygen + 1)
                        hurtCount++;
                    solveCount++;
                    break;
                case EventType.View:
                    if (solve.param0 > 0 == tankState.Viewing)
                        hurtCount++;
                    solveCount++;
                    break;
            }
            _hurt = hurtCount == solveCount;
        }
    }

    private void OnFeed(TankState state)
    {
        _fed = true;
    }
}