using UnityEngine;

public class StarCollisionScript : MonoBehaviour
{
    // Set up that is not relevant yet -syd
    /*[Header("Visual States")]
    public GameObject[] staffStages; // Drag your different staff meshes here
    private int currentStars = 0;
    */

    // Drag the Cylinder (child) into this slot in the Inspector
    public Renderer staffRenderer;

    private void OnTriggerEnter(Collider other)
    {
        /*// Check if the object entering the trigger is a star
        // Ensure your Star prefab has the Tag "Star"
        if (other.CompareTag("Star"))
        {
            AddStar(other.gameObject);
        }*/

        // We check for the name "Sphere" since that is the default Building Block name
        if (other.gameObject.name.Contains("Sphere"))
        {
            ChangeStaffColor();

            // Destroy the sphere so it looks like it was "absorbed"
            Destroy(other.gameObject);
        }
    }

    void ChangeStaffColor()
    {
        // Changes the cylinder to a random color to show the hit registered
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        staffRenderer.material.color = randomColor;

        Debug.Log("Star absorbed! Color changed.");
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
