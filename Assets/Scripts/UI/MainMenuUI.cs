using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject CreditsParent;
    public GameObject CharacterSelect;
    public GameObject FirstElement;

    private void Awake()
    {
        HideCredits();
    }

    private void Start()
    {
        MusicPlaylistManager.Instance.ChangePlaylist("Menu");
        DataManager.Instance.playerColors.Clear();
        Invoke("playIntroSound", 1f);

        if (this.FirstElement != null && gameObject.activeInHierarchy && EventSystem.current != null && EventSystem.current.currentSelectedGameObject != this.FirstElement)
        {
            EventSystem.current.SetSelectedGameObject(this.FirstElement);
        }
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
