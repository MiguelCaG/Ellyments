using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] private PlayerData pD;

    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject restartPanel;
    private TextMeshProUGUI restartText;

    private void Start()
    {
        Time.timeScale = 1f;

        ZoneChanger.ChangeScene += ChangeScene;
        PlayerBehaviour.Restart += RestartPanel;

        if (restartPanel != null)
            restartText = restartPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") ShowPanel(quitPanel);
            else ShowPanel(optionsPanel);
        }
    }

    public void StartButton()
    {
        Time.timeScale = 1f;
        pD.lastCheckpoint.respawn = true;
        SceneManager.LoadScene(pD.lastCheckpoint.scene);
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        if (pD.lastCheckpoint.respawn)
        {
            SceneManager.LoadScene(pD.lastCheckpoint.scene);
        }
        else
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    public void ShowPanel(GameObject panel)
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            panel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void RestartPanel(string result)
    {
        Time.timeScale = 0f;
        restartPanel.SetActive(true);
        restartText.text += result;
        pD.lastCheckpoint.respawn = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        ZoneChanger.ChangeScene -= ChangeScene;
        PlayerBehaviour.Restart -= RestartPanel;
    }
}