using UnityEngine;

public class StarMischief : MonoBehaviour
{
    [Header("Avoid Target (leave empty to auto-find)")]
    public Transform targetToAvoid; // If empty, script will auto-find the active XR camera.

    [Header("Height")]
    public float floatHeight = 1.2f; // meters off the floor

    [Header("Movement")]
    public float smallMoveRadius = 1.2f;
    public float bigMoveRadius = 2.5f;
    public float baseSpeed = 0.8f;
    public float maxSpeed = 3.0f;

    [Header("Behavior")]
    public float scareRadius = 0.7f;
    public float repickMinTime = 0.3f;
    public float repickMaxTime = 1.2f;
    [Range(0f, 1f)] public float bigJumpChance = 0.3f;

    [Header("Bounds (prevents drifting away forever)")]
    public float maxDistanceFromHome = 2.75f; // hard clamp from spawn point

    [Header("Float")]
    public float bobAmount = 0.1f;
    public float bobSpeed = 3f;

    [Header("Debug")]
    public bool drawGizmos = false;

    private Vector3 home;
    private Vector3 goal;
    private float nextPick;
    private float currentSpeed;
    private float yBase;

    // Grab friendliness: if the object gets parented (common in grab systems),
    // stop moving it so it doesn’t fight the grab.
    private Transform initialParent;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialParent = transform.parent;
    }

    void Start()
    {
        // Auto-find target if not assigned
        if (targetToAvoid == null)
            targetToAvoid = FindBestCameraTransform();

        // Set a consistent base height and treat current position as "home" (XZ)
        home = transform.position;
        yBase = floatHeight;

        home.y = yBase;
        transform.position = home;

        PickNewTarget(forceBig: false);
    }

    void Update()
    {
        // 1) If grabbed (often becomes child of hand), don’t fight the grab.
        if (transform.parent != initialParent && transform.parent != null)
            return;

        // 2) Keep trying to auto-find the camera if it wasn't ready at Start.
        if (targetToAvoid == null)
            targetToAvoid = FindBestCameraTransform();

        // 3) Flee if target is close
        if (targetToAvoid != null)
        {
            float d = Vector3.Distance(transform.position, targetToAvoid.position);
            if (d < scareRadius)
                PickFleeTarget();
        }

        // 4) Random repick
        if (Time.time >= nextPick)
            PickNewTarget(forceBig: false);

        // 5) Compute next position (XZ move + Y bob)
        Vector3 currentPos = rb != null ? rb.position : transform.position;

        Vector3 next = Vector3.MoveTowards(currentPos, goal, currentSpeed * Time.deltaTime);
        next.y = yBase + Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        // 6) Apply movement safely with or without physics
        if (rb != null && !rb.isKinematic)
            rb.MovePosition(next);
        else
            transform.position = next;
    }

    private void PickNewTarget(bool forceBig)
    {
        nextPick = Time.time + Random.Range(repickMinTime, repickMaxTime);

        bool bigJump = forceBig || (Random.value < bigJumpChance);
        float radius = bigJump ? bigMoveRadius : smallMoveRadius;

        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 candidate = new Vector3(home.x + randomCircle.x, yBase, home.z + randomCircle.y);

        goal = ClampToHome(candidate);
        currentSpeed = Random.Range(baseSpeed, maxSpeed);
    }

    private void PickFleeTarget()
    {
        nextPick = Time.time + Random.Range(0.2f, 0.5f);

        if (targetToAvoid == null)
        {
            PickNewTarget(forceBig: true);
            return;
        }

        Vector3 fromPos = rb != null ? rb.position : transform.position;

        Vector3 away = (fromPos - targetToAvoid.position);
        away.y = 0f;

        // If we're basically on top of target, pick a random direction
        if (away.sqrMagnitude < 0.0001f)
        {
            Vector2 r = Random.insideUnitCircle.normalized;
            away = new Vector3(r.x, 0f, r.y);
        }

        away.Normalize();

        float jumpDistance = Random.Range(smallMoveRadius, bigMoveRadius);
        Vector3 candidate = fromPos + away * jumpDistance;

        goal = ClampToHome(candidate);
        currentSpeed = Random.Range(maxSpeed * 0.7f, maxSpeed);
    }

    private Vector3 ClampToHome(Vector3 candidate)
    {
        Vector3 flat = candidate - home;
        flat.y = 0f;

        if (flat.magnitude > maxDistanceFromHome)
        {
            flat = flat.normalized * maxDistanceFromHome;
            candidate = home + flat;
            candidate.y = yBase;
        }

        return candidate;
    }

    private Transform FindBestCameraTransform()
    {
        // 1) Camera.main if tagged properly
        if (Camera.main != null) return Camera.main.transform;

        // 2) Any enabled camera in scene
        Camera[] cams = FindObjectsOfType<Camera>(true);
        foreach (var c in cams)
        {
            if (c != null && c.enabled) return c.transform;
        }

        return null; // still ok; star will just wander without fleeing
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Vector3 drawHome = (home == Vector3.zero) ? transform.position : home;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(drawHome, maxDistanceFromHome);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(goal, 0.05f);
    }
}