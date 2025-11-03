using System;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    private Material originalMaterial;
    private Material currentGlowMat;
    
    public UnityEvent onInteract;
    
    public AudioClip hitEffect;
    
    [SerializeField] private Material glowMaterial;

    private bool isGlowing;

    private Renderer rend;
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        
        // Save original material
        originalMaterial = rend.sharedMaterial;
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
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            StartGlow();
            DemoGameManager.Instance.PlaySound(hitEffect);
            
            onInteract?.Invoke();
            // GameManager.Instance.ChangeMode(modeChange);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            StopGlow();
        }
    }
}
