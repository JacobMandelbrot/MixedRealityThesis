using UnityEngine;

[DisallowMultipleComponent]
public class ConnectGripWithColliders : MonoBehaviour
{
    [Header("Anchors")]
    public Transform leftTransform;
    public Transform rightTransform;

    [Header("Grip gating (OVR)")]
    public bool useAnalog = false;
    [Range(0f, 1f)] public float downThreshold = 0.75f;

    [Header("Extension")]
    [Tooltip("Meters beyond RIGHT along Left→Right")]
    public float extraDistance = 0.20f;
    [Tooltip("If L↔R distance exceeds this, hide extension & color main line red")]
    public float maxDistance = 0.80f;
    public Transform extendedPointMarker; // optional

    [Header("Visuals")]
    public float lineWidth = 0.01f;
    public Color lrColorNormal = Color.white; // L↔R
    public Color lrColorOverMax = Color.red;  // L↔R when too far
    public Color extColor = Color.blue;       // R→Ext (always blue)

    [Header("Collision (extension only)")]
    public bool useTriggers = true;
    [Tooltip("Capsule radius = lineWidth * 0.5 * scale (min 5mm)")]
    public float colliderRadiusScale = 0.75f;

    LineRenderer lrMain; // controllers line
    LineRenderer lrExt;  // blue extension
    CapsuleCollider extCapsule; // collider only for extension
    Rigidbody rb;

    void Awake()
    {
        // Create/attach main LR (L↔R)
        lrMain = gameObject.GetComponent<LineRenderer>();
        if (!lrMain) lrMain = gameObject.AddComponent<LineRenderer>();
        SetupLR(lrMain, lineWidth, lrColorNormal);

        // Create child for extension LR (R→Ext)
        var extGO = new GameObject("LR_Extension");
        extGO.transform.SetParent(transform, false);
        lrExt = extGO.AddComponent<LineRenderer>();
        SetupLR(lrExt, lineWidth, extColor);

        // Rigidbody so trigger callbacks fire
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;

        // Extension capsule (only this has a collider)
        extCapsule = extGO.AddComponent<CapsuleCollider>();
        extCapsule.isTrigger = useTriggers;
        extCapsule.direction = 1; // Y axis; we’ll rotate to match the segment

        HideAll();
    }

    void SetupLR(LineRenderer lr, float width, Color c)
    {
        lr.useWorldSpace = true;
        lr.widthMultiplier = width;
        lr.numCapVertices = 8;
        lr.numCornerVertices = 4;
        if (!lr.material) lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = c; lr.endColor = c;
        lr.material.color = c;
        lr.positionCount = 0;
        lr.enabled = false;
    }

    void Update()
    {
        if (!leftTransform || !rightTransform) { HideAll(); return; }

        // Input gating
        bool leftHeld, rightHeld;
#if OCULUS_INTEGRATION_PRESENT || UNITY_ANDROID || UNITY_EDITOR
        if (useAnalog)
        {
            var l = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
            var r = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
            leftHeld  = l >= downThreshold;
            rightHeld = r >= downThreshold;
        }
        else
        {
            leftHeld  = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            rightHeld = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        }
#else
        leftHeld = Input.GetMouseButton(0);
        rightHeld = Input.GetMouseButton(1);
#endif
        if (!(leftHeld && rightHeld)) { HideAll(); return; }

        // Points
        Vector3 pL = leftTransform.position;
        Vector3 pR = rightTransform.position;
        float distLR = Vector3.Distance(pL, pR);

        // Always draw L↔R line
        lrMain.enabled = true;
        lrMain.positionCount = 2;
        lrMain.SetPosition(0, pL);
        lrMain.SetPosition(1, pR);
        SetLRColor(lrMain, distLR > maxDistance ? lrColorOverMax : lrColorNormal);

        if (distLR > maxDistance)
        {
            // Too far: no extension/collider
            lrExt.enabled = false;
            lrExt.positionCount = 0;
            extCapsule.enabled = false;
            if (extendedPointMarker) extendedPointMarker.gameObject.SetActive(false);
            return;
        }

        // Within max: draw extension R→Ext in blue
        Vector3 dirLR = (pR - pL).normalized;
        Vector3 pExt  = pR + dirLR * extraDistance;

        lrExt.enabled = true;
        lrExt.positionCount = 2;
        lrExt.SetPosition(0, pR);
        lrExt.SetPosition(1, pExt);
        SetLRColor(lrExt, extColor);

        if (extendedPointMarker)
        {
            extendedPointMarker.gameObject.SetActive(true);
            extendedPointMarker.position = pExt;
        }

        // Align the extension capsule to R→Ext
        float radius = Mathf.Max(lineWidth * 0.5f * colliderRadiusScale, 0.005f);
        AlignCapsule(extCapsule, pR, pExt, radius);
        extCapsule.enabled = true;
    }

    void HideAll()
    {
        lrMain.enabled = false;
        lrMain.positionCount = 0;

        lrExt.enabled = false;
        lrExt.positionCount = 0;

        extCapsule.enabled = false;
        if (extendedPointMarker) extendedPointMarker.gameObject.SetActive(false);
    }

    void SetLRColor(LineRenderer lr, Color c)
    {
        lr.startColor = c; lr.endColor = c;
        if (lr.material) lr.material.color = c;
    }

    void AlignCapsule(CapsuleCollider cap, Vector3 a, Vector3 b, float radius)
    {
        Vector3 mid = (a + b) * 0.5f;
        Vector3 dir = b - a;
        float len = dir.magnitude;
        if (len < 1e-5f) { cap.enabled = false; return; }
        dir /= len;

        var t = cap.transform;
        t.position = mid;
        t.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        cap.radius = radius;
        cap.height = Mathf.Max(2f * radius + len, 2f * radius + 1e-4f);
        cap.center = Vector3.zero;
    }

    // ---------- Trigger prints for extension ----------
    void OnTriggerEnter(Collider other)
    {
        if (!other) return;
        if (other.transform.IsChildOf(transform)) return; // ignore our own child (capsule)
        if (other.CompareTag("SceneObject"))
            Debug.Log($"[BlueExt] Enter SceneObject: {other.name} (t={Time.time:F2})");
    }
    void OnTriggerStay(Collider other)
    {
        if (!other) return;
        if (other.transform.IsChildOf(transform)) return;
        if (other.CompareTag("SceneObject"))
            Debug.Log($"[BlueExt] Stay  SceneObject: {other.name}");
    }
    void OnTriggerExit(Collider other)
    {
        if (!other) return;
        if (other.transform.IsChildOf(transform)) return;
        if (other.CompareTag("SceneObject"))
            Debug.Log($"[BlueExt] Exit  SceneObject: {other.name}");
    }
}
