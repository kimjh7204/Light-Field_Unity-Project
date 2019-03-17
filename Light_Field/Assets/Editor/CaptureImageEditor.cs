using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaptureImage))]
public class CaptureImageEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		CaptureImage myScript = target as CaptureImage;
		if (GUILayout.Button("Take Picture"))
		{
			myScript.TakeImage();
		}
	}
}
