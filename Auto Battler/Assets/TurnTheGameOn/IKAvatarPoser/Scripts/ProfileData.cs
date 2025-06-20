namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "ProfileData", menuName = "IK Avatar Poser/ProfileData")]
    public class ProfileData : ScriptableObject
    {
        public List<Profile> profiles = new List<Profile>();
    }

    [System.Serializable]
    public class Profile
    {
        public string name;
        public bool leftLegControl;
        public bool rightLegControl;
        public bool leftArmControl;
        public bool rightArmControl;
        public bool headControl;
        public Vector3 leftFootPosition;
        public Quaternion leftFootRotation;
        public Vector3 rightFootPosition;
        public Quaternion rightFootRotation;
        public Vector3 leftHandPosition;
        public Quaternion leftHandRotation;
        public Vector3 rightHandPosition;
        public Quaternion rightHandRotation;
        public Vector3 headPosition;
        public Quaternion headRotation;
        public Vector3 leftElbowPosition;
        public Vector3 rightElbowPosition;
        public Vector3 leftKneePosition;
        public Vector3 rightKneePosition;
        public float leftFootPositionWeight;
        public float leftFootRotationWeight;
        public float rightFootPositionWeight;
        public float rightFootRotationWeight;
        public float leftHandPositionWeight;
        public float leftHandRotationWeight;
        public float rightHandPositionWeight;
        public float rightHandRotationWeight;
        public float headPositionWeight;
        public float leftKneePositioWeight;
        public float rightKneePositioWeight;
        public float leftElbowPositioWeight;
        public float rightElbowPositioWeight;
    }
}
