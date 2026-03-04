using UnityEngine;

public class CeilingSocket : MonoBehaviour
{
    public bool isFilled = false;
    public Color filledColor = Color.yellow;
    private Renderer myRenderer;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the projectile is one of our shot cubes
        if (!isFilled && (other.gameObject.name.Contains("Cube") || other.gameObject.name.Contains("Star")))
        {
            CaptureStar(other.gameObject);
        }
    }

    void CaptureStar(GameObject star)
    {
        isFilled = true;
        Destroy(star); // Remove the flying physics cube

        // Visual feedback: the socket "glows"
        myRenderer.material.color = filledColor;

        // Notify the parent manager to check if all 5 are full
        SendMessageUpwards("CheckForWin");
    }
}