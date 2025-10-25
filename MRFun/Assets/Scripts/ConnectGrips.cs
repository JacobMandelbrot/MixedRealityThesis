using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectGrips : MonoBehaviour
{
    [Header("Anchors (controller or hand)")]
    public Transform leftTransform;
    public Transform rightTransform;

  [Header("Grip gating")]
    public bool useAnalog = false;
    [Range(0f, 1f)] public float downThreshold = 0.75f;

    [Header("Extended point")]
    [Tooltip("Meters beyond the right anchor along the Left->Right direction")]
    public float extraDistance = 0.20f;
    public Transform extendedPointMarker; // optional gizmo

    [Header("Distance rule")]
    [Tooltip("If L↔R distance exceeds this, hide the extra point and turn the line red")]
    public float maxDistance = 0.80f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color overMaxColor = Color.red;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.positionCount = 3;              // default: we’ll switch to 2 when over max
        lr.useWorldSpace = true;
        lr.widthMultiplier = 0.01f;
        lr.numCapVertices = 8;
        lr.numCornerVertices = 4;

        if (lr.material == null)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            lr.material = mat;
        }
        SetLineColor(normalColor);
    }

    void Update()
    {
        if (!leftTransform || !rightTransform) { lr.enabled = false; return; }

        bool leftHeld, rightHeld;
        if (useAnalog)
        {
            float l = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
            float r = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
            leftHeld  = l >= downThreshold;
            rightHeld = r >= downThreshold;
        }
        else
        {
            leftHeld  = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            rightHeld = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        }

        if (!(leftHeld && rightHeld))
        {
            lr.enabled = false;
            if (extendedPointMarker) extendedPointMarker.gameObject.SetActive(false);
            return;
        }

        // Both held: update line
        Vector3 pL = leftTransform.position;
        Vector3 pR = rightTransform.position;
        float distLR = Vector3.Distance(pL, pR);

        if (distLR > maxDistance)
        {
            // Over the max: no extra point, red line
            lr.enabled = true;
            lr.positionCount = 2;
            lr.SetPosition(0, pL);
            lr.SetPosition(1, pR);
            SetLineColor(overMaxColor);

            if (extendedPointMarker) extendedPointMarker.gameObject.SetActive(false);
        }
        else
        {
            // Within max: show extended point, normal color
            Vector3 dirLR = (pR - pL).normalized;
            Vector3 pExt = pR + dirLR * extraDistance;

            lr.enabled = true;
            lr.positionCount = 3;
            lr.SetPosition(0, pL);
            lr.SetPosition(1, pR);
            lr.SetPosition(2, pExt);
            SetLineColor(normalColor);

            if (extendedPointMarker)
            {
                extendedPointMarker.gameObject.SetActive(true);
                extendedPointMarker.position = pExt;
                // optional: align with right controller
                // extendedPointMarker.rotation = rightTransform.rotation;
            }
        }
    }

    void SetLineColor(Color c)
    {
        lr.startColor = c;
        lr.endColor = c;
        if (lr.material) lr.material.color = c; // helps in some pipelines
    }
}
