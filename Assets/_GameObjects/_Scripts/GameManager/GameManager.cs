using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action GameSetUp;
    public static Action GameSetCompleted;

    private void OnEnable()
    {
        GameSetCompleted += OnGameSetCompleted;
    }

    private void OnDisable()
    {
        GameSetCompleted -= OnGameSetCompleted;
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.StopAudio("bg menu");

        UiManager.Instance.ShowLoadingScreen();

        StartCoroutine(SetUpGame());
    }

    IEnumerator SetUpGame()
    {
        yield return new WaitForSeconds(0.5f);

        Player.SetUpPlayer?.Invoke();
        GameSetUp?.Invoke();
    }

    private void OnGameSetCompleted()
    {
        EnemySpawner.StartWaves?.Invoke();

        UiManager.Instance.ShowGameplayScreen();

        SoundManager.PlayAudio?.Invoke("bg", false, true);
    }
}
