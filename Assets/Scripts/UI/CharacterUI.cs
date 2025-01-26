using TMPro;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    public TextMeshProUGUI PlayerLabel;
    public Animator Animator;
    public PlayerData AttachedPlayer;
    
    public void StartAnimation()
    {
        Animator.SetTrigger("PlayerSelection_startingUp");
    }

    public void LockAnimation()
    {
        Animator.SetTrigger("PlayerPressSelectButton");
    }

    public void UnLockAnimation()
    {
        Animator.SetTrigger("PlayerPressSelectButton");
    }

}