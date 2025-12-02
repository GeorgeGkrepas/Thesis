using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR;

public class HandAnimator : MonoBehaviour
{
    private Animator animator;
    bool gripPressed = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(transform.name == "Right Hand Model")
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out gripPressed);
        else if (transform.name == "Left Hand Model")
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out gripPressed);

        if (gripPressed)
            animator.SetFloat("Trigger", 1);
        else if(!gripPressed)
            animator.SetFloat("Trigger", 0);
    }
}
