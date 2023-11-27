using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.AI.Navigation;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LevelGenerator : MonoBehaviour
{
    [Header("Randomizer")]
    [SerializeField] private int seed;

    [Header("Spawning Data")]
    [SerializeField] private List<LevelObject> spawnedLevelObjects;
    [SerializeField] private List<GameObject> levelBoundayObjs;
    [SerializeField] private bool isLevelSpawningTriggered;
    [SerializeField] private bool isLevelSpawned;

    [Header("Wave Function Collapse")]
    [SerializeField] private bool useCleanUp;
    [SerializeField] private List<GridNode> openNodes;
    [SerializeField] private List<GridNode> closedNodes;
    [SerializeField] private List<GridNode> collopsedNodes;
    [SerializeField] private bool isWaveFunctionCollapsed;

    [Header("Level Objects")]
    [SerializeField] private string levelObjectTag;
    [SerializeField] private int minObjectIndex;
    [SerializeField] private int maxObjectIndex;

    private NavMeshSurface navMeshSurface;

    private GridManager gridManager;
    private LevelObjectConnectionData levelObjectConnection;
    private ObjectPooler objectPooler;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = GridManager.Instance;
        levelObjectConnection = LevelObjectConnectionData.Instance;
        objectPooler = ObjectPooler.Instance;

        navMeshSurface = GetComponent<NavMeshSurface>();

        Random.InitState(seed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GenerateLevel();
        }

        if(isWaveFunctionCollapsed && !isLevelSpawningTriggered)
        {
            isLevelSpawningTriggered = true;
            isLevelSpawned = false;

            StartCoroutine(SpawneLevel());
        }
    }

    #region Wave function colapse
    private void GenerateLevel()
    {
        StartCoroutine(SetUpPossibleLevelObject());
    }

    private IEnumerator SetUpPossibleLevelObject()
    {
        isWaveFunctionCollapsed = false;

        collopsedNodes = new List<GridNode>();

        while (!IsAllGridNodesLevelObjectTypeSet())
        {
            openNodes = new List<GridNode>();
            closedNodes = new List<GridNode>();

            GridNode nodeToCollapse = GetGridNodeWithLowestEntropy();
            
            CollapseGridNode(nodeToCollapse);

            UpdateNeighbourOf(nodeToCollapse);

            yield return null;
        }

        if (useCleanUp)
        {
            yield return null;

            for (int i = 0; i < 2; i++)
            {
                CleanUp();

                yield return null;
            }
        }

        isWaveFunctionCollapsed = true;
    }

    private bool IsAllGridNodesLevelObjectTypeSet()
    {
        bool isSet = true;

        for (int x = 0; x < gridManager.GridSize.x; x++)
        {
            for (int y = 0; y < gridManager.GridSize.y; y++)
            {
                for (int z = 0; z < gridManager.GridSize.z; z++)
                {
                    if (gridManager.GetGridNode(new Vector3Int(x, y, z)).possibleLevelObjectType.Count != 1)
                    {
                        isSet = false;
                    }
                }
            }
        }

        return isSet;
    }

    private GridNode GetGridNodeWithLowestEntropy()
    {
        int lowestEntropy = System.Enum.GetValues(typeof(LevelObjectType)).Length;
        List<GridNode> gridNodes = new List<GridNode>();

        for (int i = 0; i < gridManager.GridNodesLst.Count; i++)
        {
            if (gridManager.GridNodesLst[i].possibleLevelObjectType.Count > 1 && gridManager.GridNodesLst[i].possibleLevelObjectType.Count < lowestEntropy)
            {
                lowestEntropy = gridManager.GridNodesLst[i].possibleLevelObjectType.Count;
            }
        }

        for (int i = 0; i < gridManager.GridNodesLst.Count; i++)
        {
            if (gridManager.GridNodesLst[i].possibleLevelObjectType.Count == lowestEntropy)
            {
                gridNodes.Add(gridManager.GridNodesLst[i]);
            }
        }

        return gridNodes[Random.Range(0, gridNodes.Count)];
    }

    private void CollapseGridNode(GridNode gridNode)
    {
        List<Vector3Int> neighboursCoord = gridNode.neighboursCoord;
        List<GridNode> neighbour = new List<GridNode>();

        for (int i = 0; i < neighboursCoord.Count; i++)
        {
            neighbour.Add(gridManager.GetGridNode(neighboursCoord[i]));
        }

        List<List<LevelObjectType>> levelObjectsBasedOnNeighbour = new List<List<LevelObjectType>>();
        List<LevelObjectType> finalLevelObjectType = new List<LevelObjectType>();
        
        for (int t = 0; t < System.Enum.GetValues(typeof(LevelObjectType)).Length; t++)
        {
            finalLevelObjectType.Add((LevelObjectType)t);
        }

        for (int i = 0; i < neighbour.Count; i++)
        {
            List<LevelObjectType> tempLevelObjectType = new List<LevelObjectType>();

            bool isRight = false;
            bool isLeft = false;
            bool isUp = false;
            bool isDown = false;
            bool isForward = false;
            bool isBack = false;

            if (gridNode.gridCoord.x != neighbour[i].gridCoord.x)
            {
                isRight = neighbour[i].gridCoord.x > gridNode.gridCoord.x;
                isLeft = neighbour[i].gridCoord.x < gridNode.gridCoord.x;
            }
            else if (gridNode.gridCoord.y != neighbour[i].gridCoord.y)
            {
                isUp = neighbour[i].gridCoord.y > gridNode.gridCoord.y;
                isDown = neighbour[i].gridCoord.y < gridNode.gridCoord.y;
            }
            else if (gridNode.gridCoord.z != neighbour[i].gridCoord.z)
            {
                isForward = neighbour[i].gridCoord.z > gridNode.gridCoord.z;
                isBack = neighbour[i].gridCoord.z < gridNode.gridCoord.z;
            }

            for (int j = 0; j < neighbour[i].possibleLevelObjectType.Count; j++)
            {
                ConnectionData cd = levelObjectConnection.GetAllPossibleConnection(neighbour[i].possibleLevelObjectType[j]);

                if (isRight)
                {
                    for (int k = 0; k < cd.possibleConnectionIn_left.Count; k++)
                    {
                        if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_left[k]))
                        {
                            tempLevelObjectType.Add(cd.possibleConnectionIn_left[k]);
                        }
                    }
                }
                else if (isLeft)
                {
                    for (int k = 0; k < cd.possibleConnectionIn_right.Count; k++)
                    {
                        if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_right[k]))
                        {
                            tempLevelObjectType.Add(cd.possibleConnectionIn_right[k]);
                        }
                    }
                }
                else if (isUp)
                {
                    for (int k = 0; k < cd.possibleConnectionIn_down.Count; k++)
                    {
                        if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_down[k]))
                        {
                            tempLevelObjectType.Add(cd.possibleConnectionIn_down[k]);
                        }
                    }
                }
                else if (isDown)
                {
                    for (int k = 0; k < cd.possibleConnectionIn_up.Count; k++)
                    {
                        if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_up[k]))
                        {
                            tempLevelObjectType.Add(cd.possibleConnectionIn_up[k]);
                        }
                    }
                }
                else if (isForward)
                {
                    for (int k = 0; k < cd.possibleConnectionIn_back.Count; k++)
                    {
                        if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_back[k]))
                        {
                            tempLevelObjectType.Add(cd.possibleConnectionIn_back[k]);
                        }
                    }
                }
                else if (isBack)
                {
                    for (int k = 0; k < cd.possibleConnectionIn_forward.Count; k++)
                    {
                        if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_forward[k]))
                        {
                            tempLevelObjectType.Add(cd.possibleConnectionIn_forward[k]);
                        }
                    }
                }
            }

            levelObjectsBasedOnNeighbour.Add(tempLevelObjectType);
        }

        for (int k = 0; k < levelObjectsBasedOnNeighbour.Count; k++)
        {
            List<LevelObjectType> tempObjectType = new List<LevelObjectType>();

            foreach (var item in finalLevelObjectType)
            {
                tempObjectType.Add(item);
            }

            var intersection = tempObjectType.Intersect(levelObjectsBasedOnNeighbour[k]);

            finalLevelObjectType.Clear();

            foreach (var item in intersection)
            {
                if (!finalLevelObjectType.Contains(item))
                {
                    finalLevelObjectType.Add(item);
                }
            }
        }

        LevelObjectType randLevelObjectType = finalLevelObjectType.Count > 0 ?
                                                finalLevelObjectType[Random.Range(0, finalLevelObjectType.Count)]
                                                : gridNode.possibleLevelObjectType[Random.Range(0, gridNode.possibleLevelObjectType.Count)];

        gridNode.possibleLevelObjectType.Clear();

        gridNode.possibleLevelObjectType.Add(randLevelObjectType);

        if(!collopsedNodes.Contains(gridNode))
        { 
            collopsedNodes.Add(gridNode); 
        }
    }

    private void UpdateNeighbourOf(GridNode gridNode)
    {
        List<Vector3Int> neighboursCoord = gridNode.neighboursCoord;

        List<GridNode> neighbour = new List<GridNode>();

        for (int i = 0; i < neighboursCoord.Count; i++)
        {
            neighbour.Add(gridManager.GetGridNode(neighboursCoord[i]));
        }

        for (int i = 0; i < neighbour.Count; i++)
        {
            if (neighbour[i].possibleLevelObjectType.Count == 1)
            {
                if (!collopsedNodes.Contains(neighbour[i]))
                {
                    collopsedNodes.Add(neighbour[i]);
                    continue;
                }
            }

            if(collopsedNodes.Contains(neighbour[i]))
            {
                continue;
            }

            if (!closedNodes.Contains(neighbour[i]) && !openNodes.Contains(neighbour[i]))
            {
                openNodes.Add(neighbour[i]);

                UpdateGridNodeLevelObjectType(neighbour[i]);

                UpdateNeighbourOf(neighbour[i]);
            }
        }
    }

    /*
     Done: modify this function
    neighbour and current node ke level object type ke common elements nhi nikalne
    neignbours ke levelobjectstype based on position kin kin objects type se connect kar skte h vo ek list me store kar lo
    yhe sare neighbours ke liye karna h
    and unke common elements nikalne h

    same check CollapseGridNode() me bhi karni h
     */
    private void UpdateGridNodeLevelObjectType(GridNode gridNode)
    {
        if (openNodes.Contains(gridNode))
        {
            List<Vector3Int> neighboursCoord = gridNode.neighboursCoord;
            List<GridNode> neighbour = new List<GridNode>();

            for (int i = 0; i < neighboursCoord.Count; i++)
            {
                neighbour.Add(gridManager.GetGridNode(neighboursCoord[i]));
            }

            List<List<LevelObjectType>> levelObjectsBasedOnNeighbour = new List<List<LevelObjectType>>();
            List<LevelObjectType> finalLevelObjectType = new List<LevelObjectType>();
            
            for (int t = 0; t < System.Enum.GetValues(typeof(LevelObjectType)).Length; t++)
            {
                finalLevelObjectType.Add((LevelObjectType)t);
            }

            for (int i = 0; i < neighbour.Count; i++)
            {
                List<LevelObjectType> tempLevelObjectType = new List<LevelObjectType>();

                bool isRight = false;
                bool isLeft = false;
                bool isUp = false;
                bool isDown = false;
                bool isForward = false;
                bool isBack = false;

                if (gridNode.gridCoord.x != neighbour[i].gridCoord.x)
                {
                    isRight = neighbour[i].gridCoord.x > gridNode.gridCoord.x;
                    isLeft = neighbour[i].gridCoord.x < gridNode.gridCoord.x;
                }
                else if (gridNode.gridCoord.y != neighbour[i].gridCoord.y)
                {
                    isUp = neighbour[i].gridCoord.y > gridNode.gridCoord.y;
                    isDown = neighbour[i].gridCoord.y < gridNode.gridCoord.y;
                }
                else if (gridNode.gridCoord.z != neighbour[i].gridCoord.z)
                {
                    isForward = neighbour[i].gridCoord.z > gridNode.gridCoord.z;
                    isBack = neighbour[i].gridCoord.z < gridNode.gridCoord.z;
                }

                for (int j = 0; j < neighbour[i].possibleLevelObjectType.Count; j++)
                {
                    ConnectionData cd = levelObjectConnection.GetAllPossibleConnection(neighbour[i].possibleLevelObjectType[j]);

                    if (isRight)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_left.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_left[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_left[k]);
                            }
                        }
                    }
                    else if (isLeft)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_right.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_right[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_right[k]);
                            }
                        }
                    }
                    else if (isUp)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_down.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_down[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_down[k]);
                            }
                        }
                    }
                    else if (isDown)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_up.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_up[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_up[k]);
                            }
                        }
                    }
                    else if (isForward)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_back.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_back[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_back[k]);
                            }
                        }
                    }
                    else if (isBack)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_forward.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_forward[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_forward[k]);
                            }
                        }
                    }
                }

                levelObjectsBasedOnNeighbour.Add(tempLevelObjectType);
            }

            for (int k = 0; k < levelObjectsBasedOnNeighbour.Count; k++)
            {
                List<LevelObjectType> tempObjectType = new List<LevelObjectType>();

                foreach (var item in finalLevelObjectType)
                {
                    tempObjectType.Add(item);
                }

                var intersection = tempObjectType.Intersect(levelObjectsBasedOnNeighbour[k]);

                finalLevelObjectType.Clear();

                foreach (var item in intersection)
                {
                    if (!finalLevelObjectType.Contains(item))
                    {
                        finalLevelObjectType.Add(item);
                    }
                }
            }

            if (finalLevelObjectType.Count > 0)
            {
                gridNode.possibleLevelObjectType = finalLevelObjectType;
            }

            if (!closedNodes.Contains(gridNode))
            {
                closedNodes.Add(gridNode);
            }

            openNodes.Remove(gridNode);
        }
    }

    private void CleanUp()
    {
        for (int i = 0; i < gridManager.GridNodesLst.Count; i++)
        {
            GridNode gridNode = gridManager.GridNodesLst[i];
            List<Vector3Int> neignboursCoord = gridNode.neighboursCoord;

            List<GridNode> neighbour = new List<GridNode>();

            for (int j = 0; j < neignboursCoord.Count; j++)
            {
                neighbour.Add(gridManager.GetGridNode(neignboursCoord[j]));
            }
            
            List<List<LevelObjectType>> levelObjectsBasedOnNeighbour = new List<List<LevelObjectType>>();
            List<LevelObjectType> finalLevelObjectType = new List<LevelObjectType>();

            GridNode connectingNeighbour = null;

            for (int t = 0; t < System.Enum.GetValues(typeof(LevelObjectType)).Length; t++)
            {
                finalLevelObjectType.Add((LevelObjectType)t);
            }

            for (int n = 0; n < neighbour.Count; n++)
            {
                List<LevelObjectType> tempLevelObjectType = new List<LevelObjectType>();

                bool isRight = false;
                bool isLeft = false;
                bool isUp = false;
                bool isDown = false;
                bool isForward = false;
                bool isBack = false;

                if (gridNode.gridCoord.x != neighbour[n].gridCoord.x)
                {
                    isRight = neighbour[n].gridCoord.x > gridNode.gridCoord.x;
                    isLeft = neighbour[n].gridCoord.x < gridNode.gridCoord.x;
                }
                else if (gridNode.gridCoord.y != neighbour[n].gridCoord.y)
                {
                    isUp = neighbour[n].gridCoord.y > gridNode.gridCoord.y;
                    isDown = neighbour[n].gridCoord.y < gridNode.gridCoord.y;
                }
                else if (gridNode.gridCoord.z != neighbour[n].gridCoord.z)
                {
                    isForward = neighbour[n].gridCoord.z > gridNode.gridCoord.z;
                    isBack = neighbour[n].gridCoord.z < gridNode.gridCoord.z;
                }

                for (int j = 0; j < neighbour[n].possibleLevelObjectType.Count; j++)
                {
                    ConnectionData cd = levelObjectConnection.GetAllPossibleConnection(neighbour[n].possibleLevelObjectType[j]);

                    if (isRight)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_left.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_left[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_left[k]);
                            }
                        }
                    }

                    if (isLeft)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_right.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_right[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_right[k]);
                            }
                        }
                    }
                    else if (isUp)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_down.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_down[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_down[k]);
                            }
                        }
                    }
                    else if (isDown)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_up.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_up[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_up[k]);
                            }
                        }
                    }
                    else if (isForward)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_back.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_back[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_back[k]);
                            }
                        }
                    }
                    else if (isBack)
                    {
                        for (int k = 0; k < cd.possibleConnectionIn_forward.Count; k++)
                        {
                            if (!tempLevelObjectType.Contains(cd.possibleConnectionIn_forward[k]))
                            {
                                tempLevelObjectType.Add(cd.possibleConnectionIn_forward[k]);
                            }
                        }
                    }
                }

                if (tempLevelObjectType.Contains(gridNode.possibleLevelObjectType[0]))
                {
                    connectingNeighbour = neighbour[n];
                }

                levelObjectsBasedOnNeighbour.Add(tempLevelObjectType);
            }

            for (int k = 0; k < levelObjectsBasedOnNeighbour.Count; k++)
            {
                List<LevelObjectType> tempObjectType = new List<LevelObjectType>();

                foreach (var item in finalLevelObjectType)
                {
                    tempObjectType.Add(item);
                }

                var intersection = tempObjectType.Intersect(levelObjectsBasedOnNeighbour[k]);

                finalLevelObjectType.Clear();

                foreach (var item in intersection)
                {
                    if(!finalLevelObjectType.Contains(item))
                    {
                        finalLevelObjectType.Add(item);
                    }
                }
            }

            if (finalLevelObjectType.Count > 0)
            {
                gridNode.possibleLevelObjectType.Clear();
                gridNode.possibleLevelObjectType.Add(finalLevelObjectType[Random.Range(0, finalLevelObjectType.Count)]);
            }
            else
            {
                /*gridNode.possibleLevelObjectType.Clear();

                bool isRight = false;
                bool isLeft = false;
                bool isUp = false;
                bool isDown = false;
                bool isForward = false;
                bool isBack = false;

                if (gridNode.gridCoord.x != connectingNeighbour.gridCoord.x)
                {
                    isRight = connectingNeighbour.gridCoord.x > gridNode.gridCoord.x;
                    isLeft = connectingNeighbour.gridCoord.x < gridNode.gridCoord.x;
                }
                else if (gridNode.gridCoord.y != connectingNeighbour.gridCoord.y)
                {
                    isUp = connectingNeighbour.gridCoord.y > gridNode.gridCoord.y;
                    isDown = connectingNeighbour.gridCoord.y < gridNode.gridCoord.y;
                }
                else if (gridNode.gridCoord.z != connectingNeighbour.gridCoord.z)
                {
                    isForward = connectingNeighbour.gridCoord.z > gridNode.gridCoord.z;
                    isBack = connectingNeighbour.gridCoord.z < gridNode.gridCoord.z;
                }

                switch (connectingNeighbour.possibleLevelObjectType[0])
                {
                    case LevelObjectType.road_staright_rightleft:
                        if(isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        break;

                    case LevelObjectType.road_staright_forwardback:
                        if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_x:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_t_forwardleftright:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_t_left_forwardback:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_t_backleftright:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_t_right_forwardback:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_turn_forwardleft:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_turn_forwardright:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_turn_backleft:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;

                    case LevelObjectType.road_turn_backright:
                        if (isRight)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_left);
                        }
                        else if (isLeft)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_right);
                        }
                        else if (isForward)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_back);
                        }
                        else if (isBack)
                        {
                            gridNode.possibleLevelObjectType.Add(LevelObjectType.road_end_forward);
                        }
                        break;
                }
                */
            }
        }
    }
    #endregion

    #region Object Spawning
   IEnumerator SpawneLevel()
    {
        if (spawnedLevelObjects == null)
        {
            spawnedLevelObjects = new List<LevelObject>();
        }
        else
        {
            foreach (LevelObject item in spawnedLevelObjects)
            {
                item.gameObject.SetActive(false);
            }

            spawnedLevelObjects.Clear();
        }

        SpawnLevelBoundary();

        for (int i = 0; i < gridManager.GridNodesLst.Count; i++)
        {
            SpawnObject(gridManager.GridNodesLst[i]);

            yield return null;
        }

        yield return null;

        isLevelSpawned = true;
        BakeNavMeshSurface();
    }

    private void SpawnObject(GridNode gridNode)
    {
        Quaternion randRotation = Quaternion.Euler(0, (int)Random.Range(0, 5) * 90, 0);

        string objectTag = gridNode.possibleLevelObjectType[0].ToString();

        GameObject obj = objectPooler.SpawnFormPool(objectTag, gridNode.gridPos, randRotation);
        obj.SetActive(true);
        obj.transform.SetParent(transform);

        obj.GetComponent<LevelObject>().SetUp(gridNode);

        gridNode.UpdateLevelObject(obj.GetComponent<LevelObject>());

        spawnedLevelObjects.Add(obj.GetComponent<LevelObject>());
    }

    private void SpawnLevelBoundary()
    {
        if (levelBoundayObjs == null)
        {
            levelBoundayObjs = new List<GameObject>();
        }
        else
        {
            foreach (GameObject item in levelBoundayObjs)
            {
                item.SetActive(false);
            }

            levelBoundayObjs.Clear();
        }

        int xMin = 0;
        int xMax = gridManager.GridSize.x;
        int zMin = 0;
        int zMax = gridManager.GridSize.z;

        Vector3 pos = Vector3.zero;
        GameObject obj = null;

        for (int i = xMin; i < xMax; i++)
        {
            pos = gridManager.GetGridNode(new Vector3Int(i, 0, zMin)).gridPos;
            pos.z -= gridManager.GridCellSize.z;

            obj = objectPooler.SpawnFormPool(Random.Range(0f, 10f) < 5f ? "city wall" : "city wall tower", pos, Quaternion.Euler(0, 180, 0));

            levelBoundayObjs.Add(obj);
        }

        for (int i = xMin; i < xMax; i++)
        {
            pos = gridManager.GetGridNode(new Vector3Int(i, 0, zMax - 1)).gridPos;
            pos.z += gridManager.GridCellSize.z;

            obj = objectPooler.SpawnFormPool(Random.Range(0f, 10f) < 5f ? "city wall" : "city wall tower", pos, Quaternion.Euler(0, 0, 0));

            levelBoundayObjs.Add(obj);
        }

        for (int i = zMin; i < zMax; i++)
        {
            pos = gridManager.GetGridNode(new Vector3Int(xMin, 0, i)).gridPos;
            pos.x -= gridManager.GridCellSize.x;

            obj = objectPooler.SpawnFormPool(Random.Range(0f, 10f) < 5f ? "city wall" : "city wall tower", pos, Quaternion.Euler(0, -90, 0));

            levelBoundayObjs.Add(obj);
        }

        for (int i = zMin; i < zMax; i++)
        {
            pos = gridManager.GetGridNode(new Vector3Int(xMax - 1, 0, i)).gridPos;
            pos.x += gridManager.GridCellSize.x;

            obj = objectPooler.SpawnFormPool(Random.Range(0f, 10f) < 5f ? "city wall" : "city wall tower", pos, Quaternion.Euler(0, 90, 0));

            levelBoundayObjs.Add(obj);
        }

        pos = gridManager.GetGridNode(new Vector3Int(xMin, 0, zMin)).gridPos;
        pos.x -= gridManager.GridCellSize.x;
        pos.z -= gridManager.GridCellSize.z;
        obj = objectPooler.SpawnFormPool("city wall cornor", pos, Quaternion.Euler(0, 0, 0));
        levelBoundayObjs.Add(obj);

        pos = gridManager.GetGridNode(new Vector3Int(xMin, 0, zMax - 1)).gridPos;
        pos.x -= gridManager.GridCellSize.x;
        pos.z += gridManager.GridCellSize.z;
        obj = objectPooler.SpawnFormPool("city wall cornor", pos, Quaternion.Euler(0, 0, 0));
        levelBoundayObjs.Add(obj);

        pos = gridManager.GetGridNode(new Vector3Int(xMax - 1, 0, zMin)).gridPos;
        pos.x += gridManager.GridCellSize.x;
        pos.z -= gridManager.GridCellSize.z;
        obj = objectPooler.SpawnFormPool("city wall cornor", pos, Quaternion.Euler(0, 0, 0));
        levelBoundayObjs.Add(obj);

        pos = gridManager.GetGridNode(new Vector3Int(xMax - 1, 0, zMax - 1)).gridPos;
        pos.x += gridManager.GridCellSize.x;
        pos.z += gridManager.GridCellSize.z;
        obj = objectPooler.SpawnFormPool("city wall cornor", pos, Quaternion.Euler(0, 0, 0));
        levelBoundayObjs.Add(obj);
    }
    #endregion

    #region Nav Mesh Baking
    private void BakeNavMeshSurface()
    {
        navMeshSurface.BuildNavMesh();
    }
    #endregion
}
