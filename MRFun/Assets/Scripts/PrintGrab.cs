using UnityEngine;

public class PrintGrab : MonoBehaviour
{
    // simple edge-detection so it prints once per press
    bool leftHeld, rightHeld;

    void Update()
    {
        // "Button" style (fires when grip passes Meta’s internal threshold)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
            Debug.Log("[Grab] Left grip pressed");

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
            Debug.Log("[Grab] Right grip pressed");

        // Optional: analog check with your own threshold
        float l = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        float r = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        const float downThresh = 0.80f, upThresh = 0.20f;

        if (!leftHeld && l >= downThresh) { leftHeld = true; Debug.Log("[Grab] Left grip >= 0.8"); }
        else if (leftHeld && l <= upThresh) { leftHeld = false; Debug.Log("[Grab] Left grip released"); }

        if (!rightHeld && r >= downThresh) { rightHeld = true; Debug.Log("[Grab] Right grip >= 0.8"); }
        else if (rightHeld && r <= upThresh) { rightHeld = false; Debug.Log("[Grab] Right grip released"); }
    }
}
