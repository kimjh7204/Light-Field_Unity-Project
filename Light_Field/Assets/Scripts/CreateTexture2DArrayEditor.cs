using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreateTexture2DArray))]
public class CreateTexture2DArrayEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		CreateTexture2DArray myScript = target as CreateTexture2DArray;
		if (GUILayout.Button("Create Texture Array"))
		{
			myScript.CreateTexArray();
		}
	}
}
