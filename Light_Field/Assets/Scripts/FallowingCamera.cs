using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class FallowingCamera : MonoBehaviour {

	public Material mat;
	public RenderTexture RT;

	//private SteamVR a;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Graphics.Blit(null, RT, mat, 0);

		if(Input.GetKeyDown(KeyCode.A))
		{
			Texture2D tex = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
			RenderTexture.active = RT;
			tex.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0, false);

			tex.EncodeToJPG();
		}
	}
}
