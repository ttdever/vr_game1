// Copyright (C) 2014-2022 Gleechi AB. All rights reserved.

using UnityEngine;
using VirtualGrasp;

/** 
 * AssembleVGArticulation shows as a tutorial on how to use the VG_Controller.ChangeObjectJoint function for
 * assemble and dissemble non-physical objects (objects without rigid body or articulation body).
 */
[LIBVIRTUALGRASP_UNITY_SCRIPT]
[HelpURL("https://docs.virtualgrasp.com/unity_vgonboarding_task5." + VG_Version.__VG_VERSION__ + ".html")]
public class AssembleVGArticulation : MonoBehaviour
{
    [Tooltip("When assemble, the new parent to assign to this object.")]
    public Transform m_newParent = null;
    [Tooltip("The target pose of the assembled object.")]
    public Transform m_desiredPose = null;
    [Tooltip("Threshold to assemble when object to Desired position is smaller than this value.")]
    public float m_assembleDistance = 0.05f;

    [Tooltip("Threshold to assemble when object to Desired angle is smaller than this value.")]
    public float m_assembleAngle = 45;

    [Tooltip("Threshold to disassemble when sensor hand position to grasped hand position is bigger than this value.")]
    public float m_disassembleDistance = 0.5f;
    [Tooltip("The VG Articulation of constrained (non-FLOATING) joint type to switch to when assemble an object.")]
    public VG_Articulation m_assembleArticulation = null;

    [Tooltip("If provided give disassemble sound effect.")]
    public AudioSource m_disassembleSoundEffect;
    [Tooltip("If provided give assemble sound effect.")]
    public AudioSource m_assembleSoundEffect;

    private float m_timeAtDisassemble = 0.0F;
    private float m_assembleDelay = 1.0F;

    void Start()
    {
        if (m_assembleArticulation == null)
            VG_Debug.LogError("Has to assign an Assemble Articulation on " + this.transform.name);
        if (m_assembleArticulation.m_type == VG_JointType.FLOATING)
            VG_Debug.LogError("Assemble Articulation should be of a constrained joint type, can not be FLOATING on " + this.transform.name);
    }

    void LateUpdate()
    {
        assembleByJointChange();
        dessembleByJointChange();
    }

    void assembleByJointChange()
    {
        VG_JointType jointType;
        Quaternion.FromToRotation(m_desiredPose.up, this.transform.up).ToAngleAxis(out float angle, out _);

        if ((Time.realtimeSinceStartup - m_timeAtDisassemble) > m_assembleDelay
           && (m_desiredPose.position - this.transform.position).magnitude < m_assembleDistance
           && (angle < m_assembleAngle)
           && VG_Controller.GetObjectJointType(this.transform, false, out jointType) == VG_ReturnCode.SUCCESS &&
           jointType == VG_JointType.FLOATING)
        {
            m_desiredPose.gameObject.SetActive(false);

            // Project object rotation axis to align to desired rotation axis.
            Quaternion q_raw = Quaternion.LookRotation(m_desiredPose.up, transform.forward);
            Quaternion q_nat = q_raw * Quaternion.Euler(0, 180, 0) * Quaternion.Euler(-90, 0, 0);
            this.transform.SetPositionAndRotation(m_desiredPose.position, q_nat);

            if (m_newParent != null)
                this.transform.SetParent(m_newParent);

            VG_ReturnCode ret = VG_Controller.ChangeObjectJoint(transform, m_assembleArticulation);
            if(ret != VG_ReturnCode.SUCCESS)
                VG_Debug.LogError("Failed to ChangeObjectJoint() on " + transform.name + " with return code " + ret);

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
                    m_desiredPose.gameObject.SetActive(true);
                    transform.SetParent(m_newParent.parent);
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
