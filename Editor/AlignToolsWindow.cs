using UnityEditor;
using UnityEngine;

namespace litefeel.AlignTools
{
    public class AlignToolsWindow : EditorWindow
    {

        const int AXIS_X = 0;
        const int AXIS_Y = 1;
        const int AXIS_Z = 2;

        private Ruler _ruler;
        private string[] _modesStr = new string[] { "UGUI", "World" };

        private bool needPepaintScene = false;

        // Update the editor window when user changes something (mainly useful when selecting objects)
        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            // head
            EditorGUI.BeginChangeCheck();
            Settings.OperatorModeInt = GUILayout.Toolbar(Settings.OperatorModeInt, _modesStr);
            needPepaintScene = EditorGUI.EndChangeCheck();

            switch (Settings.OperatorMode)
            {
                case OperatorMode.UGUI:
                    ShowUGUIMode();
                    break;
                case OperatorMode.World:
                    ShowWorldMode();
                    break;
            }
            AdjustPosition.Execute();
            if (needPepaintScene)
                SceneView.RepaintAll();
        }
        
        private void ShowUGUIMode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_left", AlignTools.AlignLeft, "Align Left");
            DrawButton("align_center_h", AlignTools.AlignCenterH, "Align Center by Horizontal");
            DrawButton("align_right", AlignTools.AlignRight, "Align Right");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_top", AlignTools.AlignTop, "Align Top");
            DrawButton("align_center_v", AlignTools.AlignCenterV, "Align Center by Vertical");
            DrawButton("align_bottom", AlignTools.AlignBottom, "Align Bottom");
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("distribution_h", AlignTools.DistributionGapHorizontal, "Distribute by Horizontal");
            DrawButton("distribution_v", AlignTools.DistributionGapVertical, "Distribute by Vertical");
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("expand_h", AlignTools.ExpandWidth, "Expand Size by Horizontal");
            DrawButton("expand_v", AlignTools.ExpandHeight, "Expand Size by Vertical");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("shrink_h", AlignTools.ShrinkWidth, "Shrink Size by Horizontal");
            DrawButton("shrink_v", AlignTools.ShrinkHeight, "Shrink Size by Vertical");
            EditorGUILayout.EndHorizontal();


            DrawLine();
            Settings.AdjustPositionByKeyboard = EditorGUILayout.ToggleLeft("Adjust Position By Keyboard", Settings.AdjustPositionByKeyboard);
            DrawLine();
            if (null == _ruler) _ruler = new Ruler();
            EditorGUI.BeginChangeCheck();
            Settings.ShowRuler = EditorGUILayout.ToggleLeft("Show Ruler", Settings.ShowRuler);
            needPepaintScene |= EditorGUI.EndChangeCheck();

            if (Settings.ShowRuler)
            {
                EditorGUI.BeginChangeCheck();
                Settings.RulerLineColor = EditorGUILayout.ColorField("Ruler Line Color", Settings.RulerLineColor);
                needPepaintScene |= EditorGUI.EndChangeCheck();
            }
        }
        private void ShowWorldMode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_left", AlignToolsWorld.AlignToMin, AXIS_X, "Align Left");
            DrawButton("align_center_h", AlignToolsWorld.AlignToCenter, AXIS_X, "Align Center by Horizontal");
            DrawButton("align_right", AlignToolsWorld.AlignToMax, AXIS_X, "Align Right");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_top", AlignToolsWorld.AlignToMax, AXIS_Y, "Align Top");
            DrawButton("align_center_v", AlignToolsWorld.AlignToCenter, AXIS_Y, "Align Center by Vertical");
            DrawButton("align_bottom", AlignToolsWorld.AlignToMin, AXIS_Y, "Align Bottom");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawButton("align_top", AlignToolsWorld.AlignToMax, AXIS_Z, "Align Top");
            DrawButton("align_center_v", AlignToolsWorld.AlignToCenter, AXIS_Z, "Align Center by Vertical");
            DrawButton("align_bottom", AlignToolsWorld.AlignToMin, AXIS_Z, "Align Bottom");
            EditorGUILayout.EndHorizontal();

            DrawLine();
            EditorGUILayout.BeginHorizontal();
            DrawButton("distribution_h", AlignToolsWorld.Distribution, AXIS_X, "Distribute by Horizontal");
            DrawButton("distribution_v", AlignToolsWorld.Distribution, AXIS_Y, "Distribute by Vertical");
            DrawButton("distribution_v", AlignToolsWorld.Distribution, AXIS_Z, "Distribute by Vertical");
            EditorGUILayout.EndHorizontal();

            //DrawLine();
            //EditorGUILayout.BeginHorizontal();
            //DrawButton("expand_h", AlignTools.ExpandWidth, "Expand Size by Horizontal");
            //DrawButton("expand_v", AlignTools.ExpandHeight, "Expand Size by Vertical");
            //EditorGUILayout.EndHorizontal();
            //EditorGUILayout.BeginHorizontal();
            //DrawButton("shrink_h", AlignTools.ShrinkWidth, "Shrink Size by Horizontal");
            //DrawButton("shrink_v", AlignTools.ShrinkHeight, "Shrink Size by Vertical");
            //EditorGUILayout.EndHorizontal();

            DrawLine();
            Settings.AdjustPositionByKeyboard = EditorGUILayout.ToggleLeft("Adjust Position By Keyboard", Settings.AdjustPositionByKeyboard);
        }
        private void DrawLine()
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        private GUIContent btnContent;
        private void DrawButton(string iconName, System.Action action, string tooltip = null)
        {
            if (null == btnContent) btnContent = new GUIContent();
            btnContent.image = Utils.LoadTexture(iconName);
            btnContent.tooltip = tooltip;
            if (GUILayout.Button(btnContent, GUILayout.ExpandWidth(false)))
                action();
        }
        private void DrawButton(string iconName, System.Action<int> action, int axis, string tooltip = null)
        {
            if (null == btnContent) btnContent = new GUIContent();
            btnContent.image = Utils.LoadTexture(iconName);
            btnContent.tooltip = tooltip;
            if (GUILayout.Button(btnContent, GUILayout.ExpandWidth(false)))
                action(axis);
        }


        private void OnEnable()
        {
            Utils.editorPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private void OnDisable()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
        }

        private void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            AdjustPosition.Execute();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            AdjustPosition.Execute();
            if (_ruler != null && Settings.OperatorMode == OperatorMode.UGUI)
                _ruler.OnSceneGUI(sceneView);
        }

    }
}


