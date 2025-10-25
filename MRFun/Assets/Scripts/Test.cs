using UnityEngine;

public class SwordBladeAimAwayFromLeft : MonoBehaviour
{
    [Header("Hierarchy")]
    [Tooltip("Child transform that holds the MeshRenderer and/or CapsuleCollider (the blade).")]
    public Transform blade; // child with mesh/collider

    [Header("Controller Anchors")]
    public Transform leftController;   // aim AWAY from this
    public Transform rightController;  // base should sit here

    [Header("Visibility")]
    [Tooltip("Hide blade if controller distance falls below this.")]
    public float minVisibleLength = 0.001f;

    [Header("Offsets")]
    [Tooltip("Local offset from the RIGHT controller anchor (e.g., grip tip).")]
    public Vector3 startOffsetLocal = Vector3.zero;

    // optional caches
    MeshRenderer bladeRenderer;
    CapsuleCollider bladeCapsule;
    MeshFilter bladeMeshFilter;

    void Awake()
    {
        if (!blade) return;
        bladeRenderer   = blade.GetComponent<MeshRenderer>();
        bladeCapsule    = blade.GetComponent<CapsuleCollider>();
        bladeMeshFilter = blade.GetComponent<MeshFilter>();
        // We are NOT changing collider/mesh sizes here.
    }

    void LateUpdate()
    {
        if (!blade || !leftController || !rightController) return;

        // Start point (right controller anchor + optional local offset)
        Vector3 start = rightController.TransformPoint(startOffsetLocal);

        // Direction = directly AWAY from the left controller
        Vector3 away = rightController.position - leftController.position;
        float dist   = away.magnitude;

        bool show = dist >= minVisibleLength;
        if (bladeRenderer) bladeRenderer.enabled = show;
        if (bladeCapsule)  bladeCapsule.enabled  = show;
        if (!show) return;

        Vector3 dir = away / dist;

        // 1) Rotation: align blade +Y with dir
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);
        blade.rotation = rot;

        // 2) Position: put the blade's base exactly at `start`.
        //    Since we're NOT scaling, offset the blade's CENTER by its OWN half-length in world units.
        float halfLenWorld = GetBladeHalfLengthWorld();
        Vector3 center     = start + dir * halfLenWorld;
        blade.position     = center;
    }

    float GetBladeHalfLengthWorld()
    {
        // Prefer mesh bounds along local Y (works even if not a capsule)
        if (bladeMeshFilter && bladeMeshFilter.sharedMesh)
        {
            // sharedMesh.bounds is in the blade's LOCAL space.
            float localSizeY = bladeMeshFilter.sharedMesh.bounds.size.y;
            float worldScaleY = blade.lossyScale.y;
            return 0.5f * localSizeY * worldScaleY;
        }

        // Fallback: capsule height in local units scaled by lossy Y
        if (bladeCapsule)
        {
            return 0.5f * bladeCapsule.height * blade.lossyScale.y;
        }

        // Last resort: assume Unity cylinder-like defaults (height ~2 in local units)
        return 0.5f * 2f * blade.lossyScale.y;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SceneObject"))
        {
            Debug.Log("Blade entered SceneObject: " + other.name);
            // TODO: add your hit or interaction logic here
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SceneObject"))
        {
            Debug.Log("Blade exited SceneObject: " + other.name);
            // TODO: add your cleanup logic here
        }
    }

    
    
}
