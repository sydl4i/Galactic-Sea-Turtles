using UnityEngine;

public class StarPickMe : MonoBehaviour
{
    [Header("Shepherd (leave empty to auto-find camera)")]
    public Transform shepherd; // usually the XR camera transform

    [Header("Height")]
    public float floatHeight = 1.2f;

    [Header("Approach Behavior")]
    public float approachStartDistance = 3.0f;   // starts moving toward you within this range
    public float stopDistance = 0.6f;            // stops this close so it doesn't clip into you
    public float approachSpeed = 0.6f;           // base approach speed
    public float maxApproachSpeed = 1.4f;        // sprinty when far
    public float turnSmooth = 6f;                // smoothing for direction changes

    [Header("Hoppy Excitement")]
    public float hopHeight = 0.12f;              // vertical hop amount
    public float hopSpeed = 3.0f;                // hop frequency
    public float wiggleAmount = 0.06f;           // small side wiggle
    public float wiggleSpeed = 2.2f;

    [Header("Optional")]
    public bool lockBaseHeight = true;           // keep roughly at spawn height
    public float minY = 0.2f;                    // don't sink below floor-ish

    private Vector3 home;
    private float yBase;
    private Vector3 planarVel;

    void Start()
    {
        home = transform.position;
        yBase = floatHeight;
        home.y = yBase;
        transform.position = home;

    }

    void Update()
    {
        if (shepherd == null) return;

        // If grabbed/parented, don't fight the grab
        if (transform.parent != null && transform.parent != transform.root)
            return;

        Vector3 shepherdPos = shepherd.position;

        // Planar (XZ) movement toward shepherd
        Vector3 toShepherd = shepherdPos - transform.position;
        toShepherd.y = 0f;

        float dist = toShepherd.magnitude;

        // Only approach when close enough, otherwise idle-hop in place
        Vector3 desiredVel = Vector3.zero;

        if (dist <= approachStartDistance && dist > stopDistance)
        {
            Vector3 dir = toShepherd.normalized;

            // Move a bit faster when farther away (feels eager)
            float t = Mathf.InverseLerp(stopDistance, approachStartDistance, dist);
            float speed = Mathf.Lerp(approachSpeed, maxApproachSpeed, t);

            desiredVel = dir * speed;
        }

        // Smooth steering
        planarVel = Vector3.Lerp(planarVel, desiredVel, turnSmooth * Time.deltaTime);

        Vector3 next = transform.position + planarVel * Time.deltaTime;

        // Excited hop + wiggle
        float hop = Mathf.Abs(Mathf.Sin(Time.time * hopSpeed)) * hopHeight; // always up
        float wiggle = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;

        // Apply wiggle sideways relative to motion or forward direction
        Vector3 side = Vector3.right;
        if (toShepherd.sqrMagnitude > 0.001f)
        {
            Vector3 forward = toShepherd.normalized;
            side = Vector3.Cross(Vector3.up, forward).normalized;
        }

        next += side * wiggle * Time.deltaTime; // tiny drift

        float baseY = lockBaseHeight ? yBase : transform.position.y;
        next.y = Mathf.Max(minY, baseY + hop);

        transform.position = next;

        // Face the shepherd slightly (cute)
        if (toShepherd.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(toShepherd.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 6f * Time.deltaTime);
        }
    }

    private Transform FindBestCameraTransform()
    {
        if (Camera.main != null) return Camera.main.transform;

        Camera[] cams = FindObjectsOfType<Camera>(true);
        foreach (var c in cams)
            if (c != null && c.enabled) return c.transform;

        return null;
    }
}
