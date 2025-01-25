using UnityEngine;

public class LockTransform : MonoBehaviour
{
    public bool LockPosition = false;
    public bool LockRotation = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (LockPosition)
        {
            transform.position = startPosition;
        }

        if (LockRotation)
        {
            transform.rotation = startRotation;
        }
    }

}
