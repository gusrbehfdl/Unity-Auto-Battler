
namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(IKAvatarPoser)), CanEditMultipleObjects]
    public class IKAvatarPoser_Inspector : Editor
    {
        private bool showReferences = false;
        private GUIStyle handleLabelStyle;
        private float scaleMod = 1.3f;

        public override void OnInspectorGUI()
        {
            IKAvatarPoser avatar = (IKAvatarPoser)target;
            if (avatar.animator == null)
            {
                EditorGUILayout.HelpBox(
                    "1. Press the 'Configure IK Avatar Poser' button.\n\n" +
                    "2. This will make the rigged character modal a child of a new game object," +
                    " create IK control points, and configure references.",
                    MessageType.Info);
                if (GUILayout.Button("Configure IK Avatar Poser"))
                {
                    avatar.ConfigureIKAvatarPoser();
                }
                return;
            }
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            float boolLabelWidth = 200;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Profile Settings", EditorStyles.centeredGreyMiniLabel);
            DrawSerializedPropertyField("profileData");
            if (avatar.profileData != null)
            {
                avatar.profileName = EditorGUILayout.TextField("Profile Name", avatar.profileName);
                bool profileNameExists = false;
                for (int i = 0; i < avatar.profileData.profiles.Count; i++)
                {
                    if (avatar.profileData.profiles[i].name == avatar.profileName)
                    {
                        profileNameExists = true;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (profileNameExists || string.IsNullOrEmpty(avatar.profileName))
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Add"))
                {
                    Undo.RecordObject(avatar.profileData, "Changed Profile Data");
                    avatar.SaveProfile(avatar.profileName);
                    EditorUtility.SetDirty(avatar.profileData);
                }
                GUI.enabled = profileNameExists ? true : false;
                if (GUILayout.Button("Update"))
                {
                    Undo.RecordObject(avatar.profileData, "Changed Profile Data");
                    avatar.SaveProfile(avatar.profileName);
                    EditorUtility.SetDirty(avatar.profileData);
                }
                if (GUILayout.Button("Load"))
                {
                    Undo.RecordObject(avatar, "Changed IK CP Transform");
                    Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                    avatar.LoadProfile(avatar.profileName);
                    avatar.animator.Update(Time.deltaTime);
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUI.indentLevel++;
            showReferences = EditorGUILayout.Foldout(showReferences, "References", true);
            if (showReferences)
            {
                DrawSerializedPropertyField("animator");
                DrawSerializedPropertyField("leftFootCP");
                DrawSerializedPropertyField("leftKneeCP");
                DrawSerializedPropertyField("rightFootCP");
                DrawSerializedPropertyField("rightKneeCP");
                DrawSerializedPropertyField("leftHandCP");
                DrawSerializedPropertyField("leftElbowCP");
                DrawSerializedPropertyField("rightHandCP");
                DrawSerializedPropertyField("rightElbowCP");
                DrawSerializedPropertyField("headCP");
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUIUtility.labelWidth = boolLabelWidth;
            DrawSerializedPropertyField("updateInEditMode");
            DrawSerializedPropertyField("ikActive");
            EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.LabelField("IK Weights", EditorStyles.centeredGreyMiniLabel);

            if (DrawSerializedToggleLeftProperty("leftLegControl", "Left Leg"))
            {
                DrawSerializedPropertyField("leftKneePositioWeight", "    Knee Position");
                DrawSerializedPropertyField("leftFootPositionWeight", "    Foot Position");
                DrawSerializedPropertyField("leftFootRotationWeight", "    Foot Rotation");
            }

            if (DrawSerializedToggleLeftProperty("rightLegControl", "Right Leg"))
            {
                DrawSerializedPropertyField("rightKneePositioWeight", "    Knee Position");
                DrawSerializedPropertyField("rightFootPositionWeight", "    Foot Position");
                DrawSerializedPropertyField("rightFootRotationWeight", "    Foot Rotation");
            }

            if (DrawSerializedToggleLeftProperty("leftArmControl", "Left Arm"))
            {
                DrawSerializedPropertyField("leftElbowPositioWeight", "    Elbow Position");
                DrawSerializedPropertyField("leftHandPositionWeight", "    Hand Position");
                DrawSerializedPropertyField("leftHandRotationWeight", "    Hand Rotation");
            }

            if (DrawSerializedToggleLeftProperty("rightArmControl", "Right Arm"))
            {
                DrawSerializedPropertyField("rightElbowPositioWeight", "    Elbow Position");
                DrawSerializedPropertyField("rightHandPositionWeight", "    Hand Position");
                DrawSerializedPropertyField("rightHandRotationWeight", "    Hand Rotation");
            }

            if (DrawSerializedToggleLeftProperty("headControl", "Head"))
            {
                DrawSerializedPropertyField("headPositionWeight", "    Position");
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void DrawSerializedPropertyField(string propertyName)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(serializedProperty);
        }

        void DrawSerializedPropertyField(string propertyName, string propertyFieldName)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(propertyFieldName));
        }

        bool DrawSerializedToggleLeftProperty(string propertyName, string toggleLeftname)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(propertyName);
            serializedProperty.boolValue = EditorGUILayout.ToggleLeft(toggleLeftname, serializedProperty.boolValue, EditorStyles.boldLabel);
            return serializedProperty.boolValue;
        }

        protected virtual void OnSceneGUI()
        {
            IKAvatarPoser avatar = (IKAvatarPoser)target;
            if (avatar.animator == null || avatar.ikActive == false)
            {
                return;
            }
            if (handleLabelStyle == null)
            {
                handleLabelStyle = new GUIStyle(EditorStyles.helpBox);
                handleLabelStyle.alignment = TextAnchor.MiddleCenter;
                handleLabelStyle.fontSize = 12;
                handleLabelStyle.fontStyle = FontStyle.Bold;
                handleLabelStyle.fixedHeight = 20;
                handleLabelStyle.normal.textColor = Color.white;
                handleLabelStyle.padding =  new RectOffset();
            }
            var startMatrix = Handles.matrix;

            switch (Tools.current)
            {
                case Tool.Rotate:
                    if (avatar.leftLegControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Quaternion leftFootRotation = Handles.RotationHandle(avatar.LeftFootRotation, avatar.LeftFootPosition * scaleMod);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.LeftFootRotation = leftFootRotation;
                        }
                        DrawHandleLabel(avatar.LeftFootPosition, " Left Foot");
                    }
                    if (avatar.rightLegControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Quaternion rightFootRotation = Handles.RotationHandle(avatar.RightFootRotation, avatar.RightFootPosition * scaleMod);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.RightFootRotation = rightFootRotation;
                        }
                        DrawHandleLabel(avatar.RightFootPosition, " Right Foot");
                    }
                    if (avatar.leftArmControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Quaternion leftHandRotation = Handles.RotationHandle(avatar.LeftHandRotation, avatar.LeftHandPosition * scaleMod);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.LeftHandRotation = leftHandRotation;
                        }
                        DrawHandleLabel(avatar.LeftHandPosition, " Left Hand");
                    }
                    if (avatar.rightArmControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Quaternion rightHandRotation = Handles.RotationHandle(avatar.RightHandRotation, avatar.RightHandPosition * scaleMod);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.RightHandRotation = rightHandRotation;
                        }
                        DrawHandleLabel(avatar.RightHandPosition, " Right Hand");
                    }
                    break;
                case Tool.Move:
                    if (avatar.leftLegControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Vector3 leftFootPosition = Handles.PositionHandle(avatar.LeftFootPosition * scaleMod, Quaternion.identity);
                        Vector3 leftKneePosition = Handles.PositionHandle(avatar.LeftKneePosition * scaleMod, Quaternion.identity);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.LeftFootPosition = leftFootPosition / scaleMod;
                            avatar.LeftKneePosition = leftKneePosition / scaleMod;
                        }
                        DrawHandleLabel(avatar.LeftFootPosition, "Left Foot");
                        DrawHandleLabel(avatar.LeftKneePosition, "Left Knee");
                    }
                    if (avatar.rightLegControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Vector3 rightFootPosition = Handles.PositionHandle(avatar.RightFootPosition * scaleMod, Quaternion.identity);
                        Vector3 rightKneePosition = Handles.PositionHandle(avatar.RightKneePosition * scaleMod, Quaternion.identity);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.RightFootPosition = rightFootPosition / scaleMod;
                            avatar.RightKneePosition = rightKneePosition / scaleMod;
                        }
                        DrawHandleLabel(avatar.RightFootPosition, "Right Foot");
                        DrawHandleLabel(avatar.RightKneePosition, "Right Knee");
                    }
                    if (avatar.leftArmControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Vector3 leftHandPosition = Handles.PositionHandle(avatar.LeftHandPosition * scaleMod, Quaternion.identity);
                        Vector3 leftElbowPosition = Handles.PositionHandle(avatar.LeftElbowPosition * scaleMod, Quaternion.identity);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.LeftHandPosition = leftHandPosition / scaleMod;
                            avatar.LeftElbowPosition = leftElbowPosition / scaleMod;
                        }
                        DrawHandleLabel(avatar.LeftHandPosition, "Left Hand");
                        DrawHandleLabel(avatar.LeftElbowPosition, "Left Elbow");
                    }
                    if (avatar.rightArmControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Vector3 rightHandPosition = Handles.PositionHandle(avatar.RightHandPosition * scaleMod, Quaternion.identity);
                        Vector3 rightElbowPosition = Handles.PositionHandle(avatar.RightElbowPosition * scaleMod, Quaternion.identity);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.RightHandPosition = rightHandPosition / scaleMod;
                            avatar.RightElbowPosition = rightElbowPosition / scaleMod;
                        }
                        DrawHandleLabel(avatar.RightHandPosition, "Right Hand");
                        DrawHandleLabel(avatar.RightElbowPosition, "Right Elbow");
                    }
                    if (avatar.headControl)
                    {
                        EditorGUI.BeginChangeCheck();
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / scaleMod) * startMatrix;
                        Vector3 headPosition = Handles.PositionHandle(avatar.HeadPosition * scaleMod, Quaternion.identity);
                        Handles.matrix = startMatrix;
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(avatar, "Changed IK CP Transform");
                            Undo.RegisterFullObjectHierarchyUndo(avatar.transform.parent.gameObject, "");
                            avatar.HeadPosition = headPosition / scaleMod;
                        }
                        DrawHandleLabel(avatar.HeadPosition, "Head");
                    }
                    break;
            }
        }

        void DrawHandleLabel(Vector3 position, string text)
        {
            Handles.Label(position, text, handleLabelStyle);
        }
    }
}
