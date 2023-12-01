using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas learnMenu;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas loadingMenu;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button learnButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button learnBackButton;
    [SerializeField] private Button settingsBackButton;

    [Header("High Score Txt")]
    [SerializeField] private TMP_Text highScoreTxt;

    [Header("Settings")]
    [SerializeField] private Slider mouseSensitivitySlider;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(OnClickPlayButton);
        learnButton.onClick.AddListener(OnClickLearnButton);
        settingsButton.onClick.AddListener(OnClickSettingsButton);
        learnBackButton.onClick.AddListener(OnClickLearnBackButton);
        settingsBackButton.onClick.AddListener(OnClickSettingsBackButton);

        LocalDataManager.Instance.LoadAlData();

        SetUpHighScoreTxt();
        SetUpSettins();

        ShowMainMenu();

        Cursor.lockState = CursorLockMode.None;

        SoundManager.PlayAudio("bg menu", false, true);
    }

    #region Canvases
    private void ShowMainMenu()
    {
        mainMenu.enabled = true;
        learnMenu.enabled = false;
        settingsMenu.enabled = false;
        loadingMenu.enabled = false;
    }

    private void ShowLearnMenu()
    {
        mainMenu.enabled = false;
        learnMenu.enabled = true;
        settingsMenu.enabled = false;
        loadingMenu.enabled = false;
    }

    private void ShowSettinsMenu()
    {
        mainMenu.enabled = false;
        learnMenu.enabled = false;
        settingsMenu.enabled = true;
        loadingMenu.enabled = false;
    }

    private void ShowLoadingMenu()
    {
        mainMenu.enabled = false;
        learnMenu.enabled = false;
        settingsMenu.enabled = false;
        loadingMenu.enabled = true;
    }
    #endregion

    #region Buttons
    private void OnClickPlayButton()
    {
        //SoundManager.PlayAudio("button", true, true);
        SoundManager.StopAudio("bg menu");
        StartCoroutine(PlayButton());
    }

    IEnumerator PlayButton()
    {
        yield return new WaitForSeconds(0.3f);

        ShowLoadingMenu();

        SceneManager.LoadSceneAsync(1);
    }

    private void OnClickLearnButton()
    {
       // SoundManager.PlayAudio("button", true, true);
        StartCoroutine(LearnButton());
    }

    IEnumerator LearnButton()
    {
        yield return new WaitForSeconds(0.3f);

        ShowLearnMenu();
    }

    private void OnClickSettingsButton()
    {
       // SoundManager.PlayAudio("button", true, true);
        StartCoroutine(SettingsButton());
    }

    IEnumerator SettingsButton()
    {
        yield return new WaitForSeconds(0.3f);

        ShowSettinsMenu();
    }

    private void OnClickLearnBackButton()
    {
      //  SoundManager.PlayAudio("button", true, true);
        StartCoroutine(LearnBackButton());
    }

    IEnumerator LearnBackButton()
    {
        yield return new WaitForSeconds(0.3f);

        ShowMainMenu();
    }

    private void OnClickSettingsBackButton()
    {
       // SoundManager.PlayAudio("button", true, true);
        StartCoroutine(SettingsBackButton());
    }

    IEnumerator SettingsBackButton()
    {
        yield return new WaitForSeconds(0.3f);

        SaveMouseSenvitivity();
        ShowMainMenu();
    }
    #endregion

    #region Settins
    private void SetUpSettins()
    {
        mouseSensitivitySlider.minValue = 0.1f;
        mouseSensitivitySlider.maxValue = 2f;
        mouseSensitivitySlider.value = LocalDataManager.Instance.MouseSensitivity;
    }

    private void SaveMouseSenvitivity()
    {
        LocalDataManager.Instance.SetMouseSensitivity(mouseSensitivitySlider.value);
    }
    #endregion

    #region High Score Txt
    private void SetUpHighScoreTxt()
    {
        float highScore = LocalDataManager.Instance.TimeSurvived;

        int hr = (int)(highScore / 3600);
        int min = (int)((highScore - hr * 3600) / 60);
        int sec = (int)(highScore - hr * 3600 - min * 60);

        highScoreTxt.text = "";


        highScoreTxt.text += hr < 10 ? "0" + hr : hr;
        highScoreTxt.text += " Hr ";

        highScoreTxt.text += min < 10 ? "0" + min : min;
        highScoreTxt.text += " Min ";

        highScoreTxt.text += sec < 10 ? "0" + sec : sec;
        highScoreTxt.text += " Sec ";
    }
    #endregion
}
