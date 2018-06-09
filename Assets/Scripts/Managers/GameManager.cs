using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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


    private TransformAccessArray transforms;
    private MovementJob moveJob;
    private JobHandle moveHandle;

    #endregion




    #region Init

    // ===== Singleton =====
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        transforms = new TransformAccessArray(0);
    }

    void OnDisable()
    {
        moveHandle.Complete();
        transforms.Dispose();
    }

    #endregion


    #region Update

    void Update()
    {
        moveHandle.Complete();

        SpawnInput();

        SetupJobs();

        moveHandle = moveJob.Schedule(transforms);

        JobHandle.ScheduleBatchedJobs();
    }


    private void SpawnInput()
    {
        if (Input.GetKey(KeyCode.Space))
            SpawnEnemies(500);
    }

    private void SetupJobs()
    {
        moveJob = new MovementJob
        {
            moveSpeed = this.enemySpeed,
            topBound = this.topBound,
            buttonBound = this.bottomBound,
            deltaTime = Time.deltaTime,
        };
    }

    #endregion



    #region Private Methods

    private void SpawnEnemies(int count)
    {
        moveHandle.Complete();

        transforms.capacity = transforms.length + count;

        for (int i = 0; i < count; i++)
        {
            var enemy = CreateEnemy();
            transforms.Add(enemy.transform);
        }

        EnemiesCount += count;
    }

    private GameObject CreateEnemy()
    {
        float xVal = Random.Range(leftBound, rightBound);
        float yVal = Random.Range(0f, 10f);

        var position = new Vector3(xVal, topBound + yVal);
        var rotation = Quaternion.Euler(90f, 180f, 0f);

        return Instantiate(enemyPrefab, position, rotation); 
    }

    #endregion
}
