using UnityEngine;

public class ParalaxLayer : MonoBehaviour
{
 [Header("Parallax Effect")]
    [Range(0f, 1f)]
    public float parallaxStrength = 0.5f; 

    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.position - lastCamPos;

        // Move layer opposite of camera movement (parallax)
        transform.position += new Vector3(deltaMovement.x * parallaxStrength, deltaMovement.y * parallaxStrength, 0f);

        lastCamPos = cam.position;
    }
}
