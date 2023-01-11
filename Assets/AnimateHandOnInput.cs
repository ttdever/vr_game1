using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    private Animator _handAnimator;

    private float _triggerValue;
    private float _gripValue;

    private void Start()
    {
        _handAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        ReadInputs();
        AssignValuesToAnimator();
    }

    private void ReadInputs()
    {
        _triggerValue = pinchAnimationAction.action.ReadValue<float>();
        _gripValue = gripAnimationAction.action.ReadValue<float>();
    }

    private void AssignValuesToAnimator()
    {
        _handAnimator.SetFloat("Trigger", _triggerValue);
        _handAnimator.SetFloat("Grip", _gripValue);
    }
}