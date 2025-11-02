using UnityEngine;

public class DaggerSpawner : MonoBehaviour
{
    [Header("Dagger Settings")]
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private Transform rightHandAnchor; // Assign hand transform in inspector
    
    [Header("Normal Grip Position")]
    [SerializeField] private Vector3 normalLocalPosition = new Vector3(0, 0, 0.05f);
    [SerializeField] private Vector3 normalLocalRotation = new Vector3(0, 0, 0);
    
    [Header("Icepick Grip Position")]
    [SerializeField] private Vector3 icepickLocalPosition = new Vector3(0, 0, 0.05f);
    [SerializeField] private Vector3 icepickLocalRotation = new Vector3(180, 0, 0);
    
    [Header("Input Settings")]
    [SerializeField] private float gripThreshold = 0.7f;
    
    private GameObject spawnedDagger;
    private bool wasGripping = false;
    private bool isIcepickGrip = false;

    void Update()
    {
        float gripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool isGripping = gripValue > gripThreshold;

        // Check B button for grip style toggle
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            isIcepickGrip = !isIcepickGrip;
            
            // Update position if dagger is already spawned
            if (spawnedDagger != null)
            {
                UpdateDaggerPosition();
            }
        }

        // Grip just pressed - spawn dagger
        if (isGripping && !wasGripping)
        {
            SpawnDagger();
        }
        // Grip just released - destroy dagger
        else if (!isGripping && wasGripping)
        {
            DespawnDagger();
        }

        wasGripping = isGripping;
    }

    void SpawnDagger()
    {
        if (spawnedDagger != null) return; // Already spawned

        spawnedDagger = Instantiate(daggerPrefab, rightHandAnchor);
        UpdateDaggerPosition();
    }

    void UpdateDaggerPosition()
    {
        if (spawnedDagger == null) return;

        if (isIcepickGrip)
        {
            spawnedDagger.transform.localPosition = icepickLocalPosition;
            spawnedDagger.transform.localRotation = Quaternion.Euler(icepickLocalRotation);
        }
        else
        {
            spawnedDagger.transform.localPosition = normalLocalPosition;
            spawnedDagger.transform.localRotation = Quaternion.Euler(normalLocalRotation);
        }
    }

    void DespawnDagger()
    {
        if (spawnedDagger != null)
        {
            Destroy(spawnedDagger);
            spawnedDagger = null;
        }
    }
}