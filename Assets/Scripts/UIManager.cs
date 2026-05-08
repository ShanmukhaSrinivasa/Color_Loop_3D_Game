using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject inGamePanel;
    public GameObject pausePanel;

    [Header("Revive System")]
    public GameObject revivePanel;
    public TextMeshProUGUI reviveTimerText;
    private Coroutine reviveCoroutine;

    public TextMeshProUGUI limitText;
    public TextMeshProUGUI levelText;

    public static bool isGameActive = false;
    public bool isPaused = false;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        pausePanel.SetActive(false);
        inGamePanel.SetActive(false);
        revivePanel.SetActive(false);
        isGameActive = false;

        LevelManager.Instance.LoadCurrentLevel();
        Time.timeScale = 1.0f;
    }

    public void StartGame()
    {
        levelSelectPanel.SetActive(false);
        inGamePanel.SetActive(true);
        isGameActive = true;
        isPaused = false;
        Time.timeScale = 1.0f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if(isPaused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ShowGameOver()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);
        inGamePanel.SetActive(false);
    }

    public void ShowVictoryPanel()
    {
        isGameActive = false;
        victoryPanel.SetActive(true);
        inGamePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        LevelManager.Instance.RestartLevel();
        gameOverPanel.SetActive(false);
    }

    public void NextLevel()
    {
        LevelManager.Instance.NextLevel();
        victoryPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        inGamePanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        inGamePanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        isGameActive = false;

        LevelManager.Instance.ClearCurrentLevel();
    }

    public void UpdateConveyorLimit(int currentAmount, int maxLimit)
    {
        if(limitText != null)
        {
            limitText.text = $"{currentAmount}/{maxLimit}";

            if(currentAmount >= maxLimit - 1)
            {
                limitText.color = Color.red;
            }
            else
            {
                limitText.color= Color.white;
            }
        }
    }

    public void UpdateLevelText(int levelNumber)
    {
        if (levelText != null)
        {
            levelText.text = $"LEVEL - {levelNumber}";
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowRevivePanel()
    {
        isGameActive = false;
        revivePanel.SetActive(true);
        Time.timeScale = 0f;

        if (reviveCoroutine != null)
        {
            StopCoroutine(reviveCoroutine);
        }

        reviveCoroutine = StartCoroutine(ReviveCountDown());
    }

    private IEnumerator ReviveCountDown()
    {
        int time = 5;
        while (time > 0)
        {
            reviveTimerText.text = $"Watch Ad to Revive? ({time})";
            yield return new WaitForSecondsRealtime(1f);
            time--;
        }

        DeclineRevive();
    }

    public void AcceptRevive()
    {
        if (reviveCoroutine != null)
        {
            StopCoroutine(reviveCoroutine);
        }
        revivePanel.SetActive(false);
        Time.timeScale = 1f;
        isGameActive = true;

        LevelManager.Instance.queueManager.ExecuteRevive();
    }

    public void DeclineRevive()
    {
        if (reviveCoroutine != null)
        {
            StopCoroutine(reviveCoroutine);
        }

        revivePanel.SetActive(false);
        ShowGameOver();
    }

}
