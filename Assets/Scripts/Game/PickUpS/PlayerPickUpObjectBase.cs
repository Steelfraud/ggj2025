using PlayerController;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPickUpObjectBase : MonoBehaviour
{
    public PickUpSpawnPosition MySpawnPosition;

    protected PickUpDataObject myData;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player") // or change to whatever we use to detect player
        {
            ApplyEffect(collision.GetComponent<PlayerAvatar>());
        }
    }

    public virtual void SetupPickup(PickUpDataObject pickUpDataObject)
    {
        myData = pickUpDataObject;
    }

    public void DestroyPickup()
    {
        GameManager.Instance.RemovePickUp(this);
        CustomCamera.Instance.RemoveFromTargetGroup(transform);
        PoolManager.ReturnObjectToPoolOrDestroyIt(gameObject);
    }

    protected virtual void ApplyEffect(PlayerAvatar player)
    {
        if (myData != null)
        {
            foreach (ModifierData modifierData in myData.ModifiersToApply)
            {
                player.PlayerModifierHandler.AddModifier(new BasicModifierSource(modifierData));
            }
        }

        Debug.Log("I was picked yaaaay :)))");
        DestroyPickup();
    }

}
