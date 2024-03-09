using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyIconBeautifier
{

    static bool _hierarchyHasFocus = false;
    static EditorWindow _hierarchyEditorWindow;

    static HierarchyIconBeautifier()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        EditorApplication.update += OnEditorUpdate;
    }

    static void OnEditorUpdate()
    {
        if (_hierarchyEditorWindow == null)
        {
            _hierarchyEditorWindow =
                EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));

            _hierarchyHasFocus = EditorWindow.focusedWindow != null &&
                                 EditorWindow.focusedWindow == _hierarchyEditorWindow;
        }
    }

    static void OnHierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        if (obj == null) 
            return;

        // GameObject is a Prefab
        if (PrefabUtility.GetCorrespondingObjectFromSource(obj) != null)
            return;
        
        Component[] components = obj.GetComponents<Component>();
        if (components == null || components.Length == 0)
            return;
        
        // most important Component right after transform
        Component component = components.Length > 1 ? components[1] : components[0];

        Type type = component.GetType();

        // some Canvas Elements are structured Rect Transform > Canvas Renderer > ... (your important Component)
        if (type == typeof(CanvasRenderer))
        {
            component = components[2];
            type = component.GetType();
        }

        GUIContent content = EditorGUIUtility.ObjectContent(component, type);
        content.text = null;
        content.tooltip = type.Name;

        if (content.image == null)
            return;

        bool isSelected = Selection.instanceIDs.Contains(instanceId);
        bool isHovered = selectionRect.Contains(Event.current.mousePosition);

        Color color = UnityEditorBackgroundColor.Get(isSelected, isHovered, _hierarchyHasFocus);
        Rect backgroundRect = selectionRect;
        backgroundRect.width = 18.5f;
        EditorGUI.DrawRect(backgroundRect, color);
        
        EditorGUI.LabelField(selectionRect, content);
    }
}
