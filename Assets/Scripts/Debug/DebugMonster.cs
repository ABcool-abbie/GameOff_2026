using System.Collections.Generic;
using UnityEngine;

public class DebugMonster : MonoBehaviour
{
    [SerializeField] private GameProgress game;
    [SerializeField] private MonsterConfig monster;
    [SerializeField] private int behaviour;

    [ContextMenu("Spawn")]
    private void Spawn()
    {
        if (game == null || monster == null) return;
        Monster.Spawn(game, monster, behaviour);
    }
}
