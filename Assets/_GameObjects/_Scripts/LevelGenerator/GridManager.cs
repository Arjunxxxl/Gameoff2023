using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Data")]
    [SerializeField] private Vector3Int gridSize;
    [SerializeField] private Vector3 gridCellSize;

    [Header("Grid Nodes Data")]
    [SerializeField] private GridNode[,,] gridNodes;
    [SerializeField] private List<GridNode> gridNodesLst;

    public Vector3Int GridSize { get { return gridSize; } }
    public Vector3 GridCellSize { get { return gridCellSize; } }
    public GridNode[,,] GridNodes { get { return gridNodes; } }
    public List<GridNode> GridNodesLst { get { return gridNodesLst; } }

    #region SingleTon
    public static GridManager Instance;
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        CreateGrid();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    #region Create Grid
    private void CreateGrid()
    {
        gridNodes = new GridNode[gridSize.x, gridSize.y, gridSize.z];
        gridNodesLst = new List<GridNode>();

        float xStart = -(gridSize.x - 1) * gridCellSize.x * 0.5f;
        float yStart = 0;// -(gridSize.y - 1) * gridCellSize.y * 0.5f;
        float zStart = -(gridSize.z - 1) * gridCellSize.z * 0.5f;

        for (int x = 0; x < gridSize.x; x++)
        {
            float xPos = xStart + x * gridCellSize.x;
            for (int y = 0; y < gridSize.y; y++)
            {
                float yPos = yStart + y * gridCellSize.y;
                for (int z = 0; z < gridSize.z; z++)
                {
                    float zPos = zStart + z * gridCellSize.z;

                    GridNode gridNode = new GridNode();
                    gridNode.gridCoord = new Vector3Int(x, y, z);
                    gridNode.gridPos = new Vector3(xPos, yPos, zPos);
                    gridNode.levelObject = null;
                    gridNode.possibleLevelObjectType = new List<LevelObjectType>();

                    for (int i = 0; i < System.Enum.GetValues(typeof(LevelObjectType)).Length; i++)
                    {
                        gridNode.possibleLevelObjectType.Add((LevelObjectType)i);
                    }

                    gridNodes[x, y, z] = gridNode;
                    gridNodesLst.Add(gridNode);
                }
            }
        }

        SetUpNeighbours();
    }

    private void SetUpNeighbours()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    List<Vector3Int> neighboursCoord = new List<Vector3Int>();

                    GridNode gridNode = gridNodes[x, y, z];
                    gridNode.neighboursCoord = neighboursCoord;

                    Vector3Int left = gridNode.gridCoord;
                    Vector3Int right = gridNode.gridCoord;
                    Vector3Int down = gridNode.gridCoord;
                    Vector3Int up = gridNode.gridCoord;
                    Vector3Int backward = gridNode.gridCoord;
                    Vector3Int forward = gridNode.gridCoord;

                    left.x -= 1;
                    right.x += 1;
                    down.y -= 1;
                    up.y += 1;
                    backward.z -= 1;
                    forward.z += 1;

                    if(left.x >= 0)
                    {
                        neighboursCoord.Add(left);
                    }

                    if(right.x < gridSize.x)
                    {
                        neighboursCoord.Add(right);
                    }

                    if(down.y >= 0)
                    {
                        neighboursCoord.Add(down);
                    }

                    if (up.y < gridSize.y)
                    {
                        neighboursCoord.Add(up);
                    }

                    if(backward.z >= 0)
                    {
                        neighboursCoord.Add(backward);
                    }

                    if (forward.z < gridSize.z)
                    {
                        neighboursCoord.Add(forward);
                    }
                }
            }
        }
    }
    #endregion

    public GridNode GetGridNode(Vector3Int gridCoord)
    {
        return gridNodes[gridCoord.x, gridCoord.y, gridCoord.z];
    }
}
