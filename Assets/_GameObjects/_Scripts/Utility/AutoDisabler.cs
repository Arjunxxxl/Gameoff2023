using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisabler : MonoBehaviour
{
    [SerializeField] private float disableDelay;

    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        yield return new WaitForSecondsRealtime(disableDelay);
        gameObject.SetActive(false);
    }
}
