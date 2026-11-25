using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TankEventConfigs))]
public class TankEventConfigs : ScriptableObject
{
    [Serializable]
    public struct TankEventConfig
    {
        public MonsterConfig monsterConfig;
        public int idxBehaviour;
        public GameObject behaviourPrefab;
    }

    public TankEventConfig[] Configs;
}