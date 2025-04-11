using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private PlayerData pD;

    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject audioPanel;
    private TextMeshProUGUI restartText;

    public static bool stopTime;

    private bool playerDead = false;

    [SerializeField] private AudioClip newMusic;

    private void Start()
    {
        stopTime = false;
        Time.timeScale = 1f;

        ZoneChanger.ChangeScene += ChangeScene;
        PlayerBehaviour.Restart += RestartPanel;
        EventManager.PlayerDead += () => playerDead = true;

        if (restartPanel != null) restartText = restartPanel.GetComponentInChildren<TextMeshProUGUI>();

        if (MusicManager.instance != null && newMusic != null)
        {
            MusicManager.instance.ChangeMusic(newMusic);
        }
    }

    private void Update()
    {
        if(restartPanel != null) if (restartPanel.activeSelf == true) return;

        if (Input.GetButtonDown("Cancel") && !playerDead)
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") ShowPanel(quitPanel);
            else ShowPanel(optionsPanel);
        }
        // DEVELOPER MODE ONLY
        DeveloperMode();
        //
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
        if (audioPanel.activeSelf) audioPanel.SetActive(false);
        else
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                Time.timeScale = 1f;
                stopTime = false;
            }
            else
            {
                panel.SetActive(true);
                Time.timeScale = 0f;
                stopTime = true;
            }
        }
    }

    public void OpenCloseAudioSettings()
    {
        if (audioPanel.activeSelf) audioPanel.SetActive(false);
        else audioPanel.SetActive(true);
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

    // DEVELOPER MODE ONLY ///////////////

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        ReloadScene();
    }

    [SerializeField] private Button changeBossLifeScale;
    private GameObject boss;

    [SerializeField] private AbilityManager aM;
    [SerializeField] private Button unlockNotSelectedAbility;

    private void DeveloperMode()
    {
        if(changeBossLifeScale != null)
        {
            if (changeBossLifeScale.interactable == false)
            {
                boss = GameObject.FindGameObjectWithTag("Boss");
                if (boss != null)
                {
                    if (boss.activeSelf)
                    {
                        changeBossLifeScale.interactable = true;
                        changeBossLifeScale.onClick.AddListener(ChangeBossLifeScale);
                    }
                }
                else changeBossLifeScale.interactable = false;
            }
        }

        if (unlockNotSelectedAbility == null) return;
        if (unlockNotSelectedAbility.interactable == true) return;
        if ((aM.IsAbilityUnlocked(AbilityManager.Ability.Dash) && !aM.IsAbilityUnlocked(AbilityManager.Ability.DoubleJump)) || (!aM.IsAbilityUnlocked(AbilityManager.Ability.Dash) && aM.IsAbilityUnlocked(AbilityManager.Ability.DoubleJump)))
        {
            unlockNotSelectedAbility.interactable = true;
            unlockNotSelectedAbility.onClick.AddListener(UnlockNotSelectedAbility);
        }
    }

    private void ChangeBossLifeScale()
    {
        Boss bossComp = boss.GetComponent<Boss>();
        if (bossComp.maxLife == 200f)
        {
            bossComp.maxLife /= 10f;
            bossComp.life /= 10f;
        }
        else
        {
            bossComp.maxLife *= 10f;
            bossComp.life *= 10f;
        }
    }

    private void UnlockNotSelectedAbility()
    {
        unlockNotSelectedAbility.interactable = false;
        if (!aM.IsAbilityUnlocked(AbilityManager.Ability.DoubleJump)) aM.UnlockAbility(AbilityManager.Ability.DoubleJump);
        if (!aM.IsAbilityUnlocked(AbilityManager.Ability.Dash)) aM.UnlockAbility(AbilityManager.Ability.Dash);
        aM.SaveState();
    }
    //////////////////////////////////////


    private void OnDestroy()
    {
        ZoneChanger.ChangeScene -= ChangeScene;
        PlayerBehaviour.Restart -= RestartPanel;
        EventManager.PlayerDead -= () => playerDead = true;
    }
}