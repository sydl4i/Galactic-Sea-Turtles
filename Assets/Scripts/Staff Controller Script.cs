using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaffController : MonoBehaviour
{
    [Header("Brute Force Setup")]
    public GameObject[] gemMeshes;   // Drag child gems here (0-4)
    public GameObject[] starPrefabs; // Drag star prefabs from folder here (0-4)

    [Header("References")]
    public Renderer staffRenderer;
    public Transform shootPoint;
    public float shootForce = 5f; // Increased for better feel
    public float animationSpeed = 5f;

    // The Queue now only needs to store the ID (0, 1, 2, 3, or 4)
    private Queue<int> starIDQueue = new Queue<int>();
    private Vector3[] originalScales; // This stores those specific 0.1102... numbers

    void Start()
    {
        // Initialize the array to match the number of gems
        originalScales = new Vector3[gemMeshes.Length];

        for (int i = 0; i < gemMeshes.Length; i++)
        {
            if (gemMeshes[i] != null)
            {
                // Record the "Artist's Scale" before we do anything
                originalScales[i] = gemMeshes[i].transform.localScale;

                // Set them to zero scale immediately so they are ready to grow
                gemMeshes[i].transform.localScale = Vector3.zero;

                // Ensure they are inactive at the very start
                gemMeshes[i].SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StarData data = other.GetComponentInParent<StarData>();

        if (data != null)
        {
            // 1. Tactile Feedback
            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
            Invoke("StopVibration", 0.1f);

            // 2. Add the ID to the Queue
            starIDQueue.Enqueue(data.starID);

            // 3. Animate the Gem appearing
            if (data.starID >= 0 && data.starID < gemMeshes.Length)
            {
                // Pass the specific original scale to the animation
                StartCoroutine(AnimateGem(data.starID, true));
            }

            // 5. Goodbye original star
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        // Check for Right Trigger pull
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            if (starIDQueue.Count > 0)
            {
                ShootNextStar();
            }
        }
    }

    void ShootNextStar()
    {
        // Get the next ID in line
        int id = starIDQueue.Dequeue();

        // BRUTE FORCE: Use the ID to grab the prefab directly from our array
        if (id >= 0 && id < starPrefabs.Length)
        {
            GameObject p = Instantiate(starPrefabs[id], shootPoint.position, shootPoint.rotation);

            // Ensure physics are active on the new star
            Rigidbody rb = p.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true; // Make sure gravity is on for the flight
                rb.AddForce(shootPoint.forward * shootForce, ForceMode.VelocityChange);
            }
        }

        // Animate the Gem disappearing
        if (id >= 0 && id < gemMeshes.Length)
        {
            StartCoroutine(AnimateGem(id, false));
        }

        // Haptic "Click"
        OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
        Invoke("StopVibration", 0.05f);
    }

    IEnumerator AnimateGem(int index, bool appearing)
    {
        GameObject gem = gemMeshes[index];
        Vector3 targetMaxScale = originalScales[index]; // Use the unique scale we saved

        gem.SetActive(true);

        Vector3 startScale = appearing ? Vector3.zero : targetMaxScale;
        Vector3 endScale = appearing ? targetMaxScale : Vector3.zero;

        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime * animationSpeed;
            gem.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        gem.transform.localScale = endScale;
        if (!appearing) gem.SetActive(false);
    }

    void StopVibration()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}

/*using UnityEngine;
using System.Collections.Generic;

public class StaffController : MonoBehaviour
{
    /*
    // Drag the Cylinder (child) into this slot in the Inspector
    [Header("References")]
    public Renderer staffRenderer;
    public GameObject cubePrefab;    // Your Cube Prefab with a Rigidbody
    public Transform shootPoint;     // An Empty at the tip of the staff

    [Header("Settings")]
    public float shootForce = 300f;
    private int storedStars = 0;*/

/*
   [System.Serializable]
   public struct GemMapping
   {
       public string starName;
       public GameObject gemMesh; // The child object on the staff
   }

   public List<GemMapping> gemMappings; // Assign these in Inspector
   public Transform shootPoint;
   public float shootForce = 15f;
   public Renderer staffRenderer;

   private Queue<StarData> starQueue = new Queue<StarData>();*/

// Brute force: Drag your 5 Gem child-objects here in order
// 0=Curious, 1=Happy, 2=Lazy, 3=Mischevious, 4=Shy
/*public GameObject[] gemMeshes;
    public Renderer staffRenderer;
    public Transform shootPoint;
    public float shootForce = 15f;

    // A simple struct to hold the star's info in the queue
    struct StoredStar
    {
        public GameObject prefab;
        public int gemIndex;
        public Color color;
    }

    private Queue<StoredStar> starQueue = new Queue<StoredStar>();

    private void OnTriggerEnter(Collider other)
    {
        /*Check if the object entering the trigger is a star
        // Ensure your Star prefab has the Tag "Star"
        if (other.CompareTag("Star"))
        {
            AddStar(other.gameObject);
        }*/

// We check for the name "Sphere" since that is the default Building Block name
/*if (other.gameObject.name.Contains("Cube") || other.gameObject.name.Contains("Star"))
{
    storedStars++;
    staffRenderer.material.color = Color.yellow; // Visual feedback
    Destroy(other.gameObject);
    Debug.Log("Stars Stored: " + storedStars);
}*/
/*
StarData data = other.GetComponent<StarData>();
if (data != null)
{
    // HAPTIC FEEDBACK: Success Thump (Frequency, Amplitude, Controller)
    OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
    Invoke("StopVibration", 0.1f); // Stop after 0.1 seconds

    if (staffRenderer != null)
    {
        staffRenderer.material.color = data.starColor;
    }

    // 1. Add data to Queue
    starQueue.Enqueue(data);

    // 2. Turn on the corresponding Gem on the staff
    ToggleGem(data.starName, true);

    Destroy(other.gameObject);
}*/
/*
StarData data = other.GetComponentInParent<StarData>();

if (data != null)
{
    // Vibrate so you know the hit registered
    OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
    Invoke("StopVibration", 0.1f);

    // BRUTE FORCE: We identify the star by its "ID" number
    // You'll add an 'int starID' to StarData
    int id = data.starID;

    StoredStar newStar;
    newStar.prefab = data.starPrefab;
    newStar.gemIndex = id;
    newStar.color = data.staffColor;

    starQueue.Enqueue(newStar);

    // Turn on the gem directly using the ID
    if (id >= 0 && id < gemMeshes.Length) gemMeshes[id].SetActive(true);

    // Change color using the most compatible method
    staffRenderer.material.SetColor("_BaseColor", data.staffColor);
    staffRenderer.material.color = data.staffColor;

    Destroy(other.gameObject);
}
}
/*
void Update()
{
if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && starQueue.Count > 0)
{
    ShootNextStar();
}
}
/*
void ShootNextStar()
{
/*storedStars--;

// Spawn and launch
GameObject newCube = Instantiate(cubePrefab, shootPoint.position, shootPoint.rotation);
Rigidbody rb = newCube.GetComponent<Rigidbody>();
if (rb != null)
{
    rb.AddForce(shootPoint.forward * shootForce);
}

// Return to white if empty
if (storedStars <= 0) staffRenderer.material.color = Color.white;*/
/*StarData dataToShoot = starQueue.Dequeue();

// 1. Spawn the character star
GameObject projectile = Instantiate(dataToShoot.starPrefab, shootPoint.position, shootPoint.rotation);

// 2. Physics launch
Rigidbody rb = projectile.GetComponent<Rigidbody>();
if (rb != null)
{
    rb.isKinematic = false;
    rb.AddForce(shootPoint.forward * shootForce, ForceMode.VelocityChange);
}

// 3. Turn off that specific Gem mesh on the staff
ToggleGem(dataToShoot.starName, false);

// HAPTIC FEEDBACK: Light Click
OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
Invoke("StopVibration", 0.05f);*/
/*
StoredStar toShoot = starQueue.Dequeue();

GameObject p = Instantiate(toShoot.prefab, shootPoint.position, shootPoint.rotation);
Rigidbody rb = p.GetComponent<Rigidbody>();
if (rb != null)
{
    rb.isKinematic = false;
    rb.AddForce(shootPoint.forward * shootForce, ForceMode.VelocityChange);
}

// Turn off the gem directly
if (toShoot.gemIndex >= 0 && toShoot.gemIndex < gemMeshes.Length)
{
    gemMeshes[toShoot.gemIndex].SetActive(false);
}

// Haptic click
OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
Invoke("StopVibration", 0.05f);
}
/*
void ToggleGem(string name, bool state)
{
bool found = false;
foreach (var mapping in gemMappings)
{
    // Trim and lower-case to prevent "Lazy " vs "Lazy" errors
    if (mapping.starName.Trim().ToLower() == name.Trim().ToLower())
    {
        if (mapping.gemMesh != null)
        {
            mapping.gemMesh.SetActive(state);
            found = true;
        }
    }
}

// DEBUG COLOR: If no gem name matches, turn the staff Red so you know!
if (!found && state == true) staffRenderer.material.color = Color.red;
}*/
/*
void StopVibration()
{
OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
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
//}
