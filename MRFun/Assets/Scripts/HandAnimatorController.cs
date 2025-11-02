using UnityEngine;
using Oculus.Interaction.Input;  // Correct namespace for OVRInput

[RequireComponent(typeof(Animator))]
public class HandAnimatorController : MonoBehaviour
{
    [SerializeField] private OVRInput.Controller controllerType = OVRInput.Controller.RTouch;
    private Animator anim;
    
    public float triggerValue;
    public float gripValue;

    void Awake() => anim = GetComponent<Animator>();

    void Update()
    {
        triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controllerType);
        gripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controllerType);

        anim.SetFloat("Trigger", triggerValue);
        anim.SetFloat("Grip", gripValue);
    }
}