// Copyright (C) 2014-2022 Gleechi AB. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using VirtualGrasp;

public class ChainAssembleVGArticulation : MonoBehaviour
{
    [Tooltip("Threshold to assemble when object articulation anchor to anchor target distance is smaller than this value.")]
    public float m_assembleDistance = 0.02f;
    [Tooltip("Threshold to disassemble when sensor hand position to grasped hand position is bigger than this value.")]
    public float m_disassembleDistance = 0.2f;
    [Tooltip("The VG Articulation of constrained (non-FLOATING) joint type to switch to when assemble an object.")]
    public VG_Articulation m_assembleArticulation = null;
    [Tooltip("The target anchor position the anchor in Assemble Articulation of another object should be matched to.")]
    public Transform m_anchorTarget = null;

    [Tooltip("If provided give disassemble sound effect.")]
    public AudioSource m_disassembleSoundEffect;
    [Tooltip("If provided give assemble sound effect.")]
    public AudioSource m_assembleSoundEffect;

    private float m_timeAtDisassemble = 0.0F;
    private float m_assembleDelay = 1.0F;

    private List<Transform> m_chainParts = new List<Transform>();

    private Transform m_oldChainPartsParent = null;

    // Start is called before the first frame update
    void Start()
    {
        if(m_assembleArticulation == null)
        {
            VG_Debug.LogError("Has to assign an Assemble Articulation on " + this.transform.name);
            return;
        }
        if(m_assembleArticulation.m_type == VG_JointType.FLOATING)
        {
            VG_Debug.LogError("Assemble Articulation should be of a constrained joint type, can not be FLOATING on " + this.transform.name);
            return;
        }

        foreach (ChainAssembleVGArticulation assemble in  new List<ChainAssembleVGArticulation>(FindObjectsOfType<ChainAssembleVGArticulation>()))
        {
            if (m_oldChainPartsParent == null)
                m_oldChainPartsParent = assemble.transform.parent;
            m_chainParts.Add(assemble.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        assembleByJointChange();
        dessembleByJointChange();
    }

    void assembleByJointChange()
    {
        // If this object is already assembled, so joint type is not floating anymore then skip
        VG_JointType jointType;
        if (VG_Controller.GetObjectJointType(this.transform, false, out jointType) == VG_ReturnCode.SUCCESS
            && jointType != VG_JointType.FLOATING)
            return;
        // Try to connect this object to others that as part of the chain
        foreach (Transform part in m_chainParts)
        {
            if (part == transform)
                continue;
            
            if ((Time.realtimeSinceStartup - m_timeAtDisassemble) <= m_assembleDelay)
                continue;

            // Anchor position on this transform
            Vector3 anchorPos = m_assembleArticulation && m_assembleArticulation.m_anchor ?
                m_assembleArticulation.m_anchor.position : transform.position;

            // Target part's link position
            part.gameObject.TryGetComponent<ChainAssembleVGArticulation>(out ChainAssembleVGArticulation part_assemble);
            Vector3 anchorTargetPos = part_assemble.m_anchorTarget.position;
            Vector3 offset = anchorTargetPos - anchorPos;
            if (offset.magnitude >= m_assembleDistance)
                continue;

            this.transform.SetPositionAndRotation(this.transform.position + offset, this.transform.rotation);
            this.transform.SetParent(part);
            VG_ReturnCode ret = VG_Controller.ChangeObjectJoint(transform, m_assembleArticulation);
            if (ret != VG_ReturnCode.SUCCESS)
            {
                VG_Debug.LogError("Failed to ChangeObjectJoint() on " + transform.name + " with return code " + ret);
                continue;
            }
            m_assembleSoundEffect?.Play();
        }
    }

    void dessembleByJointChange()
    {
        foreach (VG_HandStatus hand in VG_Controller.GetHands())
        {
            VG_JointType jointType;
            if (hand.m_selectedObject == transform && hand.IsHolding()
                && VG_Controller.GetObjectJointType(transform, false, out jointType) == VG_ReturnCode.SUCCESS
                && jointType != VG_JointType.FLOATING)
            {
                VG_Controller.GetSensorPose(hand.m_avatarID, hand.m_side, out Vector3 sensor_pos, out Quaternion sensor_rot);
                float jointState = 0.0f;
                if (jointType == VG_JointType.REVOLUTE)
                    VG_Controller.GetObjectJointState(transform, out jointState);
                if (jointState == 0.0f && (sensor_pos - hand.m_hand.position).magnitude > m_disassembleDistance)
                {
                    transform.SetParent(m_oldChainPartsParent);
                    VG_ReturnCode ret = VG_Controller.RecoverObjectJoint(transform);
                    if (ret != VG_ReturnCode.SUCCESS)
                        VG_Debug.LogError("Failed to RecoverObjectJoint() on " + transform.name + " with return code " + ret);

                    m_timeAtDisassemble = Time.realtimeSinceStartup;
                    m_disassembleSoundEffect?.Play();
                }
            }
        }
    }
}
