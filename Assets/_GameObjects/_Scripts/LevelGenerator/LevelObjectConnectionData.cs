using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectConnectionData : MonoBehaviour
{
    [SerializeField] private List<ConnectionData> connectionData;

    #region SingleTon
    public static LevelObjectConnectionData Instance;
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
    }
    #endregion

    public ConnectionData GetAllPossibleConnection(LevelObjectType levelObjectType)
    {
        for(int i = 0; i < connectionData.Count; i++)
        {
            if (connectionData[i].baseLevelObjectType == levelObjectType)
            {
                return connectionData[i];
            }
        }

        return connectionData[0];
    }
}

[System.Serializable]
public class ConnectionData
{
    public LevelObjectType baseLevelObjectType;
    public List<LevelObjectType> possibleConnectionIn_right;
    public List<LevelObjectType> possibleConnectionIn_left;
    public List<LevelObjectType> possibleConnectionIn_up;
    public List<LevelObjectType> possibleConnectionIn_down;
    public List<LevelObjectType> possibleConnectionIn_forward;
    public List<LevelObjectType> possibleConnectionIn_back;
}