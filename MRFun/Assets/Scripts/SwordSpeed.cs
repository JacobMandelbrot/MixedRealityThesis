using UnityEngine;

[DisallowMultipleComponent]
public class SwordSpeed : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;                 // Blade's rigidbody
    [SerializeField] private Renderer[] renderers;         // Mesh/SkinnedMesh renderers to tint
    [SerializeField] private bool includeChildren = true;  // Auto-find child renderers

    [Header("Speed Threshold")]
    [Min(0f)] public float speedThreshold = 2.0f;          // m/s
    public bool useSmoothBlend = true;                      // Blend instead of hard switch

    [Header("Colors")]
    public Color slowColor = Color.white;
    public Color fastColor = Color.green;

    [Header("Debug (read-only)")]
    [SerializeField, Tooltip("Current linear speed (m/s).")] private float currentSpeed;

    // Property names (URP/HDRP use _BaseColor; Built-in uses _Color)
    static readonly int BaseColorProp = Shader.PropertyToID("_BaseColor");
    static readonly int ColorProp     = Shader.PropertyToID("_Color");

    MaterialPropertyBlock mpb;

    void Reset()
    {
        // Try to auto-wire on add
        rb = GetComponentInParent<Rigidbody>();
        CollectRenderers();
    }

    void OnValidate()
    {
        // Keep refs fresh in editor
        if (rb == null) rb = GetComponentInParent<Rigidbody>();
        if (renderers == null || renderers.Length == 0) CollectRenderers();
    }

    void Awake()
    {
        if (rb == null) rb = GetComponentInParent<Rigidbody>();
        CollectRenderers();
        mpb = new MaterialPropertyBlock();
    }

    void FixedUpdate()
    {
        if (rb == null || renderers == null || renderers.Length == 0) return;

        currentSpeed = rb.linearVelocity.magnitude; // linear speed in m/s

        // 0..1 factor above threshold (for smooth blend)
        float t = Mathf.Clamp01(speedThreshold <= 0f ? 1f : currentSpeed / speedThreshold);
        Color target = useSmoothBlend
            ? Color.Lerp(slowColor, fastColor, t)
            : (currentSpeed >= speedThreshold ? fastColor : slowColor);

        // Push color to all renderers via MPB
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor(BaseColorProp, target);
            mpb.SetColor(ColorProp,     target); // cover Built-in
            r.SetPropertyBlock(mpb);
        }
    }

    void CollectRenderers()
    {
        if (includeChildren)
            renderers = GetComponentsInChildren<Renderer>(true);
        else
            renderers = GetComponents<Renderer>();
    }
}
