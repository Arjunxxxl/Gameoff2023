using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour
{
    [Header("Object Type")]
    [SerializeField] private LevelObjectType levelObjectType;
    [SerializeField] private GridNode gridNode;

    [Header("Rotation")]
    [SerializeField] private Vector3 originalRotation;
    [SerializeField] private bool randomRotation;

    [Header("Connecting Points")]
    [SerializeField] private List<ObjectConnector> connectors;

    [Header("Spawn Point")]
    [SerializeField] private GameObject spawnPointParent;
    [SerializeField] private GameObject pickUpPointParent;

    public List<ObjectConnector> Connectors {  get { return connectors; } }
     
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(GridNode gridNode)
    {
        if (randomRotation)
        {
            transform.rotation = Quaternion.Euler(0, Random.Range(0, 5) * 90, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(originalRotation);
        }

        this.gridNode = gridNode;

        SetUpSpawnPoints();
        SetUpPickUpPoints();
    }

    public void UpdateGridNode(GridNode gridNode)
    {
        this.gridNode = gridNode;
    }

    private void SetUpSpawnPoints()
    {
        List<Transform> points = new List<Transform>();

        for (int i = 0; i < spawnPointParent.transform.childCount; i++)
        {
            points.Add(spawnPointParent.transform.GetChild(i).transform);
        }

        EnemySpawner.SetUpSpawnPoints(points);
    }

    private void SetUpPickUpPoints()
    {
        if (pickUpPointParent != null)
        {
            List<Transform> points = new List<Transform>();

            for (int i = 0; i < pickUpPointParent.transform.childCount; i++)
            {
                points.Add(pickUpPointParent.transform.GetChild(i).transform);
            }

            PickUpManager.SetUpSpawnPoints(points);
        }
    }
}
