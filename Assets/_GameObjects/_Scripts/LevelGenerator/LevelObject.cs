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

    [Header("Connecting Points")]
    [SerializeField] private List<ObjectConnector> connectors;

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
        transform.rotation = Quaternion.Euler(originalRotation);

        this.gridNode = gridNode;
    }

    public void UpdateGridNode(GridNode gridNode)
    {
        this.gridNode = gridNode;
    }
}
