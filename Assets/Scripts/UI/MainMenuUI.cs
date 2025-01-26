using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject CreditsParent;
    public GameObject CharacterSelect;

    private void Awake()
    {
        HideCredits();
    }

    private void Start()
    {
        MusicPlaylistManager.Instance.ChangePlaylist("Menu");
        DataManager.Instance.playerColors.Clear();
    }

    public void StartGame()
    {
        CharacterSelect.SetActive(true);
        gameObject.SetActive(false);
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
