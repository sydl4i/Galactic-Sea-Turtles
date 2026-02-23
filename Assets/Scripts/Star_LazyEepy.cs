using UnityEngine;

public class LazyStarSleepy : MonoBehaviour
{
    [Header("Drift Area")]
    public float driftRadius = 1.2f;
    public float repickMinTime = 2.0f;
    public float repickMaxTime = 4.5f;

    [Header("Height")]
    public float floatHeight = 1.2f;


    [Header("Speed (Sleepy)")]
    public float minSpeed = 0.05f;
    public float maxSpeed = 0.18f;
    public float turnSmooth = 1.5f;

    [Header("Bobbing")]
    public float bobAmount = 0.12f;
    public float bobSpeed = 0.9f;

    [Header("Sleep Behavior")]
    public float sleepChance = 0.35f;       // chance to fall asleep when picking new target
    public float sleepMinTime = 1.5f;
    public float sleepMaxTime = 3.5f;
    public float sleepyBobMultiplier = 0.5f;
    public float sleepyRotationAmount = 12f;

    [Header("Optional")]
    public bool lockBaseHeight = true;

    private Vector3 home;
    private Vector3 target;
    private float nextPickTime;
    private float currentSpeed;
    private float yBase;
    private Vector3 velocity;

    private bool isSleeping = false;
    private float wakeTime = 0f;

    void Start()
{
    home = transform.position;

    yBase = floatHeight;
    home.y = yBase;
    transform.position = home;

    PickNewTarget();
}

    void Update()
    {
        // Sleep state
        if (isSleeping)
        {
            if (Time.time >= wakeTime)
            {
                isSleeping = false;
                PickNewTarget();
            }

            SleepMotion();
            return;
        }

        // Periodically choose new drift target
        if (Time.time >= nextPickTime)
            PickNewTarget();

        DriftMotion();
    }

    private void DriftMotion()
    {
        Vector3 toTarget = (target - transform.position);
        toTarget.y = 0f;

        Vector3 desiredDir = toTarget.sqrMagnitude < 0.0001f ? Vector3.zero : toTarget.normalized;
        Vector3 desiredVel = desiredDir * currentSpeed;

        velocity = Vector3.Lerp(velocity, desiredVel, turnSmooth * Time.deltaTime);

        Vector3 next = transform.position + velocity * Time.deltaTime;

        // Keep within radius
        Vector3 fromHome = next - home;
        fromHome.y = 0f;
        if (fromHome.magnitude > driftRadius)
        {
            Vector3 inward = (-fromHome.normalized) * currentSpeed;
            velocity = Vector3.Lerp(velocity, inward, (turnSmooth * 2f) * Time.deltaTime);
            next = transform.position + velocity * Time.deltaTime;
        }

        // Bobbing
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        next.y = (lockBaseHeight ? yBase : transform.position.y) + bob;

        transform.position = next;

        // Gentle facing direction
        if (velocity.sqrMagnitude > 0.00001f)
        {
            Quaternion look = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 0.6f * Time.deltaTime);
        }
    }

    private void SleepMotion()
    {
        float sleepyBob = Mathf.Sin(Time.time * (bobSpeed * 0.5f)) * (bobAmount * sleepyBobMultiplier);

        Vector3 pos = transform.position;
        pos.y = yBase + sleepyBob;
        transform.position = pos;

        // Gentle nodding rotation
        float sway = Mathf.Sin(Time.time * 0.7f) * sleepyRotationAmount;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, sway);
    }

    private void PickNewTarget()
    {
        nextPickTime = Time.time + Random.Range(repickMinTime, repickMaxTime);

        // Decide whether to fall asleep
        if (Random.value < sleepChance)
        {
            isSleeping = true;
            wakeTime = Time.time + Random.Range(sleepMinTime, sleepMaxTime);
            velocity = Vector3.zero;
            return;
        }

        currentSpeed = Random.Range(minSpeed, maxSpeed);

        Vector2 r = Random.insideUnitCircle * driftRadius;
        target = new Vector3(home.x + r.x, yBase, home.z + r.y);
    }
}
