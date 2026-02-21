using UnityEngine;

public class SpawnInFront : MonoBehaviour
{
    public Transform centerEye;   // drag the camera from the rig here
    public GameObject starPrefab;

    void Start()
    {
        Vector3 forwardFlat = centerEye.forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();

        Vector3 spawnPos = centerEye.position + forwardFlat * 1.5f + Vector3.up * 0.2f;
        Instantiate(starPrefab, spawnPos, Quaternion.identity);
    }
}
