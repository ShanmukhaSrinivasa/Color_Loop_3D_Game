using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelButtonPrefab;
    public Transform gridContainer;
    public UIManager uiManager;

    private void OnEnable()
    {
        GenerateLevelButton();
    }

    public void GenerateLevelButton()
    {
        foreach(Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        int totalLevels = LevelManager.Instance.levels.Count;
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 0);

        for(int i=0; i<totalLevels; i++)
        {
            int levelIndex = i;

            GameObject newButton = Instantiate(levelButtonPrefab, gridContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();

            Button btnComponent = newButton.GetComponent<Button>();

            if(i <= unlockedLevels)
            {
                btnComponent.interactable = true;
                btnComponent.onClick.AddListener(() => LoadLevelAndPlay(levelIndex));
            }
            else
            {
                btnComponent.interactable= false;
            }
        }
    }

    private void LoadLevelAndPlay(int index)
    {
        LevelManager.Instance.LoadSpecificLevel(index);

        gameObject.SetActive(false);

        uiManager.StartGame();
    }
}
