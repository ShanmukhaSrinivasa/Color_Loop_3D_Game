using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    public static bool isGameActive = false;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        isGameActive = false;

        LevelManager.Instance.LoadCurrentLevel();
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        isGameActive = true;
    }

    public void ShowGameOver()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);
    }

    public void ShowVictoryPanel()
    {
        isGameActive = false;
        victoryPanel.SetActive(true);
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

}
