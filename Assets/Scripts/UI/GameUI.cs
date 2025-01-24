using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI TimerLabel;
    public GameObject GameEndPanelParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerLabel != null)
        {
            if (GameManager.Instance != null)
            {
                TimerLabel.text = Mathf.RoundToInt(GameManager.Instance.RoundTimer).ToString();
            }
            else
            {
                TimerLabel.text = "";
            }
        }
    }

    public void ToggleGameEndPanel(bool setTo)
    {
        GameEndPanelParent.SetActive(setTo);
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

}