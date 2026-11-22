using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public enum MonsterState
{
    Wondering,
    Escape,
    Escaped,
}

public class Monster : MonoBehaviour
{
    [SerializeField] private float _freq = 0.1f;
    [SerializeField] private float _speed = 0.01f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro textName;
    [SerializeField] private TextMeshPro textBehaviour;
    private bool _fed = false;
    private BehaviourConfig _behaviour;
    private GameProgress _game;
    private TankState _tank;

    public MonsterState State { get; private set; }

    private Vector2 _escapeVel;

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

        instance.textName.text = monster.name;
        instance.textBehaviour.text = instance._behaviour.behaviour.ToString();

        return instance;
    }

    void SetStateEscape()
    {
        State = MonsterState.Escape;
        _escapeVel = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        
        spriteRenderer.DOFade(0f, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    internal void AttackEvent()
    {
        if (_game.EventSource is GameEventSource src)
            src.DropSan(_behaviour.damage);
        SetStateEscape();
    }

    internal void Tick(float time, float eventTime)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (State == MonsterState.Wondering)
        {
            if (time > eventTime)
                AttackEvent();
            else
                TickWondering();
        }
        else if (State == MonsterState.Escape)
        {
            TickEscape();
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
        var center = 0.5f * (_tank.BoundsMin + _tank.BoundsMax);
        vel += center * 0.1f;
        vel = Vector2.ClampMagnitude(vel, 1f);
        pos += vel * _speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, _tank.BoundsMin.x, _tank.BoundsMax.x);
        pos.y = Mathf.Clamp(pos.y, _tank.BoundsMin.y, _tank.BoundsMax.y);
        SetPos(pos);
    }

    void TickEscape()
    {
        Vector2 pos = transform.position;
        pos += _escapeVel * _speed * Time.deltaTime;
        SetPos(pos);
    }

    void SetPos(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    void OnTankStateChanged(TankState tankState)
    {
        int solveCount = 0;
        int totalSolveCount = 0;
        foreach (var solve in _behaviour.solveMethod)
        {
            switch (solve.type)
            {
                case EventType.Temp:
                    if (Mathf.Abs(solve.param0 - (int)tankState.Temperature + 2) > 1)
                        solveCount++;
                    totalSolveCount++;
                    break;
                case EventType.Light:
                    if (solve.param0 > 0 == tankState.LightOn)
                        solveCount++;
                    totalSolveCount++;
                    break;
                case EventType.Feed:
                    if (solve.param0 > 0 == _fed)
                        solveCount++;
                    totalSolveCount++;
                    break;
                case EventType.Music:
                    if (solve.param0 > 0 == tankState.SoundOn)
                        solveCount++;
                    totalSolveCount++;
                    break;
                case EventType.Oxygen:
                    if (solve.param0 == (int)tankState.Oxygen + 1)
                        solveCount++;
                    totalSolveCount++;
                    break;
                case EventType.View:
                    if (solve.param0 > 0 == tankState.Viewing)
                        solveCount++;
                    totalSolveCount++;
                    break;
            }
            if (solveCount == totalSolveCount)
            {
                SetStateEscape();
            }
        }
    }

    private void OnFeed(TankState state)
    {
        _fed = true;
    }
}