using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _freq = 0.1f;
    [SerializeField] private float _speed = 0.01f;
    private BehaviourConfig _bahrivour;
    private TankState _tank;

    internal static Monster Spawn(GameProgress gameProgress, MonsterConfig monster)
    {
        var instance = GameObject.Instantiate(monster.prefab.gameObject).GetComponent<Monster>();
        int idx = UnityEngine.Random.Range(0, monster.behaviours.Length);
        instance._bahrivour = monster.behaviours[idx];

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

        return instance;
    }

    internal void Event()
    {
        // throw new NotImplementedException();
    }

    internal void Tick()
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
}