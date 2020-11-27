using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorController : Editor
{

    TreeGenerator TG;

    private void OnEnable()
    {

        TG = (TreeGenerator)target;

    }

    override public void OnInspectorGUI()
    {
        
        DrawDefaultInspector();
        
        if (GUILayout.Button("Apply"))
        {

            TG.GenerateTree();

        }

    }

}
