using UnityEngine;

public class FreezeAfterLanding : MonoBehaviour
{
    private Rigidbody rb;
    private bool frozen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (frozen) return;

        // Stop all motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Freeze in place
        rb.isKinematic = true;

        frozen = true;
    }
}