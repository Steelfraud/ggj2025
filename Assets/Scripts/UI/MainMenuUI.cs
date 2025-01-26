using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject CreditsParent;

    private void Awake()
    {
        HideCredits();
    }

    private void Start()
    {
        MusicPlaylistManager.Instance.ChangePlaylist("Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowCredits()
    {
        CreditsParent.SetActive(true);
    }

    public void HideCredits()
    {
        CreditsParent.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
