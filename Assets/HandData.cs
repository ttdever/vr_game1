using UnityEngine;

public class HandData : MonoBehaviour
{
    public enum HandModelType
    {
        Left = 0,
        Right = 1
    }

    [SerializeField] private HandModelType handType;
    [SerializeField] private Transform[] fingerBones;
    private Animator _handAnimator;
    private Transform _handRoot;

    private void Awake()
    {
        _handRoot = gameObject.transform;
        _handAnimator = gameObject.GetComponent<Animator>();
    }

    public void SetAnimatorEnabledState(bool state)
    {
        _handAnimator.enabled = state;
    }
    
    public HandStruct GetHandPositionAndRotation()
    {
        Quaternion[] fingerBonesRotation = new Quaternion[fingerBones.Length];
        for (var i = 0; i < fingerBonesRotation.Length; i++)
        {
            fingerBonesRotation[i] = fingerBones[i].localRotation;
        }
        return new HandStruct(_handRoot.localPosition, _handRoot.localRotation, fingerBonesRotation);
    }
    
    public void SetHandPositionAndRotation(HandStruct handStruct)
    {
        _handRoot.localPosition = handStruct.position;
        _handRoot.localRotation = handStruct.rotation;

        for (var i = 0; i < handStruct.bonesRotation.Length; i++)
        {
            fingerBones[i].localRotation = handStruct.bonesRotation[i];
        }
    }

    public void SetHandRoot(Transform newHandRoot)
    {
        _handRoot = newHandRoot;
    }

    public Vector3 GetHandScale()
    {
        return transform.localScale;
    }

    public HandModelType GetHandType()
    {
        return handType;
    }
}

public struct HandStruct
{
    public HandStruct(Vector3 position, Quaternion rotation, Quaternion[] bonesRotation)
    {
        this.position = position;
        this.rotation = rotation;
        this.bonesRotation = bonesRotation;
    }

    public Vector3 position;
    public Quaternion rotation;
    public Quaternion[] bonesRotation;
}