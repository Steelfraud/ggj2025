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
        Invoke("playIntroSound", 1f);
    }

    void playIntroSound()
    {
        SoundEffectManager.instance.PlaySoundEffect("Announcer_Bubbler2000");
    }

    public void StartGame()
    {
        CharacterSelect.SetActive(true);
        SoundEffectManager.instance.PlaySoundEffect("Announcer_Choose");
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
