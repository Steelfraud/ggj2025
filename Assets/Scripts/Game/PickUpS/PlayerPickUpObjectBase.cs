using PlayerController;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPickUpObjectBase : MonoBehaviour
{
    public PickUpSpawnPosition MySpawnPosition;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player") // or change to whatever we use to detect player
        {
            ApplyEffect(collision.GetComponent<PlayerAvatar>());
        }
    }

    public void OnEnable()
    {
        CustomCamera.Instance.AddToTargetGroup(transform, 0.3f);
    }

    public void DestroyPickup()
    {
        GameManager.Instance.RemovePickUp(this);
        CustomCamera.Instance.RemoveFromTargetGroup(transform);
        PoolManager.ReturnObjectToPoolOrDestroyIt(gameObject);
    }

    protected virtual void ApplyEffect(PlayerAvatar player)
    {
        Debug.Log("I was picked yaaaay :)))");
        DestroyPickup();
    }

}
