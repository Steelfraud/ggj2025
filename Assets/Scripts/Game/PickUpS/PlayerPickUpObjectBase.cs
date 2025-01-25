using PlayerController;
using UnityEngine;

public class PlayerPickUpObjectBase : MonoBehaviour
{
    public PickUpSpawnPosition MySpawnPosition;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player") // or change to whatever we use to detect player
        {
            ApplyEffect(collision.GetComponent<Player>());
        }
    }

    public void DestroyPickup()
    {
        GameManager.Instance.RemovePickUp(this);
        PoolManager.ReturnObjectToPoolOrDestroyIt(gameObject);
    }

    protected virtual void ApplyEffect(Player player)
    {
        Debug.Log("I was picked yaaaay :)))");
        DestroyPickup();
    }

}
