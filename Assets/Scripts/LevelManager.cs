using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class levelConfig
    {
        public string levelname;
        [Tooltip("Type your map here! 0=Empty, 1=Color[0], 2=color[1], etc.")]
        public string[] layout;
        public Material[] colorsToUse;
    }

    [Header("Level Data")]
    public List<levelConfig> levels;
    public int currentLevelindex = 0;

    [Header("System References")]
    public GridManager gridManager;
    public UIManager uiManager;
    public QueueManager queueManager;

    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    public void LoadCurrentLevel()
    {
        // 1.Clear any existing cubes if we are moving to a new Level
        foreach(Transform child in gridManager.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. Get the layout for the current level
        levelConfig config = levels[currentLevelindex];

        // 3.Pass the layout to the GridManager

        gridManager.layout = config.layout;
        gridManager.availableColors = config.colorsToUse;

        gridManager.GenerateGrid();

        queueManager.ResetAndGenerateQueue();

        uiManager.UpdateLevelText(currentLevelindex + 1);
    }

    public void LoadSpecificLevel(int index)
    {
        currentLevelindex = index;
        LoadCurrentLevel();
    }

    public void NextLevel()
    {
        currentLevelindex++;

        int highestUnlocked = PlayerPrefs.GetInt("UnlockedLevels", 0);
        if(currentLevelindex > highestUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevels", currentLevelindex);
            PlayerPrefs.Save();
        }

        if(currentLevelindex >= levels.Count)
        {
            currentLevelindex = 0;
        }

        LoadCurrentLevel();
        uiManager.StartGame();
    }

    public void RestartLevel()
    {
        LoadCurrentLevel();
        uiManager.StartGame();
    }

    public void ClearCurrentLevel()
    {
        foreach (Transform child in gridManager.transform)
        {
            Destroy(child.gameObject);
        }

        queueManager.WipeAllCharacters();
    }

}
