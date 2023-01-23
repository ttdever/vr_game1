
using UnityEngine;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class HeightController : MonoBehaviour
{
    public InputActionProperty aButtonReference;
    private float _triggerValue;
    [SerializeField] private XROrigin xr;

    void Update()
    {
        _triggerValue = aButtonReference.action.ReadValue<float>();
        xr.CameraYOffset = _triggerValue == 1 ? 1f : 2f;
        Debug.Log(_triggerValue);
    }
}
