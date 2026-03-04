using UnityEngine;
using Oculus.Interaction;

public class Star2_Mischievous : MonoBehaviour
{
    [Header("Movement Bounds")]
    public float radiusMeters = 0.6096f;        // 2 ft
    public float minLingerSeconds = 2.0f;
    public float maxLingerSeconds = 3.5f;

    [Header("Sudden Hop")]
    public float hopDistanceMin = 0.08f;
    public float hopDistanceMax = 0.22f;
    public float hopDuration = 0.08f;

    [Header("Idle Bob")]
    public float bobAmplitude = 0.02f;
    public float bobSpeed = 2.0f;

    private Rigidbody rb;
    private Grabbable grabbable;

    private Vector3 anchorPos;
    private bool isGrabbed = false;
    private float nextMoveTime;

    private bool hopping = false;
    private float hopT = 0f;
    private Vector3 hopFrom, hopTo;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<Grabbable>();
    }

    void Start()
    {
        anchorPos = transform.position;
        ScheduleNextMove();
    }

    void Update()
    {
        // Detect grabbed state via Grabbable (no HandGrabInteractable reference)
        bool currentlyGrabbed = (grabbable != null && grabbable.SelectingPointsCount > 0);
        if (currentlyGrabbed != isGrabbed)
        {
            SetGrabbed(currentlyGrabbed);
        }

        if (isGrabbed) return;

        // idle bob
        if (!hopping && bobAmplitude > 0f)
        {
            Vector3 p = transform.position;
            p.y = anchorPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.position = p;
        }

        if (!hopping && Time.time >= nextMoveTime)
        {
            StartHop();
        }

        if (hopping)
        {
            hopT += Time.deltaTime / Mathf.Max(0.001f, hopDuration);
            float t = Mathf.Clamp01(hopT);
            float eased = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(hopFrom, hopTo, eased);

            if (t >= 1f)
            {
                hopping = false;
                ScheduleNextMove();
            }
        }
    }

    private void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;

        hopping = false;
        hopT = 0f;

        if (rb != null)
        {
            rb.isKinematic = grabbed;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (!grabbed)
        {
            anchorPos = transform.position; // new placement spot becomes the anchor
            ScheduleNextMove();
        }
    }

    private void StartHop()
    {
        hopFrom = transform.position;

        Vector3 dir = Random.onUnitSphere;
        dir.y *= 0.3f;
        dir.Normalize();

        float dist = Random.Range(hopDistanceMin, hopDistanceMax);
        Vector3 candidate = hopFrom + dir * dist;

        Vector3 offset = candidate - anchorPos;
        if (offset.magnitude > radiusMeters)
        {
            candidate = anchorPos + offset.normalized * radiusMeters;
        }

        hopTo = candidate;
        hopping = true;
        hopT = 0f;
    }

    private void ScheduleNextMove()
    {
        nextMoveTime = Time.time + Random.Range(minLingerSeconds, maxLingerSeconds);
    }
}