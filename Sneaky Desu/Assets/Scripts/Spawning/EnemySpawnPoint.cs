using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "eSpawn", menuName = "Enemy Spawn Point")]
public class EnemySpawnPoint : ScriptableObject
{
    public static EnemySpawnPoint spawnPoint;

    [Header("Prefab")]
    public Pawn enemy;

    [Header("Spawn Position")]
    public Vector2 position;

    [Header("Attributes")]
    public float health;
    public float reserve;
}

public class EnemySpawn : MonoBehaviour
{
    private void Start()
    {
        Instantiate(EnemySpawnPoint.spawnPoint.enemy.gameObject);
    }
}
