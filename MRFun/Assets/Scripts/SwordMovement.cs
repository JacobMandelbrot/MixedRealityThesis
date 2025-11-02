using UnityEngine;
using UnityEngine.XR;

public class SwordController : MonoBehaviour
{
    [Header("Controller Settings")]
    [Tooltip("The right controller anchor (usually OVRCameraRig's RightHandAnchor)")]
    public Transform rightControllerAnchor;
    
    [Header("Rotation Offset")]
    [Tooltip("Adjust these values to orient the sword correctly in your hand")]
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);
    
    [Header("Position Offset")]
    [Tooltip("Optional position offset from the controller")]
    public Vector3 positionOffset = Vector3.zero;
    
    [Header("Follow Settings")]
    [Tooltip("Should the sword follow smoothly or snap to controller")]
    public bool smoothFollow = false;
    
    [Tooltip("If smooth follow is enabled, how fast should it follow")]
    public float followSpeed = 15f;

    private Quaternion offsetRotation;

    void Start()
    {
        // Convert the rotation offset to a quaternion
        offsetRotation = Quaternion.Euler(rotationOffset);
        
        // Try to find the right controller anchor automatically if not assigned
        if (rightControllerAnchor == null)
        {
            GameObject ovrCameraRig = GameObject.Find("OVRCameraRig");
            if (ovrCameraRig != null)
            {
                Transform rightHand = ovrCameraRig.transform.Find("TrackingSpace/RightHandAnchor");
                if (rightHand != null)
                {
                    rightControllerAnchor = rightHand;
                    Debug.Log("SwordController: Automatically found RightHandAnchor");
                }
                else
                {
                    Debug.LogWarning("SwordController: Could not find RightHandAnchor. Please assign manually.");
                }
            }
        }
    }

    void Update()
    {
        if (rightControllerAnchor == null)
        {
            Debug.LogWarning("SwordController: Right controller anchor is not assigned!");
            return;
        }

        // Calculate target position and rotation
        Vector3 targetPosition = rightControllerAnchor.position + rightControllerAnchor.TransformDirection(positionOffset);
        Quaternion targetRotation = rightControllerAnchor.rotation * offsetRotation;

        if (smoothFollow)
        {
            // Smooth interpolation
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
        else
        {
            // Direct snap
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }
    }

    // Helper method to update rotation offset at runtime (useful for testing)
    public void SetRotationOffset(Vector3 newOffset)
    {
        rotationOffset = newOffset;
        offsetRotation = Quaternion.Euler(rotationOffset);
    }

    // Optional: Draw gizmos in editor to visualize the attachment
    void OnDrawGizmos()
    {
        if (rightControllerAnchor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rightControllerAnchor.position, transform.position);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 0.3f);
        }
    }
    
        void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("WeakSpot"))
        {
            Debug.Log("Blade entered WeakSpot: " + other.name);
            var fw = other.GetComponent<FlashWhenHit>();
            if (fw) fw.Flash();
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("SceneObject"))
        {
            Debug.Log("Blade exited SceneObject: " + other.name);
        }
    }
}

    
    

