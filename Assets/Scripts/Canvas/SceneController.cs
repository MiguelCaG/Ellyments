using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static AbilityManager;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject restartPanel;
    private TextMeshProUGUI restartText;

    // TEMPORAL
    AbilityManager abilityM;
    //

    private void Start()
    {
        Time.timeScale = 1f;
        PlayerBehaviour.Restart += RestartPanel;
        Boss.Restart += RestartPanel;

        if (restartPanel != null)
            restartText = restartPanel.GetComponentInChildren<TextMeshProUGUI>();

        // TEMPORAL
        if (SceneManager.GetActiveScene().name == "ZephyrosBossFight")
            abilityM = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilityManager>();
        //
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            QuitPanel();
        }

        // TEMPORAL
        if (SceneManager.GetActiveScene().name == "ZephyrosBossFight" && !abilityM.IsAbilityUnlocked(Ability.DoubleJump))
            abilityM.UnlockAbility(Ability.DoubleJump);
        //
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitPanel()
    {
        if (quitPanel.activeSelf)
        {
            quitPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            quitPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void RestartPanel(string result)
    {
        restartPanel.SetActive(true);
        restartText.text += result;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        PlayerBehaviour.Restart -= RestartPanel;
        Boss.Restart -= RestartPanel;
    }
}