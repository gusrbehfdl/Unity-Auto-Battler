namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;

    [ExecuteInEditMode][System.Serializable]
    [RequireComponent(typeof(Animator))]
    public class IKAvatarPoser : MonoBehaviour
    {
        public RuntimeAnimatorController defaultAnuimatorController;
        public Animator animator;
        public ProfileData profileData;
        public bool updateInEditMode = true;
        public bool ikActive = true;
        public bool leftLegControl = true;
        public bool rightLegControl = true;
        public bool leftArmControl = true;
        public bool rightArmControl = true;
        public bool headControl = true;

        public Transform leftFootCP;
        public Vector3 LeftFootPosition { get { return leftFootCP.position; } set { leftFootCP.position = value; } }
        public Quaternion LeftFootRotation { get { return leftFootCP.rotation; } set { leftFootCP.rotation = value; } }

        public Transform rightFootCP;
        public Vector3 RightFootPosition { get { return rightFootCP.position; } set { rightFootCP.position = value; } }
        public Quaternion RightFootRotation { get { return rightFootCP.rotation; } set { rightFootCP.rotation = value; } }

        public Transform leftHandCP;
        public Vector3 LeftHandPosition { get { return leftHandCP.position; } set { leftHandCP.position = value; } }
        public Quaternion LeftHandRotation { get { return leftHandCP.rotation; } set { leftHandCP.rotation = value; } }

        public Transform rightHandCP;
        public Vector3 RightHandPosition { get { return rightHandCP.position; } set { rightHandCP.position = value; } }
        public Quaternion RightHandRotation { get { return rightHandCP.rotation; } set { rightHandCP.rotation = value; } }

        public Transform headCP;
        public Vector3 HeadPosition { get { return headCP.position; } set { headCP.position = value; } }
        public Quaternion HeadRotation { get { return headCP.rotation; } set { headCP.rotation = value; } }

        public Transform leftElbowCP;
        public Vector3 LeftElbowPosition { get { return leftElbowCP.position; } set { leftElbowCP.position = value; } }

        public Transform rightElbowCP;
        public Vector3 RightElbowPosition { get { return rightElbowCP.position; } set { rightElbowCP.position = value; } }

        public Transform leftKneeCP;
        public Vector3 LeftKneePosition { get { return leftKneeCP.position; } set { leftKneeCP.position = value; } }

        public Transform rightKneeCP;
        public Vector3 RightKneePosition { get { return rightKneeCP.position; } set { rightKneeCP.position = value; } }

        [Range(0, 1)] public float leftFootPositionWeight = 1;
        [Range(0, 1)] public float leftFootRotationWeight = 1;
        [Range(0, 1)] public float rightFootPositionWeight = 1;
        [Range(0, 1)] public float rightFootRotationWeight = 1;
        [Range(0, 1)] public float leftHandPositionWeight = 1;
        [Range(0, 1)] public float leftHandRotationWeight = 1;
        [Range(0, 1)] public float rightHandPositionWeight = 1;
        [Range(0, 1)] public float rightHandRotationWeight = 1;
        [Range(0, 1)] public float headPositionWeight = 1;
        [Range(0, 1)] public float leftKneePositioWeight = 1;
        [Range(0, 1)] public float rightKneePositioWeight = 1;
        [Range(0, 1)] public float leftElbowPositioWeight = 1;
        [Range(0, 1)] public float rightElbowPositioWeight = 1;

        [HideInInspector] public string profileName;

#if UNITY_EDITOR
        private void Update()
        {
            if (animator && updateInEditMode)
            {
                animator.Update(Time.deltaTime);
            }
        }
#endif

        public void OnAnimatorIK()
        {
            if (ikActive)
            {
                // set animator ik positions, rotations and weights based off script and control point values
                if (leftLegControl)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootRotation);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneePositioWeight);
                    animator.SetIKHintPosition(AvatarIKHint.LeftKnee, LeftKneePosition);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
                }
                if (rightLegControl)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPositionWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootRotationWeight);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, RightFootRotation);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneePositioWeight);
                    animator.SetIKHintPosition(AvatarIKHint.RightKnee, RightKneePosition);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);
                }
                if (leftArmControl)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandRotation);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowPositioWeight);
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowPosition);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
                }
                if (rightArmControl)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPositionWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandRotationWeight);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandRotation);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowPositioWeight);
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowPosition);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                }
                if (headControl)
                {
                    animator.SetLookAtWeight(headPositionWeight);
                    animator.SetLookAtPosition(HeadPosition);
                }
                else
                {
                    animator.SetLookAtWeight(0);
                }
            }
            else
            {
                // zero all weights
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                animator.SetLookAtWeight(0);
            }
        }

        public void ConfigureIKAvatarPoser()
        {
            // setup animator reference
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }
            
            // setup parent transform
            Transform parentTransform = new GameObject("IKAvatarPoser").transform;
            parentTransform.position = Vector3.zero;
            transform.parent = parentTransform;
            transform.position = Vector3.zero;
            // setup left foot cp
            leftFootCP = new GameObject("Left Foot CP").transform;
            leftFootCP.parent = transform.parent;
            leftFootCP.transform.SetPositionAndRotation(
                animator.GetBoneTransform(HumanBodyBones.LeftFoot).position,
                Quaternion.Euler(0, 0, 0));
            //leftFootCP.transform.SetPositionAndRotation(new Vector3(-0.18f, 0f, 0.3f), Quaternion.Euler(0, 0, 0));
            // setup left knee cp
            leftKneeCP = new GameObject("Left Knee CP").transform;
            leftKneeCP.parent = transform.parent;
            Vector3 leftKneePosition = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position + (Vector3.forward / 5);
            leftKneeCP.transform.SetPositionAndRotation(
                leftKneePosition,
                Quaternion.Euler(0, 0, 0));
            //leftKneeCP.transform.SetPositionAndRotation(new Vector3(-0.1f, 0.5f, 0.23f), Quaternion.Euler(0, 0, 0));
            // setup right foot cp
            rightFootCP = new GameObject("Right Foot CP").transform;
            rightFootCP.parent = transform.parent;
            rightFootCP.transform.SetPositionAndRotation(
                animator.GetBoneTransform(HumanBodyBones.RightFoot).position,
                Quaternion.Euler(0, 0, 0));
            //rightFootCP.transform.SetPositionAndRotation(new Vector3(0.26f, 0f, -0.05f), Quaternion.Euler(0, 40, 0));
            // setup right knee cp
            rightKneeCP = new GameObject("Right Knee CP").transform;
            rightKneeCP.parent = transform.parent;
            Vector3 rightKneePosition = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position + (Vector3.forward / 5);
            rightKneeCP.transform.SetPositionAndRotation(
                rightKneePosition,
                Quaternion.Euler(0, 0, 0));
            //rightKneeCP.transform.SetPositionAndRotation(new Vector3(0.2f, 0.5f, 0.03f), Quaternion.Euler(0, 0, 0));
            // setup left hand cp
            leftHandCP = new GameObject("Left Hand CP").transform;
            leftHandCP.parent = transform.parent;
            leftHandCP.transform.SetPositionAndRotation(
                animator.GetBoneTransform(HumanBodyBones.LeftHand).position,
                Quaternion.Euler(0, 0, 0));
            //leftHandCP.transform.SetPositionAndRotation(new Vector3(-0.22f, 1.084f, 0.03f), Quaternion.Euler(0, 0, 130));
            // setup left elbow cp
            leftElbowCP = new GameObject("Left Elbow CP").transform;
            leftElbowCP.parent = transform.parent;
            leftElbowCP.transform.SetPositionAndRotation(
                animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position,
                Quaternion.Euler(0, 0, 0));
            //leftElbowCP.transform.SetPositionAndRotation(new Vector3(-0.23f, 1.16f, -0.1f), Quaternion.Euler(0, 0, 0));
            // setup right hand cp
            rightHandCP = new GameObject("Right Hand CP").transform;
            rightHandCP.parent = transform.parent;
            rightHandCP.transform.SetPositionAndRotation(
                animator.GetBoneTransform(HumanBodyBones.RightHand).position,
                Quaternion.Euler(0, 0, 0));
            //rightHandCP.transform.SetPositionAndRotation(new Vector3(0.43f, 1.3f, 0.1f), Quaternion.Euler(0, 45, -200));
            // setup right elbow cp
            rightElbowCP = new GameObject("Right Elbow CP").transform;
            rightElbowCP.parent = transform.parent;
            rightElbowCP.transform.SetPositionAndRotation(
                animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position,
                Quaternion.Euler(0, 0, 0));
            //rightElbowCP.transform.SetPositionAndRotation(new Vector3(0.27f, 1.13f, -0.109f), Quaternion.Euler(0, 0, 0));
            // setup heap cp
            headCP = new GameObject("Head CP").transform;
            headCP.parent = transform.parent;
            Vector3 heatPosition = animator.GetBoneTransform(HumanBodyBones.Head).position + (Vector3.forward/5);
            headCP.transform.SetPositionAndRotation(heatPosition, Quaternion.Euler(0, 0, 0));
            //headCP.transform.SetPositionAndRotation(new Vector3(0.36f, 1.45f, 0.35f), Quaternion.Euler(0, 0, 0));
            if (animator.runtimeAnimatorController == null)
            {
                animator.runtimeAnimatorController = defaultAnuimatorController;
            }
        }

        public void SaveProfile(string profileName)
        {
            if (profileData == null)
                return;
            bool saveComplete = false;
            Profile currentProfile = new Profile();
            currentProfile.name = profileName;
            currentProfile.leftLegControl = leftLegControl;
            currentProfile.rightLegControl = rightLegControl;
            currentProfile.leftArmControl = leftArmControl;
            currentProfile.rightArmControl = rightArmControl;
            currentProfile.headControl = headControl;
            currentProfile.leftFootPosition = LeftFootPosition;
            currentProfile.leftFootRotation = LeftFootRotation;
            currentProfile.rightFootPosition = RightFootPosition;
            currentProfile.rightFootRotation = RightFootRotation;
            currentProfile.leftHandPosition = LeftHandPosition;
            currentProfile.leftHandRotation = LeftHandRotation;
            currentProfile.rightHandPosition = RightHandPosition;
            currentProfile.rightHandRotation = RightHandRotation;
            currentProfile.headPosition = HeadPosition;
            currentProfile.headRotation = HeadRotation;
            currentProfile.leftElbowPosition = LeftElbowPosition;
            currentProfile.rightElbowPosition = RightElbowPosition;
            currentProfile.leftKneePosition = LeftKneePosition;
            currentProfile.rightKneePosition = RightKneePosition;
            currentProfile.leftFootPositionWeight = leftFootPositionWeight;
            currentProfile.leftFootRotationWeight = leftFootRotationWeight;
            currentProfile.rightFootPositionWeight = rightFootPositionWeight;
            currentProfile.rightFootRotationWeight = rightFootRotationWeight;
            currentProfile.leftHandPositionWeight = leftHandPositionWeight;
            currentProfile.leftHandRotationWeight = leftHandRotationWeight;
            currentProfile.rightHandPositionWeight = rightHandPositionWeight;
            currentProfile.rightHandRotationWeight = rightHandRotationWeight;
            currentProfile.headPositionWeight = headPositionWeight;
            currentProfile.leftKneePositioWeight = leftKneePositioWeight;
            currentProfile.rightKneePositioWeight = rightKneePositioWeight;
            currentProfile.leftElbowPositioWeight = leftElbowPositioWeight;
            currentProfile.rightElbowPositioWeight = rightElbowPositioWeight;

            for (int i = 0; i < profileData.profiles.Count; i++)
            {
                if (profileData.profiles[i].name == currentProfile.name)
                {
                    profileData.profiles[i] = currentProfile;
                    saveComplete = true;
                    Debug.Log("IKAvatarPoser updated profile: " + profileName);
                    break;
                }
            }
            if (saveComplete == false)
            {
                profileData.profiles.Add(currentProfile);
                Debug.Log("IKAvatarPoser added profile: " + profileName);
            }
        }

        public void LoadProfile(string profileName)
        {
            if (profileData == null)
                return;

            for (int i = 0; i < profileData.profiles.Count; i++)
            {
                if (profileData.profiles[i].name == profileName)
                {
                    leftLegControl = profileData.profiles[i].leftLegControl;
                    rightLegControl = profileData.profiles[i].rightLegControl;
                    leftArmControl = profileData.profiles[i].leftArmControl;
                    rightArmControl = profileData.profiles[i].rightArmControl;
                    headControl = profileData.profiles[i].headControl;
                    LeftFootPosition = profileData.profiles[i].leftFootPosition;
                    LeftFootRotation = profileData.profiles[i].leftFootRotation;
                    RightFootPosition = profileData.profiles[i].rightFootPosition;
                    RightFootRotation = profileData.profiles[i].rightFootRotation;
                    LeftHandPosition = profileData.profiles[i].leftHandPosition;
                    LeftHandRotation = profileData.profiles[i].leftHandRotation;
                    RightHandPosition = profileData.profiles[i].rightHandPosition;
                    RightHandRotation = profileData.profiles[i].rightHandRotation;
                    HeadPosition = profileData.profiles[i].headPosition;
                    HeadRotation = profileData.profiles[i].headRotation;
                    LeftElbowPosition = profileData.profiles[i].leftElbowPosition;
                    RightElbowPosition = profileData.profiles[i].rightElbowPosition;
                    LeftKneePosition = profileData.profiles[i].leftKneePosition;
                    RightKneePosition = profileData.profiles[i].rightKneePosition;
                    leftFootPositionWeight = profileData.profiles[i].leftFootPositionWeight;
                    leftFootRotationWeight = profileData.profiles[i].leftFootRotationWeight;
                    rightFootPositionWeight = profileData.profiles[i].rightFootPositionWeight;
                    rightFootRotationWeight = profileData.profiles[i].rightFootRotationWeight;
                    leftHandPositionWeight = profileData.profiles[i].leftHandPositionWeight;
                    leftHandRotationWeight = profileData.profiles[i].leftHandRotationWeight;
                    rightHandPositionWeight = profileData.profiles[i].rightHandPositionWeight;
                    rightHandRotationWeight = profileData.profiles[i].rightHandRotationWeight;
                    headPositionWeight = profileData.profiles[i].headPositionWeight;
                    leftKneePositioWeight = profileData.profiles[i].leftKneePositioWeight;
                    rightKneePositioWeight = profileData.profiles[i].rightKneePositioWeight;
                    leftElbowPositioWeight = profileData.profiles[i].leftElbowPositioWeight;
                    rightElbowPositioWeight = profileData.profiles[i].rightElbowPositioWeight;
                    Debug.Log("IKAvatarPoser loaded profile: " + profileName);
                    break;
                }
            }
        }
    }
}
