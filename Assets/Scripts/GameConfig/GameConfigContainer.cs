using System.Collections.Generic;
using UnityEngine;
using Utility;

public class GameConfigContainer : SingletonBehaviour<GameConfigContainer>
{
    public GameProgressConfig Config => config;


    [SerializeField] GameProgressConfig config;

    [SerializeField] MonsterConfig[] monsterConfigs;
    public IReadOnlyList<MonsterConfig> MonsterConfigs => monsterConfigs;
}
