using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlyaerHp : MonoBehaviour
{
    [Header("Hp Data")]
    [SerializeField] private bool isDead;
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;

    public static Action<int> UpdateCurrentHp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        currentHp = maxHp;
        isDead = false;

        UpdateCurrentHp?.Invoke(currentHp);
    }

    private void UpdateHp(int amt)
    {
        currentHp += amt;

        if(currentHp >= maxHp)
        {
            currentHp = maxHp;
        }
        else if(currentHp <= 0)
        {
            isDead = true;
            currentHp = 0;
        }

        UpdateCurrentHp?.Invoke(currentHp);
    }
}
