using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridNode
{
    public Vector3Int gridCoord;
    public Vector3 gridPos;

    public List<Vector3Int> neighboursCoord;

    public LevelObject levelObject;
    public List<LevelObjectType> possibleLevelObjectType;

    public void UpdateLevelObject(LevelObject levelObject)
    {
        this.levelObject = levelObject;
    }
}
