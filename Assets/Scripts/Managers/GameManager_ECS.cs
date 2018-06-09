using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class GameManager_ECS : MonoBehaviour {

    #region Fields

    [Header("UI")]
    public Text countText;

    [Header("Enemy")]
    public float enemySpeed = 5f;
    public GameObject enemyPrefab;

    [Header("Bounds")]
    public float leftBound = 0;
    public float topBound = 0;
    public float rightBound = 0;
    public float bottomBound = 0;

    private int _enemiesCount;
    private int EnemiesCount
    {
        get => _enemiesCount;
        set
        {
            _enemiesCount = value;
            countText.text = _enemiesCount.ToString();
        }
    }


    private EntityManager _manager;

    #endregion




    #region Init

    // ===== Singleton =====
    public static GameManager_ECS Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        _manager = World.Active.GetOrCreateManager<EntityManager>();
    }


    #endregion


    #region Update

    void Update()
    {
        SpawnInput();
    }


    private void SpawnInput()
    {
        if (Input.GetKey(KeyCode.Space))
            SpawnEnemies(500);
    }


    #endregion


    #region Private Methods

    private void SpawnEnemies(int count)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        _manager.Instantiate(enemyPrefab, entities);

        for (int index = 0; index < count; index++)
        {
            float xVal = Random.Range(leftBound, rightBound);
            float yVal = Random.Range(0f, 10f);

            _manager.SetComponentData(entities[index], 
                new Position { Value = new float3(xVal, 0, topBound + yVal) });
            _manager.SetComponentData(entities[index],
                new Rotation { Value = new quaternion(0, 1, 0, 0) });
            _manager.SetComponentData(entities[index],
                new MoveSpeed { speed = enemySpeed });
        }
        entities.Dispose();

        EnemiesCount += count;
    }

    #endregion
}
