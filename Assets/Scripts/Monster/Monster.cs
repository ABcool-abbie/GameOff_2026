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
    [SerializeField] private GameObject goBehaviour0;
    [SerializeField] private GameObject goBehaviour1;
    [SerializeField] private GameObject goBehaviour2;

    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private bool _fed = false;
    private BehaviourConfig _behaviour;
    private GameProgress _game;
    private TankState _tank;

    public MonsterState State { get; private set; }
    public TankState Tank => _tank;
    public float TimeLeftRatio { get; private set; } = 1f;


    // BBR: for the monster book
    public Behaviour BehaviourType => _behaviour.behaviour;
    public static event Action<Monster> Encountered;
    private bool _encounterReported;

    // BBR: for the monster lamp
    public bool IsSolving => _solved && State == MonsterState.Wondering && _solvedTime > 0f;
    public float SolveProgress => 1f - _solvedTime / SOLVING_TIME;

    public const float SOLVING_TIME = 3f;

    private Vector2 _escapeVel;
    private float _startTime = -1f;
    private bool _solved;
    private float _solvedTime = SOLVING_TIME;
    private static GameProgressConfig.RoomType _roomType;
    private int _idxBehaviour;
    private MonsterConfig _config;

    public event Action OnHurt;
    public event Action OnDead;

    internal static Monster Spawn(GameProgress gameProgress, MonsterConfig monster, int idxBehaviour = -1)
    {
        var instance = GameObject.Instantiate(monster.prefab.gameObject).GetComponent<Monster>();

        instance._idxBehaviour = idxBehaviour >= 0 ? idxBehaviour : UnityEngine.Random.Range(0, monster.behaviours.Length);
        instance._behaviour = monster.behaviours[instance._idxBehaviour];

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

        _roomType = targetRoom.roomType;
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

        tankInstance.MonsterCount += 1;
        gameProgress.FireMonsterCountChangeEvent(_roomType, tankInstance.MonsterCount);

        instance._config = monster;
        instance._tank.SetBehaviour(monster, instance._idxBehaviour, true);

        instance.goBehaviour0.Unwrap()?.SetActive(instance._idxBehaviour == 0);
        instance.goBehaviour1.Unwrap()?.SetActive(instance._idxBehaviour == 1);
        instance.goBehaviour2.Unwrap()?.SetActive(instance._idxBehaviour == 2);

        return instance;
    }

    void OnDisable()
    {
        _tank.SetBehaviour(_config, _idxBehaviour, false);
    }

    void SetStateEscape()
    {
        // BBR: tell the monster book once
        if (!_encounterReported)
        {
            _encounterReported = true;
            Encountered?.Invoke(this);
        }
        // BBR: that player been via the event! 

        State = MonsterState.Escape;
        _escapeVel = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;

        goBehaviour0.Unwrap()?.GetComponent<ParticleSystem>().Stop();
        goBehaviour1.Unwrap()?.GetComponent<ParticleSystem>().Stop();
        goBehaviour2.Unwrap()?.GetComponent<ParticleSystem>().Stop();

        spriteRenderer.DOFade(0f, 1f).OnComplete(() =>
        {
            _tank.MonsterCount -= 1;
            _game.FireMonsterCountChangeEvent(_roomType, _tank.MonsterCount);
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
        if (!gameObject.activeInHierarchy || State == MonsterState.Escaped)
            return;

        if (_solved && _solvedTime > 0f)
        {
            _solvedTime -= Time.deltaTime;
            if (_solvedTime <= 0f)
            {
                _solvedTime = 0f;
                _game.OnSolveEvent(_config, _idxBehaviour);
                SetStateEscape();
            }
        }
        else if (!_solved)
        {
            _solvedTime += Time.deltaTime;
            _solvedTime = Mathf.Min(_solvedTime, SOLVING_TIME);
        }

        if (State == MonsterState.Wondering)
        {
            //BBR: added for the monster lamp blink blink
            if (_startTime < 0f) _startTime = time;
            float duration = eventTime - _startTime;
            TimeLeftRatio = duration > 0f ? Mathf.Clamp01((eventTime - time) / duration) : 0f;

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
        if (!gameObject.activeInHierarchy)
            return;
        int solveCount = 0;
        int totalSolveCount = 0;
        foreach (var solve in _behaviour.solveMethod)
        {
            switch (solve.type)
            {
                case EventType.Temp:
                    if (solve.param0 == (int)tankState.Temperature + 2)
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
        }

        //BBR: old one only set to true/wont back to false if change setting within 3s
        //if (solveCount == totalSolveCount)
        //{
        //    _solved = true;
        //}
        _solved = solveCount == totalSolveCount;
        //BBR: keep it in sync both way
    }

    private void OnFeed(TankState state)
    {
        _fed = true;
    }
}