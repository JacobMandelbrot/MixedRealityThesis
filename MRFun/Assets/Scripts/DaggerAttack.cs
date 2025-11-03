using System.Collections;
using UnityEngine;

public class DaggerAttack : MonoBehaviour
{
    [SerializeField] private Material glowMaterial;
    [SerializeField] private float glowSpeed = 5f;
    
    private Renderer rend;
    private Material originalMaterial;
    private Material currentGlowMat;
    private bool isGlowing = false;
    
    // For velocity calculation
    private Vector3 lastPosition;
    private float currentSpeed;

    public AudioClip hitEffect;
    void Awake()
    {
        rend = GetComponent<Renderer>();
        
        // Save original material
        originalMaterial = rend.sharedMaterial;
        
        // Initialize position tracking
        lastPosition = transform.position;
    }
    
    void Update()
    {
        // Calculate speed based on position change
        currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
        
        
        // Start glowing when moving fast
        if (currentSpeed > glowSpeed && !isGlowing)
        {
           
        }
        // Stop glowing when slow
        else if (currentSpeed < glowSpeed && isGlowing)
        {
            
        }
    }
    
    void StartGlow()
    {
        isGlowing = true;
        
        // Create instance of glow material
        currentGlowMat = new Material(glowMaterial);
        rend.material = currentGlowMat;
    }
    
    void StopGlow()
    {
        isGlowing = false;
        
        // Restore original
        rend.sharedMaterial = originalMaterial;
        
        // Clean up instance
        if (currentGlowMat != null)
        {
            Destroy(currentGlowMat);
        }
    }
    
    void OnDestroy()
    {
        // Always clean up material instances
        if (currentGlowMat != null)
        {
            Destroy(currentGlowMat);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flash"))
        {
            Debug.Log("Blade entered WeakSpot: " + other.name);
            var fw = other.GetComponent<FlashWhenHit>();
            if (fw) fw.Flash();
            
            DemoGameManager.Instance.PlaySound(hitEffect);
            TriggerHapticHalfSecond(OVRInput.Controller.RTouch);
        }
        
        if (other.CompareTag("WeakSpot"))
        {
            Debug.Log("Blade entered WeakSpot: " + other.name);
            var fw = other.GetComponentInParent<CycleWeakSpots>();
            fw.ActivateRandomWeakSpot();
            other.gameObject.SetActive(false);
            
            DemoGameManager.Instance.PlaySound(hitEffect);
            TriggerHapticHalfSecond(OVRInput.Controller.RTouch);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeakSpot"))
        {
            Debug.Log("Blade exited SceneObject: " + other.name);
        }
    }
    
    // Trigger haptic for half a second
    public void TriggerHapticHalfSecond(OVRInput.Controller controller, float frequency = 0.5f, float amplitude = 0.8f)
    {
        StartCoroutine(HapticPulse(controller, frequency, amplitude, 0.5f));
    }
    
    // Coroutine to handle timed haptic feedback
    private IEnumerator HapticPulse(OVRInput.Controller controller, float frequency, float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controller);
    }
    
    // Example usage

}