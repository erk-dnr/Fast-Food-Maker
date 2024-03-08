using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

// This component will not persist on the GameObject, it will destroy itself immediately after renaming the object
public class RemoveSuffixFromGameObject : MonoBehaviour
{

    Regex regex;

    void Reset()
    {
        var namingScheme = EditorSettings.gameObjectNamingScheme;
        string objectName = gameObject.name;

        // find part of the string that should be removed
        regex = namingScheme switch
        {
            // ObjectName.1
            EditorSettings.NamingScheme.Dot => new Regex(@"\.\d+$"),
            // ObjectName_1
            EditorSettings.NamingScheme.Underscore => new Regex(@"_\d+$"),
            // ObjectName (1)
            EditorSettings.NamingScheme.SpaceParenthesis => new Regex(@"\s?\(\d+\)$"),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        string[] splitParts = regex.Split(objectName);

        gameObject.name = splitParts[0];
        
        DestroyImmediate(this);
    }
}
