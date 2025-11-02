// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;
// using UnityEngine.XR.Interaction.Toolkit.Interactors;
//
// public class LeftHandOnlyGrab : MonoBehaviour
// {
//     private XRGrabInteractable grabInteractable;
//     
//     void Awake()
//     {
//         grabInteractable = GetComponent<XRGrabInteractable>();
//     }
//     
//     public bool CanSelect(IXRSelectInteractor interactor)
//     {
//         // Check if interactor is left hand
//         return interactor.transform.name.ToLower().Contains("left");
//     }
// }