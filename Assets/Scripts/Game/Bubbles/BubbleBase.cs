using PlayerController;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleBase : MonoBehaviour
{
    [SerializeField]
    private float force = 1000f;

    [SerializeField, Min(0f)]
    private float addedPushMultiplier = 0.2f;

    private float destroyAfterSeconds = 5;
    public float initialMoveForce = 3f;
    [SerializeField, Min(0f)]
    private float lifeTime = 10f;
    private Rigidbody rb;
    private Collider bubbleCollider;
    private Animator bubbleAnimator;

    void OnEnable()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            bubbleCollider = GetComponent<Collider>();
            bubbleAnimator = GetComponentInChildren<Animator>();
        }

        rb.AddForce(initialMoveForce * transform.forward, ForceMode.VelocityChange);
        bubbleCollider.enabled = true;
        StartCoroutine(TimeOutBubble());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.TryGetComponent(out PlayerAvatar playerAvatar) && !playerAvatar.ultimateFormEnabled)
        {
            Vector3 direction = other.transform.position - transform.position;
            float newForce = playerAvatar.PlayerRigidbody.linearVelocity.magnitude + force;
            playerAvatar.Push(transform, newForce * direction, addedPushMultiplier);

            StartCoroutine(DestroyBubble());
        }
    }

    private void OnBecameInvisible()
    {
        if (enabled && gameObject.activeInHierarchy)
        {
            StartCoroutine(PendingDestroy());
        }
    }

    IEnumerator PendingDestroy()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);

        if (!GetComponentInChildren<MeshRenderer>().isVisible)
        {
            StartCoroutine(DestroyBubble());
            Debug.Log("Destroy");
        }
    }

    IEnumerator TimeOutBubble()
    {
        yield return new WaitForSeconds(lifeTime);
        StartCoroutine(DestroyBubble());
    }

    public IEnumerator DestroyBubble()
    {
        bubbleCollider.enabled = false;
        bubbleAnimator.SetBool("BubbleWasHit", true);
        yield return new WaitForSeconds(bubbleAnimator.GetCurrentAnimationLength() - 0.1f);
        PoolManager.ReturnObjectToPoolOrDestroyIt(gameObject);
        //Destroy(gameObject);
    }
}
