using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
[RequireComponent(typeof(HandPhysics))]
[RequireComponent(typeof(SteamVR_Behaviour_Pose))]
public class HandUnscaledFollower : MonoBehaviour
{
    private Hand hand;
    private HandPhysics handPhysics;
    private HandCollider handCollider;

    const int wristBone = SteamVR_Skeleton_JointIndexes.wrist;
    const int rootBone  = SteamVR_Skeleton_JointIndexes.root;

    void Awake()
    {
        hand        = GetComponent<Hand>();
        handPhysics = GetComponent<HandPhysics>();
    }

    void Start()
    {
        handCollider = handPhysics != null ? handPhysics.handCollider : null;
    }

    void LateUpdate()
    {
        if (hand == null || hand.skeleton == null || handCollider == null)
            return;
        if (Mathf.Approximately(Time.timeScale, 1f))
            return;
        Matrix4x4 wristToRoot = Matrix4x4.TRS(
            ProcessPos(wristBone, hand.skeleton.GetBone(wristBone).localPosition),
            ProcessRot(wristBone, hand.skeleton.GetBone(wristBone).localRotation),
            Vector3.one
        ).inverse;

        Matrix4x4 rootToArmature = Matrix4x4.TRS(
            ProcessPos(rootBone, hand.skeleton.GetBone(rootBone).localPosition),
            ProcessRot(rootBone, hand.skeleton.GetBone(rootBone).localRotation),
            Vector3.one
        ).inverse;

        Matrix4x4 wristToArmature = (wristToRoot * rootToArmature).inverse;

        Vector3 targetPosition = transform.TransformPoint(wristToArmature.MultiplyPoint3x4(Vector3.zero));
        Quaternion targetRotation = transform.rotation * ExtractRotation(wristToArmature);

        Vector3 offset = hand.skeleton.GetBonePosition(SteamVR_Skeleton_JointIndexes.middleProximal)
                       - hand.skeleton.GetBonePosition(SteamVR_Skeleton_JointIndexes.root);
        handCollider.SetCenterPoint(hand.skeleton.transform.position + offset);

        UpdateFingertipsLocal(handCollider, hand);

        handCollider.TeleportTo(targetPosition, targetRotation);
    }

    Vector3 ProcessPos(int boneIndex, Vector3 pos)
    {
        if (hand.skeleton.mirroring != SteamVR_Behaviour_Skeleton.MirrorType.None)
            return SteamVR_Behaviour_Skeleton.MirrorPosition(boneIndex, pos);
        return pos;
    }

    Quaternion ProcessRot(int boneIndex, Quaternion rot)
    {
        if (hand.skeleton.mirroring != SteamVR_Behaviour_Skeleton.MirrorType.None)
            return SteamVR_Behaviour_Skeleton.MirrorRotation(boneIndex, rot);
        return rot;
    }
    static Quaternion ExtractRotation(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    static void UpdateFingertipsLocal(HandCollider hc, Hand h)
    {
        Transform wristTf = h.skeleton.GetBone(SteamVR_Skeleton_JointIndexes.wrist);
        if (wristTf == null || hc.fingerColliders == null) return;

        for (int finger = 0; finger < 5; finger++)
        {
            int tip = SteamVR_Skeleton_JointIndexes.GetBoneForFingerTip(finger);
            int bone = tip;
            var collArr = hc.fingerColliders[finger];
            if (collArr == null) continue;

            for (int i = 0; i < collArr.Length; i++)
            {
                bone = tip - 1 - i; // distal abwärts
                if (collArr[i] != null)
                {
                    var boneTf = h.skeleton.GetBone(bone);
                    if (boneTf != null)
                        collArr[i].localPosition = wristTf.InverseTransformPoint(boneTf.position);
                }
            }
        }
    }
}
