using UnityEngine;

public class StaffController : MonoBehaviour
{
    // Set up that is not relevant yet -syd
    /*[Header("Visual States")]
    public GameObject[] staffStages; // Drag your different staff meshes here
    private int currentStars = 0;
    */

    // Drag the Cylinder (child) into this slot in the Inspector
    [Header("References")]
    public Renderer staffRenderer;
    public GameObject cubePrefab;    // Your Cube Prefab with a Rigidbody
    public Transform shootPoint;     // An Empty at the tip of the staff

    [Header("Settings")]
    public float shootForce = 500f;
    private int storedStars = 0;

    void Update()
    {
        // Check for Right Index Trigger press
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && storedStars > 0)
        {
            ShootCube();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*// Check if the object entering the trigger is a star
        // Ensure your Star prefab has the Tag "Star"
        if (other.CompareTag("Star"))
        {
            AddStar(other.gameObject);
        }*/

        // We check for the name "Sphere" since that is the default Building Block name
        if (other.gameObject.name.Contains("Cube"))
        {
            storedStars++;
            staffRenderer.material.color = Color.yellow; // Visual feedback
            Destroy(other.gameObject);
            Debug.Log("Stars Stored: " + storedStars);
        }
    }

    void ShootCube()
    {
        storedStars--;

        // Spawn and launch
        GameObject newCube = Instantiate(cubePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = newCube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootPoint.forward * shootForce);
        }

        // Return to white if empty
        if (storedStars <= 0) staffRenderer.material.color = Color.white;
    }

    /*void AddStar(GameObject starInstance)
    {
        if (currentStars < staffStages.Length - 1)
        {
            // 1. Remove the star from the player's hand
            Destroy(starInstance);

            // 2. Increment star count
            currentStars++;

            // 3. Update the staff visual
            UpdateStaffMesh();

            Debug.Log("Star collected! Current Level: " + currentStars);
        }
    }

    void UpdateStaffMesh()
    {
        for (int i = 0; i < staffStages.Length; i++)
        {
            // Enable the mesh that matches our current star count, disable others
            staffStages[i].SetActive(i == currentStars);
        }
    }*/
}
