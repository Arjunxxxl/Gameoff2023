using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Canavs")]
    [SerializeField] private Canvas loadingMenu;
    [SerializeField] private Canvas gameplayMenu;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas gameoverMenu;

    [Header("Button")]
    [SerializeField] private Button gameOverHomeButton;

    #region SingleTon
    public static UiManager Instance;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameOverHomeButton.onClick.AddListener(OnClickGameOverHomeButton);
    }

    public void ShowLoadingScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;

        loadingMenu.enabled = true;
        gameplayMenu.enabled = false;
        pauseMenu.enabled = false;
        gameoverMenu.enabled = false;
    }

    public void ShowGameplayScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;

        loadingMenu.enabled = false;
        gameplayMenu.enabled = true;
        pauseMenu.enabled = false;
        gameoverMenu.enabled = false;
    }

    public void ShowPauseScreen()
    {
        Cursor.lockState = CursorLockMode.None;

        loadingMenu.enabled = false;
        gameplayMenu.enabled = false;
        pauseMenu.enabled = true;
        gameoverMenu.enabled = false;
    }

    public void ShowGameOverScreen()
    {
        Cursor.lockState = CursorLockMode.None;

        loadingMenu.enabled = false;
        gameplayMenu.enabled = false;
        pauseMenu.enabled = false;
        gameoverMenu.enabled = true;
    }

    private void OnClickGameOverHomeButton()
    {
        SoundManager.PlayAudio("button", true, true);
        StartCoroutine(GameOverHomebutton());
    }

    IEnumerator GameOverHomebutton()
    {
        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene(0);
    }
}
