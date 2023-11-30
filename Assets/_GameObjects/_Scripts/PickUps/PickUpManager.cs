using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUpManager : MonoBehaviour
{
    [Header("Pick up Data")]
    [SerializeField] private List<PickUpType> activePickUpType;
    [SerializeField] private int minPickUps;
    [SerializeField] private int maxPickUps;
    [SerializeField] private int currentPickUpsAmt;

    [Header("Spawn Data")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<PickUp> pickups;

    public static Action SetUpPickUps;
    public static Action<List<Transform>> SetUpSpawnPoints;

    private void OnEnable()
    {
        SetUpPickUps += SetUp;
        SetUpSpawnPoints += OnSetUpSpawnPoints;
    }

    private void OnDisable()
    {
        SetUpPickUps -= SetUp;
        SetUpSpawnPoints -= OnSetUpSpawnPoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region SetUp
    private void SetUp()
    {
        currentPickUpsAmt = Random.Range(minPickUps, maxPickUps);
        SpawnPickUps();
    }

    private void OnSetUpSpawnPoints(List<Transform> points)
    {
        if (spawnPoints == null)
        {
            spawnPoints = new List<Transform>();
        }

        spawnPoints.AddRange(points);
    }
    #endregion

    private void SpawnPickUps()
    {
        if(pickups == null)
        {
            pickups = new List<PickUp>();
        }
        else
        {
            foreach (var item in pickups)
            {
                item.gameObject.SetActive(false);
            }

            pickups.Clear();
        }

        List<Transform> tempPoints = new List<Transform>();

        foreach (var item in spawnPoints)
        {
            tempPoints.Add(item);
        }

        for (int i = 0; i < currentPickUpsAmt; i++)
        {
            int randPointIndex = Random.Range(0, tempPoints.Count);

            GameObject obj = ObjectPooler.Instance.SpawnFormPool(activePickUpType[Random.Range(0, activePickUpType.Count)].ToString(), tempPoints[randPointIndex].position, Quaternion.Euler(0, Random.Range(0, 360), 0));

            pickups.Add(obj.GetComponent<PickUp>());

            tempPoints.RemoveAt(randPointIndex);
        }
    }
}
