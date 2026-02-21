using UnityEngine;

public class StarMischief : MonoBehaviour
{
    [Header("Avoid Target (leave empty to auto-find)")]
    public Transform targetToAvoid; // If empty, script will auto-find the active XR camera.

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

    void OnEnable()
    {
        // If this object is enabled after being moved/spawned, reset home correctly.
        home = transform.position;
        yBase = home.y;
        PickNewTarget(forceBig: false);
    }

    void Start()
    {
        // If user didn’t assign a target, find the active XR camera (works with Meta Camera Rig).
        if (targetToAvoid == null)
            targetToAvoid = FindBestCameraTransform();

        // Ensure goal + speed are initialized
        home = transform.position;
        yBase = home.y;
        PickNewTarget(forceBig: false);
    }

    void Update()
    {
        // 1) If grabbed (often becomes child of hand), don’t fight the grab.
        if (transform.parent != initialParent && transform.parent != null)
            return;

        // 2) If physics is driving it (non-kinematic), also avoid fighting.
        if (rb != null && !rb.isKinematic)
            return;

        // 3) Flee if target is close
        if (targetToAvoid != null)
        {
            float d = Vector3.Distance(transform.position, targetToAvoid.position);
            if (d < scareRadius)
            {
                PickFleeTarget();
            }
        }

        // 4) Random repick
        if (Time.time >= nextPick)
        {
            PickNewTarget(forceBig: false);
        }

        // 5) Move toward goal (XZ movement)
        Vector3 next = Vector3.MoveTowards(transform.position, goal, currentSpeed * Time.deltaTime);

        // 6) Bob (Y only)
        next.y = yBase + Mathf.Sin(Time.time * bobSpeed) * bobAmount;

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

        Vector3 away = (transform.position - targetToAvoid.position);
        away.y = 0f;

        // If we're basically on top of target, pick a random direction
        if (away.sqrMagnitude < 0.0001f)
        {
            Vector2 r = Random.insideUnitCircle.normalized;
            away = new Vector3(r.x, 0f, r.y);
        }

        away.Normalize();

        float jumpDistance = Random.Range(smallMoveRadius, bigMoveRadius);
        Vector3 candidate = transform.position + away * jumpDistance;

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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(home == Vector3.zero ? transform.position : home, maxDistanceFromHome);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(goal, 0.05f);
    }
}
