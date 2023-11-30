using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationDirection;
    [SerializeField] private float rotationSpeed;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationDirection, rotationSpeed * Time.deltaTime);
    }
}
