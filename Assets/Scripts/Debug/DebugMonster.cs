using System.Collections.Generic;
using UnityEngine;

public class DebugMonster : MonoBehaviour
{
    [SerializeField] private GameProgress game;
    [SerializeField] private MonsterConfig[] monsters;
    [SerializeField] private Rect area = new Rect(160, 10, 240, 100);

    private int monsterIndex;
    private int behaviour;
    private readonly List<Monster> spawned = new List<Monster>();

    private void OnGUI()
    {
        if (monsters == null || monsters.Length == 0) return;
        MonsterConfig monster = monsters[monsterIndex];
        int count = (monster != null && monster.behaviours != null) ? monster.behaviours.Length : 0;

        GUILayout.BeginArea(area, GUI.skin.box);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(30))) { monsterIndex = (monsterIndex - 1 + monsters.Length) % monsters.Length; behaviour = 0; }
        GUILayout.Label(monster != null ? monster.name : "(empty)");
        if (GUILayout.Button(">", GUILayout.Width(30))) { monsterIndex = (monsterIndex + 1) % monsters.Length; behaviour = 0; }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(30)) && count > 0) behaviour = (behaviour - 1 + count) % count;
        GUILayout.Label(count > 0 ? behaviour + ": " + monster.behaviours[behaviour].behaviour : "-");
        if (GUILayout.Button(">", GUILayout.Width(30)) && count > 0) behaviour = (behaviour + 1) % count;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn")) Spawn(monster);
        if (GUILayout.Button("Cancel (" + spawned.Count + ")")) CancelAll();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }


    [ContextMenu("Spawn")]
    private void SpawnFromMenu()
    {
        if (monsters != null && monsters.Length > 0) Spawn(monsters[monsterIndex]);
    }

    private void Spawn(MonsterConfig monster)
    {
        if (game == null || monster == null) return;
        Monster m = Monster.Spawn(game, monster, behaviour);
        if (m != null) spawned.Add(m);
    }

    [ContextMenu("Cancel All")]
    private void CancelAll()
    {
        foreach (Monster m in spawned)
        {
            if (m == null) continue;
            if (m.Tank != null) m.Tank.MonsterCount -= 1;
            Destroy(m.gameObject);
        }
        spawned.Clear();
    }

}
