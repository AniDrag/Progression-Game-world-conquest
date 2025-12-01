#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonBehaviour))]
public class ButtonBehaviourEditor : Editor
{
    SerializedProperty doEnable;
    SerializedProperty actionProp;
    SerializedProperty useAudioProp;

    // Sound
    SerializedProperty audioClipProp;
    SerializedProperty audioOutputProp;
    SerializedProperty pitchProp;
    SerializedProperty volumeProp;
    SerializedProperty randomPitchProp;

    // Target / children
    SerializedProperty targetProp;
    SerializedProperty childIndexProp;
    SerializedProperty grandchildIndexProp;

    // Events
    SerializedProperty onClickProp;

    private void OnEnable()
    {
        doEnable = serializedObject.FindProperty("enableDisable");
        actionProp = serializedObject.FindProperty("action");
        useAudioProp = serializedObject.FindProperty("useAudio");

        audioClipProp = serializedObject.FindProperty("audioClip");
        audioOutputProp = serializedObject.FindProperty("audioOutput");
        pitchProp = serializedObject.FindProperty("pitch");
        volumeProp = serializedObject.FindProperty("volume");
        randomPitchProp = serializedObject.FindProperty("randomPitch");

        targetProp = serializedObject.FindProperty("target");
        childIndexProp = serializedObject.FindProperty("childIndex");
        grandchildIndexProp = serializedObject.FindProperty("grandchildIndex");

        onClickProp = serializedObject.FindProperty("onClick");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ----------------------------
        // BUTTON SETTINGS
        // ----------------------------
        DrawHeader("Button Settings");
        EditorGUILayout.PropertyField(doEnable);
        EditorGUILayout.PropertyField(actionProp);
        EditorGUILayout.PropertyField(useAudioProp, new GUIContent("Use Audio"));

        var action = (ButtonBehaviour.ButtonAction)actionProp.enumValueIndex;

        EditorGUILayout.Space(6);

        // ----------------------------
        // SOUND SETTINGS (conditional)
        // ----------------------------
        
        if (useAudioProp.boolValue)
        {
            DrawHeader("Sound Settings");
            EditorGUILayout.PropertyField(audioClipProp);
            EditorGUILayout.PropertyField(audioOutputProp);

            EditorGUILayout.Slider(volumeProp, 0f, 1f, new GUIContent("Volume"));
            EditorGUILayout.Slider(pitchProp, 0.1f, 3f, new GUIContent("Pitch"));
            EditorGUILayout.PropertyField(randomPitchProp);
        }

        EditorGUILayout.Space(6);

        // ----------------------------
        // TARGET / CHILDREN SETTINGS
        // ----------------------------
        if (action == ButtonBehaviour.ButtonAction.ToggleTarget ||
            action == ButtonBehaviour.ButtonAction.ToggleChild ||
            action == ButtonBehaviour.ButtonAction.ToggleGrandChild)
        {
            DrawHeader("Target Details");

            switch (action)
            {
                case ButtonBehaviour.ButtonAction.ToggleTarget:
                    EditorGUILayout.PropertyField(targetProp, new GUIContent("Target"));
                    break;

                case ButtonBehaviour.ButtonAction.ToggleChild:
                    EditorGUILayout.PropertyField(childIndexProp, new GUIContent("Child Index"));
                    break;

                case ButtonBehaviour.ButtonAction.ToggleGrandChild:
                    EditorGUILayout.PropertyField(childIndexProp, new GUIContent("Child Index"));
                    EditorGUILayout.PropertyField(grandchildIndexProp, new GUIContent("Grandchild Index"));
                    break;
            }
        }

        EditorGUILayout.Space(6);

        // ----------------------------
        // EVENTS
        // ----------------------------
        DrawHeader("Events");
        EditorGUILayout.PropertyField(onClickProp);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(string label)
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("----------------------------------", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("----------------------------------", EditorStyles.boldLabel);
    }
}
#endif
