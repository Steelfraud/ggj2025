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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(initialMoveForce * transform.forward, ForceMode.VelocityChange);
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
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetBool("BubbleWasHit", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimationLength() - 0.1f);
        PoolManager.ReturnObjectToPoolOrDestroyIt(gameObject);
        //Destroy(gameObject);
    }
}
