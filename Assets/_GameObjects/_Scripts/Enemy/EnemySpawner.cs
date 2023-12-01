using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Wave Data")]
    [SerializeField] private int currentWaveNumber;
    [SerializeField] private int minEnemiesAmt;
    [SerializeField] private int maxEnemiesAmt;
    [SerializeField] private int enemiesAmt;

    [Header("Next Wave Delay")]
    [SerializeField] private bool watingForNewWave;
    [SerializeField] private float nextWaveDelay;
    [SerializeField] private float timeElapsedSinceWaveCompletion;

    [Header("Spawn Data")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<Enemy> enemies;

    public static Action StartWaves;
    public static Action<Enemy> RemoveEnemy;
    public static Action<List<Transform>> SetUpSpawnPoints;
    public static Action NewWaveStarted;

    private void OnEnable()
    {
        StartWaves += SetUp;
        SetUpSpawnPoints += OnSetUpSpawnPoints;
        RemoveEnemy += RemoveEnemyOnDeath;
    }

    private void OnDisable()
    {
        StartWaves -= SetUp;
        SetUpSpawnPoints -= OnSetUpSpawnPoints;
        RemoveEnemy -= RemoveEnemyOnDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WaitForNextWave();
    }

    #region SetUp
    private void SetUp()
    {
        timeElapsedSinceWaveCompletion = 0;
        watingForNewWave = false;

        StartNewWave();
    }

    private void OnSetUpSpawnPoints(List<Transform> points)
    {
        if(spawnPoints == null)
        {
            spawnPoints = new List<Transform>();
        }

        spawnPoints.AddRange(points);
    }
    #endregion

    #region Waves
    private void StartNewWave()
    {
        timeElapsedSinceWaveCompletion = 0;
        watingForNewWave = false;

        currentWaveNumber++;

        enemiesAmt = Random.Range(minEnemiesAmt, maxEnemiesAmt);

        SpawnEnemies();

        PickUpManager.SetUpPickUps?.Invoke();

        NewWaveStarted?.Invoke();

        GameplayMenu.SetWaveActive?.Invoke(true);
        GameplayMenu.UpdateWaveTxt?.Invoke(currentWaveNumber);
        GameplayMenu.UpdateEnemiesTxt?.Invoke(enemies.Count, enemiesAmt);
    }

    private void SpawnEnemies()
    {
        if(enemies == null)
        {
            enemies = new List<Enemy>();
        }
        else
        {
            foreach(Enemy enemy in enemies)
            {
                enemy.gameObject.SetActive(false);
            }

            enemies.Clear();
        }

        List<Transform> tempPoints = new List<Transform>();

        foreach (var item in spawnPoints)
        {
            tempPoints.Add(item);
        }

        for (int i = 0; i < enemiesAmt; i++)
        {
            int randPointIndex = Random.Range(0, tempPoints.Count);

            GameObject obj = ObjectPooler.Instance.SpawnFormPool("Ground Enemy", tempPoints[randPointIndex].position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            obj.GetComponent<Enemy>().SetUp(player);

            enemies.Add(obj.GetComponent<Enemy>());

            tempPoints.RemoveAt(randPointIndex);
        }
    }

    private void RemoveEnemyOnDeath(Enemy enemy)
    {
        if(enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }

        SetEnemiesToChasePlayerIfLowEnemies();

        if (enemies.Count == 0)
        {
            timeElapsedSinceWaveCompletion = 0;
            watingForNewWave = true;

            GameplayMenu.SetWaveActive?.Invoke(false);
        }

        GameplayMenu.UpdateEnemiesTxt?.Invoke(enemies.Count, enemiesAmt);
    }

    private void SetEnemiesToChasePlayerIfLowEnemies()
    {
        if(enemies.Count <= enemiesAmt / 4)
        {
            foreach (var item in enemies)
            {
                item.SetChasePlayer(true);
            }
        }
    }
    #endregion

    #region Next wave wait
    private void WaitForNextWave()
    {
        if(watingForNewWave)
        {
            timeElapsedSinceWaveCompletion += Time.deltaTime;

            if(timeElapsedSinceWaveCompletion > nextWaveDelay)
            {
                timeElapsedSinceWaveCompletion = 0;
                watingForNewWave = false;

                StartNewWave();
            }

            GameplayMenu.UpdateNextWaveTimerTxt?.Invoke((int)(nextWaveDelay - timeElapsedSinceWaveCompletion));
        }
    }
    #endregion
}
