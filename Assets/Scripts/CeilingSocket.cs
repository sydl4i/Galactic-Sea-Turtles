using UnityEngine;

public class CeilingSocket : MonoBehaviour
{
    public bool isFilled = false;
    private Renderer myRenderer;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        /*// Check if the projectile is one of our shot cubes
        if (!isFilled && (other.gameObject.name.Contains("Cube") || other.gameObject.name.Contains("Star")))
        {
            CaptureStar(other.gameObject);
        }*/
        StarData data = other.GetComponent<StarData>();
        if (!isFilled && data != null)
        {
            isFilled = true;

            // Take the color from the star's data
            myRenderer.material.color = data.starColor;

            Destroy(other.gameObject);
            SendMessageUpwards("CheckForWin");
        }
    }
    /*
    void CaptureStar(GameObject star)
    {
        isFilled = true;
        Destroy(star); // Remove the flying physics cube

        // Visual feedback: the socket "glows"
        myRenderer.material.color = filledColor;

        // Notify the parent manager to check if all 5 are full
        SendMessageUpwards("CheckForWin");
    }*/
}