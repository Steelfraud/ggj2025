using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float force = 15f;

    [SerializeField]
    private float lifeTime = 15f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(force * transform.forward, ForceMode.VelocityChange);
        StartCoroutine(DestroyAfterSeconds(lifeTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.rigidbody.AddForce(force * transform.forward, ForceMode.VelocityChange);
            Destroy(gameObject);
        }
        else if(collision.rigidbody !=null)
        {
            collision.rigidbody.AddForce(force * transform.forward, ForceMode.VelocityChange);
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
