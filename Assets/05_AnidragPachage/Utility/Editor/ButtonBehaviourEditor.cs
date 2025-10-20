#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ButtonBehaviour))]
public class ButtonBehaviourEditor : Editor
{
    SerializedProperty resolveTypeProp;
    SerializedProperty targetProp;
    SerializedProperty enableTargetProp;
    SerializedProperty audioClipProp;
    SerializedProperty audioOutputProp;
    SerializedProperty childIndexProp;
    SerializedProperty grandchildIndexProp;

    private void OnEnable()
    {
        resolveTypeProp = serializedObject.FindProperty("resolveType");
        targetProp = serializedObject.FindProperty("target");
        enableTargetProp = serializedObject.FindProperty("enableTarget");
        audioClipProp = serializedObject.FindProperty("audioClip");
        audioOutputProp = serializedObject.FindProperty("audioAutput");
        childIndexProp = serializedObject.FindProperty("childIndex");
        grandchildIndexProp = serializedObject.FindProperty("grandchildIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Show indexes based on enum
        var action = (ButtonBehaviour.ButtonActivationResult)resolveTypeProp.enumValueIndex;
        bool showTarget = action != ButtonBehaviour.ButtonActivationResult.OnlySound || action != ButtonBehaviour.ButtonActivationResult.DissableThisGameObject;
        bool showEnableTarget = action == ButtonBehaviour.ButtonActivationResult.CloseChildrenInTarget;
        bool showIndexes = action == ButtonBehaviour.ButtonActivationResult.DissableChildObject ||
                           action == ButtonBehaviour.ButtonActivationResult.DissableGrandChildObject;

        EditorGUILayout.PropertyField(resolveTypeProp);
        if(showTarget)
            EditorGUILayout.PropertyField(targetProp);
        if(showEnableTarget)
            EditorGUILayout.PropertyField(enableTargetProp);
        EditorGUILayout.PropertyField(audioClipProp);
        EditorGUILayout.PropertyField(audioOutputProp);


        if (showIndexes)
        {
            EditorGUILayout.LabelField("Child/Grandchild Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(childIndexProp);

            if (action == ButtonBehaviour.ButtonActivationResult.DissableGrandChildObject)
                EditorGUILayout.PropertyField(grandchildIndexProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
