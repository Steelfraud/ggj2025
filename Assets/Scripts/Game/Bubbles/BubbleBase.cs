using PlayerController;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleBase : MonoBehaviour
{
    [SerializeField]
    private float force = 1000f;

    private float destroyAfterSeconds = 5;
    public float initialMoveForce = 3f;
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
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && !other.gameObject.GetComponent<PlayerAvatar>().ultimateFormEnabled)
        {
            Vector3 direction = other.transform.position - transform.position;
            float newForce = other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude + force;
            other.collider.GetComponent<Rigidbody>().AddForce(newForce * direction, ForceMode.VelocityChange);

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

    public IEnumerator DestroyBubble()
    {
        bubbleCollider.enabled = false;
        bubbleAnimator.SetBool("BubbleWasHit", true);
        yield return new WaitForSeconds(bubbleAnimator.GetCurrentAnimationLength() - 0.1f);
        PoolManager.ReturnObjectToPoolOrDestroyIt(gameObject);
        //Destroy(gameObject);
    }
}
