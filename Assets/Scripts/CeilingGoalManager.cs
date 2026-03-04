using UnityEngine;
using Meta.XR.MRUtilityKit; // Make sure you have the MRUK namespace

public class CeilingGoalManager : MonoBehaviour
{
    private CeilingSocket[] sockets;
    public GameObject galaxyVisual; // An object with a galaxy texture/particles

    void Start()
    {
        sockets = GetComponentsInChildren<CeilingSocket>();
        if (galaxyVisual) galaxyVisual.SetActive(false);

        // AUTO-SCALE LOGIC
        // MRUKAnchor is added to this object by the Spawner at runtime
        MRUKAnchor anchor = GetComponent<MRUKAnchor>();
        if (anchor != null && galaxyVisual != null)
        {
            // Get the dimensions of the ceiling from MRUK
            Vector3 ceilingSize = anchor.PlaneRect.Value.size;

            // Unity Planes are 10m x 10m at scale 1. 
            // We divide by 10 to match the real-world meters.
            galaxyVisual.transform.localScale = new Vector3(ceilingSize.x / 10f, 1f, ceilingSize.y / 10f);
        }
    }

    public void CheckForWin()
    {
        int count = 0;
        foreach (var s in sockets) if (s.isFilled) count++;

        if (count >= 5) galaxyVisual.SetActive(true);
    }
}