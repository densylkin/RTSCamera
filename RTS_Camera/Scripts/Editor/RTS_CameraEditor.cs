using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace RTS_Cam
{
    [CustomEditor(typeof(RTS_Camera))]
    public class RTS_CameraEditor : Editor
    {
        private RTS_Camera camera { get { return target as RTS_Camera; } }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(camera, "RTS_Camera");
            GUILayout.BeginVertical(GUI.skin.box);

            camera.useFixedUpdate = GUILayout.Toggle(camera.useFixedUpdate, "Use FixedUpdate: ");
            camera.targetFollow = EditorGUILayout.ObjectField("Target: ", camera.targetFollow, typeof(Transform), true) as Transform;

            MovementSettings();
            HeightSettings();
            InputSettings();

            GUILayout.EndVertical();
            EditorUtility.SetDirty(camera);
        }

        private void MovementSettings()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            GUILayout.Space(10f);
            camera.movementSettingsFoldout = EditorGUILayout.Foldout(camera.movementSettingsFoldout, "Movement");

            GUILayout.EndHorizontal();

            if(camera.movementSettingsFoldout)
            {
                GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));

                GUILayout.Space(10f);

                camera.keyboardMovementSpeed = EditorGUILayout.FloatField("Movement speed: ", camera.keyboardMovementSpeed);
                camera.screenEdgeMovementSpeed = EditorGUILayout.FloatField("Screen edge movement speed: ", camera.screenEdgeMovementSpeed);
                camera.rotationSped = EditorGUILayout.FloatField("Rotation speed: ", camera.rotationSped);
                camera.followingSpeed = EditorGUILayout.FloatField("Following speed: ", camera.followingSpeed);

                GUILayout.Space(10);
                GUILayout.Label("Map limits");
                camera.limitX = EditorGUILayout.FloatField("Limit X: ", camera.limitX);
                camera.limitY = EditorGUILayout.FloatField("Limit Y: ", camera.limitY);

                GUILayout.EndVertical();
            }
        }

        private void HeightSettings()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            GUILayout.Space(10f);
            camera.heightSettingsFoldout= EditorGUILayout.Foldout(camera.heightSettingsFoldout, "Height");

            GUILayout.EndHorizontal();

            if (camera.heightSettingsFoldout)
            {
                GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
                GUILayout.Space(10f);

                camera.groundMask = LayerMaskField("Ground mask: ", camera.groundMask);
                camera.minHeight = EditorGUILayout.FloatField("Min height: ", camera.minHeight);
                camera.maxHeight = EditorGUILayout.FloatField("Max height: ", camera.maxHeight);
                camera.zoomSpeed = EditorGUILayout.FloatField("Zoom speed: ", camera.zoomSpeed);

                GUILayout.EndVertical();
            }
        }

        private void MapLimitsSettings()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            GUILayout.Space(10f);
            camera.mapLimitSettingsFoldout = EditorGUILayout.Foldout(camera.mapLimitSettingsFoldout, "Map limits");

            GUILayout.EndHorizontal();

            if (camera.mapLimitSettingsFoldout)
            {
                GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));



                GUILayout.Space(10f);



                GUILayout.EndVertical();
            }
        }

        private void InputSettings()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            GUILayout.Space(10f);
            camera.inputSettingsFoldout = EditorGUILayout.Foldout(camera.inputSettingsFoldout, "Input");

            GUILayout.EndHorizontal();

            if (camera.inputSettingsFoldout)
            {
                GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));

                GUILayout.Space(10f);

                camera.useKeyboardInput = EditorGUILayout.Toggle("Use keyboard input: ", camera.useKeyboardInput);
                if(camera.useKeyboardInput)
                {
                    camera.horizontalAxis = EditorGUILayout.TextField("Horizontal axis name: ", camera.horizontalAxis);
                    camera.verticalAxis = EditorGUILayout.TextField("Vertical axis name: ", camera.verticalAxis);
                }

                camera.useScreenEdgeInput = EditorGUILayout.Toggle("Use screen edge input: ", camera.useScreenEdgeInput);
                if (camera.useScreenEdgeInput)
                    EditorGUILayout.FloatField("Screen edge border size: ", camera.screenEdgeBorder);

                camera.useKeyboardRotation = EditorGUILayout.Toggle("Use keyboard rotation: ", camera.useKeyboardRotation);
                if(camera.useKeyboardRotation)
                {
                    camera.rotateLeftKey = (KeyCode)EditorGUILayout.EnumPopup("Rotate left key: ", camera.rotateLeftKey);
                    camera.rotateRightKey = (KeyCode)EditorGUILayout.EnumPopup("Rotate right key: ", camera.rotateRightKey);
                }

                camera.useKeyboardZooming = EditorGUILayout.Toggle("Use keyboard zooming: ", camera.useKeyboardZooming);
                if(camera.useKeyboardZooming)
                {
                    camera.zoomInKey = (KeyCode)EditorGUILayout.EnumPopup("Zoom in key: ", camera.zoomInKey);
                    camera.zoomOutKey = (KeyCode)EditorGUILayout.EnumPopup("Zoom out key: ", camera.zoomOutKey);
                }
                GUILayout.EndVertical();
            }
        }

        private LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }
    }
}