using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandPose : MonoBehaviour
{
    [SerializeField] private GameObject handGhost;
    private HandData _handPos;

    private Vector3 _startHandPos;
    private Vector3 _finalHandPos;
    private Quaternion _startHandRot;
    private Quaternion _finalHandRot;
    private Quaternion[] _startBonesRot;
    private Quaternion[] _finalBonesRot;

    private void Awake()
    {
        var grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(ApplyHandPose);
        grabInteractable.selectExited.AddListener(UnsetPos);
        _handPos = handGhost.GetComponent<HandData>();
        _handPos.SetHandRoot(handGhost.transform);
        _handPos.gameObject.SetActive(false);
    }

    private void UnsetPos(BaseInteractionEventArgs args)
    {
        var handData = args.interactorObject.transform.GetComponentInChildren<HandData>();
        handData.SetAnimatorEnabledState(false);
        
        SetHandData(handData, _startHandPos, _startHandRot, _startBonesRot);
    }

    private void ApplyHandPose(BaseInteractionEventArgs args)
    {
        if (args.interactableObject is XRGrabInteractable)
        {
            var handData = args.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.SetAnimatorEnabledState(false);

            SetHandDataValues(handData, _handPos);
            SetHandData(handData, _finalHandPos, _finalHandRot, _finalBonesRot);
        }
    }

    public void SetHandDataValues(HandData startHandState, HandData finalHandState)
    {
        var handStruct1 = startHandState.GetHandPositionAndRotation();
        var handStruct2 = finalHandState.GetHandPositionAndRotation();

        handStruct1.position = applyScaleToPosition(handStruct1.position, startHandState.GetHandScale());
        handStruct2.position = applyScaleToPosition(handStruct2.position, finalHandState.GetHandScale());

        _startHandPos = handStruct1.position;
        _finalHandPos = handStruct2.position;

        _startHandRot = handStruct1.rotation;
        _finalHandRot = handStruct2.rotation;

        _startBonesRot = new Quaternion[handStruct1.bonesRotation.Length];
        _finalBonesRot = new Quaternion[handStruct2.bonesRotation.Length];

        for (var bonesIterator = 0; bonesIterator < handStruct1.bonesRotation.Length; bonesIterator++)
        {
            _startBonesRot[bonesIterator] = handStruct1.bonesRotation[bonesIterator];
            _finalBonesRot[bonesIterator] = handStruct2.bonesRotation[bonesIterator];
        }
    }

    private void SetHandData(HandData handData, Vector3 newPos, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        handData.SetHandPositionAndRotation(new HandStruct(newPos, newRotation, newBonesRotation));
    }

    private Vector3 applyScaleToPosition(Vector3 position, Vector3 scale)
    {
        return new Vector3(position.x / scale.x, position.y / scale.y, position.z / scale.z);
    }
}