using PlayerController;
using UnityEngine;

public class KillField : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.KillPlayer(other.GetComponent<Player>());
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }

}
